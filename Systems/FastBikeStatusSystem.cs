// File: Systems/FastBikeStatusSystem.cs
// Purpose: On-demand in-world runtime counts for personal vehicles.
// Notes:
// - Base filter uses Game.Vehicles.PersonalCar (includes bicycles + e-scooters).
// - Bicycle-group gate uses Game.Prefabs.BicycleData on the prefab entity.
// - In-world definition:
//   - Parked = Game.Vehicles.ParkedCar
//   - Active = Game.Vehicles.CarCurrentLane
// - Total = Parked || Active (excludes registry-only/unspawned vehicles).

namespace FastBikes
{
    using Game;                // GameSystemBase
    using Game.Common;         // Deleted
    using Game.Prefabs;        // BicycleData, PrefabBase, PrefabRef, PrefabSystem
    using Game.Tools;          // Temp
    using Game.Vehicles;       // CarCurrentLane, ParkedCar, PersonalCar
    using System;              // DateTime, StringComparison
    using Unity.Collections;   // Allocator, NativeArray
    using Unity.Entities;      // Entity, EntityQuery, ComponentLookup, ComponentType

    public sealed partial class FastBikeStatusSystem : GameSystemBase
    {
        public readonly struct Snapshot
        {
            public readonly long BikeGroupTotal;      // In-world bikes + e-scooters
            public readonly long BikeGroupParked;     // ParkedCar subset
            public readonly long BikeGroupActive;     // CarCurrentLane subset
            public readonly long ScooterTotal;        // ElectricScooter* subset
            public readonly long BikeOnlyTotal;       // BikeGroupTotal - ScooterTotal

            public readonly long CarGroupTotal;       // In-world PersonalCar NOT in bike group
            public readonly long CarGroupParked;      // ParkedCar subset
            public readonly long CarGroupActive;      // CarCurrentLane subset

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

                SnapshotTimeLocal = snapshotTimeLocal;
            }
        }

        private PrefabSystem m_PrefabSystem = null!;
        private EntityQuery m_PersonalVehicleQuery;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            m_PersonalVehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

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

            long bikeGroupTotal = 0;
            long bikeGroupParked = 0;
            long bikeGroupActive = 0;
            long scooterTotal = 0;

            long carGroupTotal = 0;
            long carGroupParked = 0;
            long carGroupActive = 0;

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

                        if (isActive)
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

                    if (isActive)
                    {
                        carGroupActive++;
                    }
                }
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
                snapshotTimeLocal: DateTime.Now);
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
