// File: Systems/FastBikeStatusSystem.cs
// Purpose: Snapshot counts for Options UI + on-demand OC hidden-car bucket report.

namespace FastBikes
{
    using Game;                // GameSystemBase
    using Game.Citizens;       // TouristHousehold, CurrentBuilding, HouseholdMember, Household
    using Game.Common;         // Deleted, Destroyed, Owner
    using Game.Objects;        // Unspawned
    using Game.Prefabs;        // BicycleData, PrefabBase, PrefabRef, PrefabSystem
    using Game.Tools;          // Temp
    using Game.Vehicles;       // CarCurrentLane, CarTrailer, ParkedCar, PersonalCar
    using System;              // DateTime, StringComparison
    using System.Collections.Generic;
    using System.Text;
    using Unity.Collections;   // Allocator, NativeArray, NativeList, NativeParallelHashSet
    using Unity.Entities;      // Entity, EntityQuery, ComponentLookup, ComponentType

    public sealed partial class FastBikeStatusSystem : GameSystemBase
    {
        private readonly struct OcHiddenCarOwned
        {
            public readonly Entity Vehicle;
            public readonly Entity Household;

            public OcHiddenCarOwned(Entity vehicle, Entity household)
            {
                Vehicle = vehicle;
                Household = household;
            }
        }

        public readonly struct Snapshot
        {
            public readonly long BikeGroupTotal;
            public readonly long BikeGroupParked;
            public readonly long BikeGroupActive;
            public readonly long ScooterTotal;
            public readonly long BikeOnlyTotal;

            public readonly long CarGroupTotal;
            public readonly long CarGroupParked;
            public readonly long CarGroupActive;

            public readonly long TrailerTotal;

            // Status row3: TOTAL OC hidden cars (car-group only)
            public readonly long CarHiddenAtBorder;

            // Diagnostic only
            public readonly long CarHiddenInBuildings;

            public readonly DateTime SnapshotTimeLocal;

            public Snapshot(
                long bikeGroupTotal,
                long bikeGroupParked,
                long bikeGroupActive,
                long scooterTotal,
                long bikeOnlyTotal,
                long carGroupTotal,
                long carGroupParked,
                long carGroupActive,
                long trailerTotal,
                long carHiddenAtBorder,
                long carHiddenInBuildings,
                DateTime snapshotTimeLocal)
            {
                BikeGroupTotal = bikeGroupTotal;
                BikeGroupParked = bikeGroupParked;
                BikeGroupActive = bikeGroupActive;
                ScooterTotal = scooterTotal;
                BikeOnlyTotal = bikeOnlyTotal;

                CarGroupTotal = carGroupTotal;
                CarGroupParked = carGroupParked;
                CarGroupActive = carGroupActive;

                TrailerTotal = trailerTotal;
                CarHiddenAtBorder = carHiddenAtBorder;
                CarHiddenInBuildings = carHiddenInBuildings;

                SnapshotTimeLocal = snapshotTimeLocal;
            }
        }

        private PrefabSystem m_PrefabSystem = null!;
        private EntityQuery m_PersonalVehicleQuery;
        private EntityQuery m_TrailerQuery;
        private EntityQuery m_CitizenLocQuery;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            m_PersonalVehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.Exclude<CarTrailer>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Destroyed>());

