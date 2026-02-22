// File: Systems/FastBikeStatusSystem.cs
// Purpose: On-demand button for counts of personal vehicles.
// Notes:
// - Snapshot built on-demand button (Options UI).
// - Base filter uses Game.Vehicles.PersonalCar (includes bicycles + e-scooters).
// - In-world definition:
//   - Parked = Game.Vehicles.ParkedCar
//   - Active = Game.Vehicles.CarCurrentLane (Parked wins if both)
// - Total shown in UI = Parked + Active (excludes pending).
// - CarTrailers are excluded so car totals reflect drivable cars.
// - Border-hidden detection prefers Game.Net.OutsideConnection on ParkedCar.m_Lane;
//   falls back to Game.Net.ConnectionLane + owner-chain probe.
// - Status "Hidden at border OC" excludes owner households currently at OC (CurrentBuilding -> OutsideConnection).

namespace FastBikes
{
    using Game;                // GameSystemBase
    using Game.Citizens;       // TouristHousehold, CurrentBuilding, HouseholdMember
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
        private readonly struct HiddenOcCar
        {
            public readonly Entity Vehicle;
            public readonly Entity Household;
            public readonly bool IsTourist;

            public HiddenOcCar(Entity vehicle, Entity household, bool isTourist)
            {
                Vehicle = vehicle;
                Household = household;
                IsTourist = isTourist;
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

            // "Hidden at border OC" used by UI status row:
            // - Parked + Unspawned + Owner
            // - Parked lane at Outside Connection
            // - Owner household is not tourist
            // - Owner household has no member currently at OC (CurrentBuilding -> OutsideConnection)
            public readonly long CarHiddenAtBorder;

            // Parked + Unspawned + Owner but not at OC lane (typically hidden inside buildings)
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

            // Snapshot base query:
            // - PersonalCar includes bicycles + e-scooters; BicycleData on prefab splits Bike group vs Car group
            // - Trailers excluded for Car group totals
            m_PersonalVehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.Exclude<CarTrailer>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Destroyed>());

            // Trailer count shown in Cars row
            m_TrailerQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.ReadOnly<CarTrailer>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>(),
                ComponentType.Exclude<Destroyed>());

            // Owner-at-OC detection:
            // - Citizen CurrentBuilding pointing to an entity with OutsideConnection is treated as "at OC"
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

        public Snapshot BuildSnapshot( )
        {
            ComponentLookup<PrefabRef> prefabRefLookup = GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<BicycleData> bicycleDataLookup = GetComponentLookup<BicycleData>(isReadOnly: true);

            ComponentLookup<ParkedCar> parkedLookup = GetComponentLookup<ParkedCar>(isReadOnly: true);
            ComponentLookup<CarCurrentLane> currentLaneLookup = GetComponentLookup<CarCurrentLane>(isReadOnly: true);

            ComponentLookup<Unspawned> unspawnedLookup = GetComponentLookup<Unspawned>(isReadOnly: true);
            ComponentLookup<Owner> ownerLookup = GetComponentLookup<Owner>(isReadOnly: true);

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

            // ParkedCar.m_Lane OC detection:
            // - Prefer OutsideConnection on lane
            // - Fallback: ConnectionLane + owner-chain probe
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

            long carHiddenAtBorder = 0;
            long carHiddenInBuildings = 0;

            // Candidate list:
            // - Hidden-at-OC cars with non-tourist owners only
            // - Owner-at-OC households filtered in second pass
            NativeList<HiddenOcCar> hiddenOcCars = new NativeList<HiddenOcCar>(256, Allocator.Temp);
            NativeParallelHashSet<Entity> hiddenOcHouseholds = new NativeParallelHashSet<Entity>(256, Allocator.Temp);

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

                    // In-world definition:
                    // - Parked wins if both ParkedCar and CarCurrentLane exist
                    bool isParked = parkedLookup.HasComponent(e);
                    bool isActive = !isParked && currentLaneLookup.HasComponent(e);

                    // Pending/other states excluded by design
                    if (!isParked && !isActive)
                    {
                        continue;
                    }

                    bool isBikeGroup = bicycleDataLookup.HasComponent(prefabEntity);

                    if (isBikeGroup)
                    {
                        bikeGroupTotal++;

                        if (isParked)
                        {
                            bikeGroupParked++;
                        }
                        else
                        {
                            bikeGroupActive++;
                        }

                        if (IsElectricScooterPrefab(prefabEntity))
                        {
                            scooterTotal++;
                        }

                        continue;
                    }

                    carGroupTotal++;

                    if (isParked)
                    {
                        carGroupParked++;
                    }
                    else
                    {
                        carGroupActive++;
                    }

                    // Hidden buckets are parked-only
                    if (!isParked)
                    {
                        continue;
                    }

                    // Hidden = Unspawned + Owner + ParkedCar
                    if (!unspawnedLookup.HasComponent(e))
                    {
                        continue;
                    }

                    if (!ownerLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Owner o = ownerLookup[e];
                    Entity household = o.m_Owner;
                    if (household == Entity.Null)
                    {
                        continue;
                    }

                    Entity lane = parkedLookup[e].m_Lane;

                    if (IsOutsideConnectionLane(lane))
                    {
                        // Status excludes tourists
                        bool isTourist = touristHouseholdLookup.HasComponent(household);
                        if (!isTourist)
                        {
                            hiddenOcCars.Add(new HiddenOcCar(e, household, isTourist: false));
                            hiddenOcHouseholds.Add(household);
                        }
                    }
                    else
                    {
                        carHiddenInBuildings++;
                    }
                }
            }

