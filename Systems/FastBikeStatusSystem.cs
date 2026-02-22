// File: Systems/FastBikeStatusSystem.cs
// Purpose: On-demand in-world runtime counts for personal vehicles.
// Notes:
// - Snapshot built on-demand (Options UI).
// - EntityQuery + ComponentLookup used to avoid per-frame work.
// - Base filter uses Game.Vehicles.PersonalCar (includes bicycles + e-scooters).
// - Bicycle-group gate uses Game.Prefabs.BicycleData on the prefab entity.
// - In-world definition:
//   - Parked = Game.Vehicles.ParkedCar
//   - Active = Game.Vehicles.CarCurrentLane (Parked wins if both)
// - Total shown in UI = Parked + Active (pending excluded by design).
// - Trailers are excluded (Game.Vehicles.CarTrailer) so car totals reflect drivable cars.
// - Border-hidden detection prefers Game.Net.OutsideConnection on ParkedCar.m_Lane;
//   falls back to Game.Net.ConnectionLane + owner-chain probe.

namespace FastBikes
{
    using Game;                // GameSystemBase
    using Game.Citizens;       // TouristHousehold
    using Game.Common;         // Deleted, Destroyed, Owner
    using Game.Objects;        // Unspawned
    using Game.Prefabs;        // BicycleData, PrefabBase, PrefabRef, PrefabSystem
    using Game.Tools;          // Temp
    using Game.Vehicles;       // CarCurrentLane, CarTrailer, ParkedCar
    using System;              // DateTime, StringComparison
    using System.Collections.Generic;
    using System.Text;
    using Unity.Collections;   // Allocator, NativeArray
    using Unity.Entities;      // Entity, EntityQuery, ComponentLookup, ComponentType

    public sealed partial class FastBikeStatusSystem : GameSystemBase
    {
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
            public readonly long CarHiddenAtBorder;
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

            long carHiddenAtBorder = 0;
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

                    if (isParked && unspawnedLookup.HasComponent(e) && ownerLookup.HasComponent(e))
                    {
                        Owner o = ownerLookup[e];
                        if (o.m_Owner != Entity.Null)
                        {
                            Entity lane = parkedLookup[e].m_Lane;

                            if (IsOutsideConnectionLane(lane))
                            {
                                carHiddenAtBorder++;
                            }
                            else
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

            ComponentLookup<Game.Net.OutsideConnection> outsideConnLookup =
                GetComponentLookup<Game.Net.OutsideConnection>(isReadOnly: true);

            ComponentLookup<Game.Net.ConnectionLane> connLaneLookup =
                GetComponentLookup<Game.Net.ConnectionLane>(isReadOnly: true);

            ComponentLookup<TouristHousehold> touristHouseholdLookup =
                GetComponentLookup<TouristHousehold>(isReadOnly: true);

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

            int total = 0;
            int touristOwners = 0;
            int otherOwners = 0;

            List<Entity> head = new List<Entity>(headCount);
            List<Entity> tail = new List<Entity>(tailCount);

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

                    if (!ownerLookup.HasComponent(e))
                    {
                        continue;
                    }

                    Owner o = ownerLookup[e];
                    if (o.m_Owner == Entity.Null)
                    {
                        continue;
                    }

                    Entity lane = parkedLookup[e].m_Lane;
                    if (!IsOutsideConnectionLane(lane))
                    {
                        continue;
                    }

                    total++;

                    if (touristHouseholdLookup.HasComponent(o.m_Owner))
                    {
                        touristOwners++;
                    }
                    else
                    {
                        otherOwners++;
                    }

                    AddHeadTailSample(e, head, tail, headCount, tailCount);
                }
            }

            Mod.LogSafe(( ) =>
            {
                StringBuilder sb = new System.Text.StringBuilder();

                sb.AppendLine("\n==================== [FB] HIDDEN CARS AT BORDER OC (SAMPLES) ====================");
                sb.AppendLine("Meaning: Parked + Unspawned + Owner, parked lane is OC/border.");
                sb.AppendLine($"Total={total} (TouristOwners={touristOwners}, Citizens={otherOwners})");
                sb.AppendLine("Samples are VehicleIndex:Version. Use Scene Explorer to Jump To.");
                sb.AppendLine();

                sb.Append("Head: ");
                AppendEntitySamples(sb, head);
                sb.AppendLine();

                sb.Append("Tail: ");
                AppendEntitySamples(sb, tail);
                sb.AppendLine();

                return sb.ToString();
            });

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
