// File: Systems/FastBikeStatusSystem.cs
// Purpose: On-demand runtime counts for personal vehicles.
// Notes:
// - Runtime filter uses Game.Vehicles.PersonalCar (includes bicycles).
// - Bicycle-group gate uses Game.Prefabs.BicycleData on the prefab entity.
// - Parked detection uses Game.Vehicles.ParkedCar (true parked, not “stopped at light”).

namespace FastBikes
{
    using Game;                // GameSystemBase
    using Game.Common;         // Deleted
    using Game.Prefabs;        // BicycleData, PrefabBase, PrefabRef, PrefabSystem
    using Game.Tools;          // Temp
    using Game.Vehicles;       // ParkedCar, PersonalCar
    using System;              // DateTime, StringComparison
    using Unity.Collections;   // Allocator, NativeArray
    using Unity.Entities;      // Entity, EntityQuery, ComponentType

    public sealed partial class FastBikeStatusSystem : GameSystemBase
    {
        public readonly struct Snapshot
        {
            public readonly long TotalPersonalVehicles;

            public readonly long BikeGroupTotal;     // BicycleData-gated (bikes + electric scooters)
            public readonly long BikeGroupParked;    // ParkedCar subset of BikeGroupTotal
            public readonly long ScooterTotal;       // Name-based subset (ElectricScooter*)
            public readonly long BikeOnlyTotal;      // BikeGroupTotal - ScooterTotal

            public readonly long CarGroupTotal;      // PersonalCar NOT in bike group
            public readonly long CarGroupParked;     // ParkedCar subset of CarGroupTotal
            public readonly long CarGroupRunning;    // CarGroupTotal - CarGroupParked

            public readonly DateTime SnapshotTimeLocal;

            public Snapshot(
                long totalPersonalVehicles,
                long bikeGroupTotal,
                long bikeGroupParked,
                long scooterTotal,
                long bikeOnlyTotal,
                long carGroupTotal,
                long carGroupParked,
                long carGroupRunning,
                DateTime snapshotTimeLocal)
            {
                TotalPersonalVehicles = totalPersonalVehicles;

                BikeGroupTotal = bikeGroupTotal;
                BikeGroupParked = bikeGroupParked;
                ScooterTotal = scooterTotal;
                BikeOnlyTotal = bikeOnlyTotal;

                CarGroupTotal = carGroupTotal;
                CarGroupParked = carGroupParked;
                CarGroupRunning = carGroupRunning;

                SnapshotTimeLocal = snapshotTimeLocal;
            }
        }

        private PrefabSystem m_PrefabSystem = null!;
        private EntityQuery m_PersonalVehicleQuery;

        protected override void OnCreate( )
        {
            base.OnCreate();

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Runtime personal vehicles (includes bikes and scooters).
            // PrefabRef is used to reach the prefab entity for BicycleData gating.
            m_PersonalVehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.Exclude<Deleted>(),
                ComponentType.Exclude<Temp>());

            // No per-frame work.
            Enabled = false;
        }

        protected override void OnUpdate( )
        {
            // Intentionally empty.
        }

        public Snapshot BuildSnapshot( )
        {
            ComponentLookup<PrefabRef> prefabRefLookup = GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<BicycleData> bicycleDataLookup = GetComponentLookup<BicycleData>(isReadOnly: true);
            ComponentLookup<Game.Vehicles.ParkedCar> parkedLookup = GetComponentLookup<ParkedCar>(isReadOnly: true);

            long totalPersonal = 0;

            long bikeGroupTotal = 0;
            long bikeGroupParked = 0;
            long scooterTotal = 0;

            long carGroupTotal = 0;
            long carGroupParked = 0;

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

                    totalPersonal++;

                    bool isParked = parkedLookup.HasComponent(e);

                    // Bicycle group gate: BicycleData exists on BicyclePrefab-derived prefabs.
                    bool isBikeGroup = bicycleDataLookup.HasComponent(prefabEntity);
                    if (isBikeGroup)
                    {
                        bikeGroupTotal++;

                        if (isParked)
                        {
                            bikeGroupParked++;
                        }

                        if (IsScooterPrefab(prefabEntity))
                        {
                            scooterTotal++;
                        }

                        continue;
                    }

                    // Non-bike personal vehicles (cars, motorcycles, SUVs, etc).
                    carGroupTotal++;

                    if (isParked)
                    {
                        carGroupParked++;
                    }
                }
            }

            long bikeOnlyTotal = bikeGroupTotal - scooterTotal;
            if (bikeOnlyTotal < 0)
            {
                bikeOnlyTotal = 0;
            }

            long carGroupRunning = carGroupTotal - carGroupParked;
            if (carGroupRunning < 0)
            {
                carGroupRunning = 0;
            }

            return new Snapshot(
                totalPersonalVehicles: totalPersonal,
                bikeGroupTotal: bikeGroupTotal,
                bikeGroupParked: bikeGroupParked,
                scooterTotal: scooterTotal,
                bikeOnlyTotal: bikeOnlyTotal,
                carGroupTotal: carGroupTotal,
                carGroupParked: carGroupParked,
                carGroupRunning: carGroupRunning,
                snapshotTimeLocal: DateTime.Now);
        }

        private bool IsScooterPrefab(Entity prefabEntity)
        {
            // PrefabBase.name via PrefabSystem.TryGetPrefab follows the Game.Prefabs source of truth.
            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
            {
                return false;
            }

            string n = prefabBase.name;
            if (string.IsNullOrEmpty(n))
            {
                return false;
            }

            return n.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase)
                || n.StartsWith("Scooter", StringComparison.OrdinalIgnoreCase);
        }
    }
}