            if (hiddenOcCars.Length > 0)
            {
                int cap = hiddenOcHouseholds.Count() + 16;
                if (cap < 64)
                {
                    cap = 64;
                }

                // Household-at-OC set:
                // - Any citizen in the household has CurrentBuilding that is an OutsideConnection entity
                NativeParallelHashSet<Entity> householdsAtOc = new NativeParallelHashSet<Entity>(cap, Allocator.Temp);

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

                        if (!hiddenOcHouseholds.Contains(household))
                        {
                            continue;
                        }

                        Entity curBuilding = currentBuildingLookup[c].m_CurrentBuilding;
                        if (curBuilding == Entity.Null)
                        {
                            continue;
                        }

                        if (!outsideConnLookup.HasComponent(curBuilding))
                        {
                            continue;
                        }

                        householdsAtOc.Add(household);
                    }
                }

                // Status count:
                // - Non-tourist households already filtered in candidate list
                // - Owner-at-OC households excluded here
                for (int i = 0; i < hiddenOcCars.Length; i++)
                {
                    HiddenOcCar hc = hiddenOcCars[i];

                    if (householdsAtOc.Contains(hc.Household))
                    {
                        continue;
                    }

                    carHiddenAtBorder++;
                }

                householdsAtOc.Dispose();
            }

            hiddenOcCars.Dispose();
            hiddenOcHouseholds.Dispose();

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
                carHiddenAtBorder: carHiddenAtBorder,
                carHiddenInBuildings: carHiddenInBuildings,
                snapshotTimeLocal: DateTime.Now);
        }

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

            ComponentLookup<PrefabRef> prefabRefLookup = GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<BicycleData> bicycleDataLookup = GetComponentLookup<BicycleData>(isReadOnly: true);

            ComponentLookup<ParkedCar> parkedLookup = GetComponentLookup<ParkedCar>(isReadOnly: true);
            ComponentLookup<Unspawned> unspawnedLookup = GetComponentLookup<Unspawned>(isReadOnly: true);
            ComponentLookup<Owner> ownerLookup = GetComponentLookup<Owner>(isReadOnly: true);

            ComponentLookup<Game.Citizens.CurrentBuilding> currentBuildingLookup =
                GetComponentLookup<Game.Citizens.CurrentBuilding>(isReadOnly: true);

            ComponentLookup<Game.Citizens.HouseholdMember> householdMemberLookup =
                GetComponentLookup<Game.Citizens.HouseholdMember>(isReadOnly: true);

            ComponentLookup<Game.Net.OutsideConnection> outsideConnLookup =
                GetComponentLookup<Game.Net.OutsideConnection>(isReadOnly: true);

            ComponentLookup<Game.Net.ConnectionLane> connLaneLookup =
                GetComponentLookup<Game.Net.ConnectionLane>(isReadOnly: true);

            ComponentLookup<TouristHousehold> touristHouseholdLookup =
                GetComponentLookup<TouristHousehold>(isReadOnly: true);

            // ParkedCar.m_Lane OC detection; same rule as snapshot
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

            // Candidate list for logging:
            // - Includes tourist + non-tourist
            NativeList<HiddenOcCar> hiddenOcCars = new NativeList<HiddenOcCar>(256, Allocator.Temp);
            NativeParallelHashSet<Entity> hiddenOcHouseholds = new NativeParallelHashSet<Entity>(256, Allocator.Temp);

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

                    // Cars only (exclude Bike group)
                    if (bicycleDataLookup.HasComponent(prefabEntity))
                    {
                        continue;
                    }

                    // Hidden candidates: Parked + Unspawned + Owner
                    if (!parkedLookup.HasComponent(e))
                    {
                        continue;
                    }

                    if (!unspawnedLookup.HasComponent(e))
                    {
                        continue;
                    }

                    if (!ownerLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Owner o = ownerLookup[e];
                    Entity household = o.m_Owner;
                    if (household == Entity.Null)
                    {
                        continue;
                    }

                    // Border OC only
                    Entity lane = parkedLookup[e].m_Lane;
                    if (!IsOutsideConnectionLane(lane))
                    {
                        continue;
                    }

                    bool isTourist = touristHouseholdLookup.HasComponent(household);

                    hiddenOcCars.Add(new HiddenOcCar(e, household, isTourist));
                    hiddenOcHouseholds.Add(household);
                }
            }

            // Household-at-OC set:
            // - Any citizen in household has CurrentBuilding pointing to OutsideConnection entity
            NativeParallelHashSet<Entity> householdsAtOc = new NativeParallelHashSet<Entity>(
                hiddenOcHouseholds.Count() + 16,
                Allocator.Temp);

            if (hiddenOcCars.Length > 0)
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

                        if (!hiddenOcHouseholds.Contains(household))
                        {
                            continue;
                        }

                        Entity curBuilding = currentBuildingLookup[c].m_CurrentBuilding;
                        if (curBuilding == Entity.Null)
                        {
                            continue;
                        }

                        if (!outsideConnLookup.HasComponent(curBuilding))
                        {
                            continue;
                        }

                        householdsAtOc.Add(household);
                    }
                }
            }

            int total = hiddenOcCars.Length;

            int cityOwnerNotAtOc = 0;  // matches status intent (non-tourist + owner household not at OC)
            int ownerAlsoAtOc = 0;     // separate issue bucket (log only)

            int ownerAlsoAtOcTourist = 0;
            int ownerAlsoAtOcOther = 0;

            // Head = first N matches encountered; Tail = last N matches encountered (rolling buffer)
            List<Entity> headCityOwnerNotAtOc = new List<Entity>(headCount);
            List<Entity> tailCityOwnerNotAtOc = new List<Entity>(tailCount);

            List<Entity> headOwnerAlsoAtOc = new List<Entity>(headCount);
            List<Entity> tailOwnerAlsoAtOc = new List<Entity>(tailCount);

            for (int i = 0; i < hiddenOcCars.Length; i++)
            {
                HiddenOcCar hc = hiddenOcCars[i];

                bool isOwnerAtOc = householdsAtOc.Contains(hc.Household);

                if (isOwnerAtOc)
                {
                    ownerAlsoAtOc++;

                    if (hc.IsTourist)
                    {
                        ownerAlsoAtOcTourist++;
                    }
                    else
                    {
                        ownerAlsoAtOcOther++;
                    }

                    AddHeadTailSample(hc.Vehicle, headOwnerAlsoAtOc, tailOwnerAlsoAtOc, headCount, tailCount);
                    continue;
                }

                // Status bucket logic: exclude tourists, exclude owner-at-OC
                if (!hc.IsTourist)
                {
                    cityOwnerNotAtOc++;
                    AddHeadTailSample(hc.Vehicle, headCityOwnerNotAtOc, tailCityOwnerNotAtOc, headCount, tailCount);
                }
            }

            Mod.LogSafe(( ) =>
            {
                StringBuilder sb = new System.Text.StringBuilder();

                sb.AppendLine("\n==================== [FB] HIDDEN CARS AT BORDER OC (SAMPLES) ====================");
                sb.AppendLine("Meaning: Parked + Unspawned + Owner, parked at OC lane outside border.");
                sb.AppendLine("Status count uses CityOwnerNotAtOC only.");
                sb.AppendLine($"TotalCandidates={total}");
                sb.AppendLine($"CityOwnerNotAtOC(Status)={cityOwnerNotAtOc}");
                sb.AppendLine($"OwnerAlsoAtOC(LogOnly)={ownerAlsoAtOc} (TouristOwners={ownerAlsoAtOcTourist}, Others={ownerAlsoAtOcOther})");
                sb.AppendLine("Samples are VehicleIndex:Version. Use Scene Explorer to Jump To.");
                sb.AppendLine();

                sb.Append("CityOwnerNotAtOC Head: ");
                AppendEntitySamples(sb, headCityOwnerNotAtOc);
                sb.AppendLine();

                sb.Append("CityOwnerNotAtOC Tail: ");
                AppendEntitySamples(sb, tailCityOwnerNotAtOc);
                sb.AppendLine();

                sb.AppendLine();

                sb.Append("OwnerAlsoAtOC Head: ");
                AppendEntitySamples(sb, headOwnerAlsoAtOc);
                sb.AppendLine();

                sb.Append("OwnerAlsoAtOC Tail: ");
                AppendEntitySamples(sb, tailOwnerAlsoAtOc);
                sb.AppendLine();

                return sb.ToString();
            });

            householdsAtOc.Dispose();
            hiddenOcCars.Dispose();
            hiddenOcHouseholds.Dispose();

            static void AddHeadTailSample(Entity e, List<Entity> headList, List<Entity> tailList, int headMax, int tailMax)
            {
                if (headList.Count < headMax)
                {
                    headList.Add(e);
                }

                if (tailList.Count == tailMax)
                {
                    tailList.RemoveAt(0);
                }

                tailList.Add(e);
            }

            static void AppendEntitySamples(System.Text.StringBuilder sb, List<Entity> items)
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