            m_TrailerQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.ReadOnly<CarTrailer>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Destroyed>());

            m_CitizenLocQuery = GetEntityQuery(
                ComponentType.ReadOnly<Game.Citizens.Citizen>(),
                ComponentType.ReadOnly<Game.Citizens.CurrentBuilding>(),
                ComponentType.ReadOnly<Game.Citizens.HouseholdMember>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Destroyed>());

            Enabled = false;
        }

        protected override void OnUpdate( )
        {
        }

        // Builds the snapshot used by Options UI rows.
        // Row3 uses TOTAL OC hidden (ParkedCar + Unspawned + OC lane) for car-group only.
        public Snapshot BuildSnapshot( )
        {
            ComponentLookup<PrefabRef> prefabRefLookup = GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<BicycleData> bicycleDataLookup = GetComponentLookup<BicycleData>(isReadOnly: true);

            ComponentLookup<ParkedCar> parkedLookup = GetComponentLookup<ParkedCar>(isReadOnly: true);
            ComponentLookup<CarCurrentLane> currentLaneLookup = GetComponentLookup<CarCurrentLane>(isReadOnly: true);

            ComponentLookup<Unspawned> unspawnedLookup = GetComponentLookup<Unspawned>(isReadOnly: true);
            ComponentLookup<Owner> ownerLookup = GetComponentLookup<Owner>(isReadOnly: true);

            ComponentLookup<Game.Net.OutsideConnection> outsideConnLookup =
                GetComponentLookup<Game.Net.OutsideConnection>(isReadOnly: true);

            ComponentLookup<Game.Net.ConnectionLane> connLaneLookup =
                GetComponentLookup<Game.Net.ConnectionLane>(isReadOnly: true);

            bool IsOutsideConnectionLane(Entity lane)
            {
                if (lane == Entity.Null)
                {
                    return false;
                }

                if (outsideConnLookup.HasComponent(lane))
                {
                    return true;
                }

                if (!connLaneLookup.HasComponent(lane))
                {
                    return false;
                }

                Entity cur = lane;
                for (int i = 0; i < 6; i++)
                {
                    if (outsideConnLookup.HasComponent(cur))
                    {
                        return true;
                    }

                    if (!ownerLookup.HasComponent(cur))
                    {
                        break;
                    }

                    Owner o = ownerLookup[cur];
                    if (o.m_Owner == Entity.Null)
                    {
                        break;
                    }

                    cur = o.m_Owner;
                }

                return false;
            }

            long bikeGroupTotal = 0;
            long bikeGroupParked = 0;
            long bikeGroupActive = 0;
            long scooterTotal = 0;

            long carGroupTotal = 0;
            long carGroupParked = 0;
            long carGroupActive = 0;

            long totalOcHidden = 0;
            long carHiddenInBuildings = 0;

            using (NativeArray<Entity> entities = m_PersonalVehicleQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Entity e = entities[i];

                    if (!prefabRefLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Entity prefabEntity = prefabRefLookup[e].m_Prefab;
                    if (prefabEntity == Entity.Null)
                    {
                        continue;
                    }

                    bool isParked = parkedLookup.HasComponent(e);
                    bool isActive = !isParked && currentLaneLookup.HasComponent(e);

                    if (!isParked && !isActive)
                    {
                        continue;
                    }

                    bool isBikeGroup = bicycleDataLookup.HasComponent(prefabEntity);

                    if (isBikeGroup)
                    {
                        bikeGroupTotal++;

                        if (isParked)
                            bikeGroupParked++;
                        else
                            bikeGroupActive++;

                        if (IsElectricScooterPrefab(prefabEntity))
                            scooterTotal++;

                        continue;
                    }

                    carGroupTotal++;

                    if (isParked)
                        carGroupParked++;
                    else
                        carGroupActive++;

                    if (isParked && unspawnedLookup.HasComponent(e))
                    {
                        Entity lane = parkedLookup[e].m_Lane;

                        if (IsOutsideConnectionLane(lane))
                        {
                            totalOcHidden++;
                        }
                        else if (ownerLookup.HasComponent(e))
                        {
                            Owner o = ownerLookup[e];
                            if (o.m_Owner != Entity.Null)
                            {
                                carHiddenInBuildings++;
                            }
                        }
                    }
                }
            }

            long trailerTotal;
            using (NativeArray<Entity> trailerEntities = m_TrailerQuery.ToEntityArray(Allocator.Temp))
            {
                trailerTotal = trailerEntities.Length;
            }

            long bikeOnlyTotal = bikeGroupTotal - scooterTotal;
            if (bikeOnlyTotal < 0)
            {
                bikeOnlyTotal = 0;
            }

            return new Snapshot(
                bikeGroupTotal: bikeGroupTotal,
                bikeGroupParked: bikeGroupParked,
                bikeGroupActive: bikeGroupActive,
                scooterTotal: scooterTotal,
                bikeOnlyTotal: bikeOnlyTotal,
                carGroupTotal: carGroupTotal,
                carGroupParked: carGroupParked,
                carGroupActive: carGroupActive,
                trailerTotal: trailerTotal,
                carHiddenAtBorder: totalOcHidden,
                carHiddenInBuildings: carHiddenInBuildings,
                snapshotTimeLocal: DateTime.Now);
        }

        // One-line A/B/C summary for the About-tab dump report.
        public void LogOcHiddenBucketsOneLine( )
        {
            ComputeOcHiddenBuckets(
                headCount: 0,
                tailCount: 0,
                collectSamples: false,
                headA: null,
                tailA: null,
                headB: null,
                tailB: null,
                out int total,
                out int bucketA,
                out int bucketB,
                out int bucketC,
                out int sum,
                out int diff,
                out int cOwnerMissingOrNull,
                out int cOwnerNotHousehold,
                out int cTouristHousehold);

            Mod.LogSafe(( ) =>
                $"[FB] OC Hidden Cars A/B/C: Total={total}, A={bucketA}, B={bucketB}, C={bucketC}, Check={sum} ({(diff == 0 ? "OK" : "DIFF " + diff)})");
        }

        // Full bucket report with head/tail sample IDs for A and B.
        // Tail = rolling buffer of last N matches encountered during scan.
        public void LogBorderParkedSamples(int headCount = 10, int tailCount = 10)
        {
            if (headCount < 1)
            {
                headCount = 10;
            }

            if (tailCount < 1)
            {
                tailCount = 10;
            }

            List<Entity> headA = new List<Entity>(headCount);
            List<Entity> tailA = new List<Entity>(tailCount);

            List<Entity> headB = new List<Entity>(headCount);
            List<Entity> tailB = new List<Entity>(tailCount);

            ComputeOcHiddenBuckets(
                headCount,
                tailCount,
                collectSamples: true,
                headA,
                tailA,
                headB,
                tailB,
                out int total,
                out int bucketA,
                out int bucketB,
                out int bucketC,
                out int sum,
                out int diff,
                out int cOwnerMissingOrNull,
                out int cOwnerNotHousehold,
                out int cTouristHousehold);

            Mod.LogSafe(( ) =>
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine("==================== [FB] OC HIDDEN CARS (3 BUCKETS) ====================");
                sb.AppendLine("OC = outside city");
                sb.AppendLine("Hidden = ParkedCar + Unspawned (mesh not shown)");
                sb.AppendLine();

                sb.AppendLine($"Total OC Hidden Cars = {total}");
                sb.AppendLine($"Bucket A (Owner in city) = {bucketA}");
                sb.AppendLine($"Bucket B (Owner at OC)   = {bucketB}");
                sb.AppendLine($"Bucket C (Other)         = {bucketC}");
                sb.AppendLine($"Check: A+B+C = {sum} ({(diff == 0 ? "OK" : "DIFF " + diff)})");
                sb.AppendLine();

                sb.AppendLine("Bucket A samples:");
                sb.Append("Head IDs: ");
                AppendEntitySamples(sb, headA);
                sb.AppendLine();
                sb.Append("Tail IDs: ");
                AppendEntitySamples(sb, tailA);
                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine("Bucket B samples:");
                sb.Append("Head IDs: ");
                AppendEntitySamples(sb, headB);
                sb.AppendLine();
                sb.Append("Tail IDs: ");
                AppendEntitySamples(sb, tailB);
                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine("Bucket C breakdown:");
                sb.AppendLine($"- Owner missing or null = {cOwnerMissingOrNull}");
                sb.AppendLine($"- Owner not Household   = {cOwnerNotHousehold}");
                sb.AppendLine($"- Tourist household     = {cTouristHousehold}");

                return sb.ToString();
            });
        }

        private void ComputeOcHiddenBuckets(
            int headCount,
            int tailCount,
            bool collectSamples,
            List<Entity>? headA,
            List<Entity>? tailA,
            List<Entity>? headB,
            List<Entity>? tailB,
            out int totalOcHiddenCars,
            out int bucketA,
            out int bucketB,
            out int bucketC,
            out int sum,
            out int diff,
            out int cOwnerMissingOrNull,
            out int cOwnerNotHousehold,
            out int cTouristHousehold)
        {
            ComponentLookup<PrefabRef> prefabRefLookup = GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<BicycleData> bicycleDataLookup = GetComponentLookup<BicycleData>(isReadOnly: true);

            ComponentLookup<ParkedCar> parkedLookup = GetComponentLookup<ParkedCar>(isReadOnly: true);
            ComponentLookup<Unspawned> unspawnedLookup = GetComponentLookup<Unspawned>(isReadOnly: true);
            ComponentLookup<Owner> ownerLookup = GetComponentLookup<Owner>(isReadOnly: true);

            ComponentLookup<Game.Citizens.Household> householdLookup =
                GetComponentLookup<Game.Citizens.Household>(isReadOnly: true);

            ComponentLookup<TouristHousehold> touristHouseholdLookup =
                GetComponentLookup<TouristHousehold>(isReadOnly: true);

            ComponentLookup<Game.Citizens.CurrentBuilding> currentBuildingLookup =
                GetComponentLookup<Game.Citizens.CurrentBuilding>(isReadOnly: true);

            ComponentLookup<Game.Citizens.HouseholdMember> householdMemberLookup =
                GetComponentLookup<Game.Citizens.HouseholdMember>(isReadOnly: true);

            ComponentLookup<Game.Net.OutsideConnection> outsideConnLookup =
                GetComponentLookup<Game.Net.OutsideConnection>(isReadOnly: true);

            ComponentLookup<Game.Net.ConnectionLane> connLaneLookup =
                GetComponentLookup<Game.Net.ConnectionLane>(isReadOnly: true);

            if (collectSamples && (headA == null || tailA == null || headB == null || tailB == null))
            {
                collectSamples = false;
            }

            bool IsOutsideConnectionLane(Entity lane)
            {
                if (lane == Entity.Null)
                {
                    return false;
                }

                if (outsideConnLookup.HasComponent(lane))
                {
                    return true;
                }

                if (!connLaneLookup.HasComponent(lane))
                {
                    return false;
                }

                Entity cur = lane;
                for (int i = 0; i < 6; i++)
                {
                    if (outsideConnLookup.HasComponent(cur))
                    {
                        return true;
                    }

                    if (!ownerLookup.HasComponent(cur))
                    {
                        break;
                    }

                    Owner o = ownerLookup[cur];
                    if (o.m_Owner == Entity.Null)
                    {
                        break;
                    }

                    cur = o.m_Owner;
                }

                return false;
            }

            bool IsOutsideConnectionLocation(Entity e)
            {
                if (e == Entity.Null)
                {
                    return false;
                }

                if (outsideConnLookup.HasComponent(e))
                {
                    return true;
                }

                Entity cur = e;
                for (int i = 0; i < 8; i++)
                {
                    if (cur == Entity.Null)
                    {
                        break;
                    }

                    if (outsideConnLookup.HasComponent(cur))
                    {
                        return true;
                    }

                    if (!ownerLookup.HasComponent(cur))
                    {
                        break;
                    }

                    Owner o = ownerLookup[cur];
                    if (o.m_Owner == Entity.Null)
                    {
                        break;
                    }

                    cur = o.m_Owner;
                }

                if (prefabRefLookup.HasComponent(e))
                {
                    Entity prefabEntity = prefabRefLookup[e].m_Prefab;
                    if (prefabEntity != Entity.Null && m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                    {
                        string n = prefabBase.name;
                        if (!string.IsNullOrEmpty(n) && n.IndexOf("Outside Connection", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            totalOcHiddenCars = 0;

            bucketA = 0;
            bucketB = 0;
            bucketC = 0;

            cOwnerMissingOrNull = 0;
            cOwnerNotHousehold = 0;
            cTouristHousehold = 0;

            NativeList<OcHiddenCarOwned> ownedCandidates = new NativeList<OcHiddenCarOwned>(256, Allocator.Temp);
            NativeParallelHashSet<Entity> ownedHouseholds = new NativeParallelHashSet<Entity>(256, Allocator.Temp);

            using (NativeArray<Entity> entities = m_PersonalVehicleQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Entity e = entities[i];

                    if (!prefabRefLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Entity prefabEntity = prefabRefLookup[e].m_Prefab;
                    if (prefabEntity == Entity.Null)
                    {
                        continue;
                    }

                    if (bicycleDataLookup.HasComponent(prefabEntity))
                    {
                        continue;
                    }

                    if (!parkedLookup.HasComponent(e))
                    {
                        continue;
                    }

                    if (!unspawnedLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Entity lane = parkedLookup[e].m_Lane;
                    if (!IsOutsideConnectionLane(lane))
                    {
                        continue;
                    }

                    totalOcHiddenCars++;

                    if (!ownerLookup.HasComponent(e))
                    {
                        bucketC++;
                        cOwnerMissingOrNull++;
                        continue;
                    }

                    Owner o = ownerLookup[e];
                    Entity ownerEntity = o.m_Owner;
                    if (ownerEntity == Entity.Null)
                    {
                        bucketC++;
                        cOwnerMissingOrNull++;
                        continue;
                    }

                    if (!householdLookup.HasComponent(ownerEntity))
                    {
                        bucketC++;
                        cOwnerNotHousehold++;
                        continue;
                    }

                    if (touristHouseholdLookup.HasComponent(ownerEntity))
                    {
                        bucketC++;
                        cTouristHousehold++;
                        continue;
                    }

                    ownedCandidates.Add(new OcHiddenCarOwned(e, ownerEntity));
                    ownedHouseholds.Add(ownerEntity);
                }
            }

            NativeParallelHashSet<Entity> householdsAtOc = new NativeParallelHashSet<Entity>(
                ownedHouseholds.Count() + 16,
                Allocator.Temp);

            if (ownedCandidates.Length > 0)
            {
                using (NativeArray<Entity> citizens = m_CitizenLocQuery.ToEntityArray(Allocator.Temp))
                {
                    for (int i = 0; i < citizens.Length; i++)
                    {
                        Entity c = citizens[i];

                        Entity household = householdMemberLookup[c].m_Household;
                        if (household == Entity.Null)
                        {
                            continue;
                        }

                        if (!ownedHouseholds.Contains(household))
                        {
                            continue;
                        }

                        Entity curBuilding = currentBuildingLookup[c].m_CurrentBuilding;
                        if (curBuilding == Entity.Null)
                        {
                            continue;
                        }

                        if (!IsOutsideConnectionLocation(curBuilding))
                        {
                            continue;
                        }

                        householdsAtOc.Add(household);
                    }
                }
            }

            for (int i = 0; i < ownedCandidates.Length; i++)
            {
                OcHiddenCarOwned oc = ownedCandidates[i];

                if (householdsAtOc.Contains(oc.Household))
                {
                    bucketB++;

                    if (collectSamples)
                    {
                        AddHeadTailSample(oc.Vehicle, headB!, tailB!, headCount, tailCount);
                    }
                }
                else
                {
                    bucketA++;

                    if (collectSamples)
                    {
                        AddHeadTailSample(oc.Vehicle, headA!, tailA!, headCount, tailCount);
                    }
                }
            }

            sum = bucketA + bucketB + bucketC;
            diff = totalOcHiddenCars - sum;

            householdsAtOc.Dispose();
            ownedCandidates.Dispose();
            ownedHouseholds.Dispose();
        }

        private static void AddHeadTailSample(Entity e, List<Entity> headList, List<Entity> tailList, int headMax, int tailMax)
        {
            if (headMax > 0 && headList.Count < headMax)
            {
                headList.Add(e);
            }

            if (tailMax > 0)
            {
                if (tailList.Count == tailMax)
                {
                    tailList.RemoveAt(0);
                }

                tailList.Add(e);
            }
        }

        private static void AppendEntitySamples(StringBuilder sb, List<Entity> items)
        {
            if (items == null || items.Count == 0)
            {
                sb.Append("<none>");
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                Entity e = items[i];
                sb.Append(e.Index);
                sb.Append(':');
                sb.Append(e.Version);
            }
        }

        private bool IsElectricScooterPrefab(Entity prefabEntity)
        {
            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
            {
                return false;
            }

            string n = prefabBase.name;
            if (string.IsNullOrEmpty(n))
            {
                return false;
            }

            return n.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase);
        }
    }
}
