// File: Systems/FastBikeStatusSystem.cs
// Purpose: On-demand in-world runtime counts for personal vehicles.
// Notes:
// - Snapshot built on-demand (Options UI), not from OnUpdate.
// - EntityQuery + ComponentLookup used to avoid per-frame work.
// - Base filter uses Game.Vehicles.PersonalCar (includes bicycles + e-scooters).
// - Bicycle-group gate uses Game.Prefabs.BicycleData on the prefab entity.
// - In-world definition:
//   - Parked = Game.Vehicles.ParkedCar
//   - Active = Game.Vehicles.CarCurrentLane (Parked wins if both)
// - Total shown in UI = Parked + Active (pending excluded by design).
// - Trailers are excluded (Game.Vehicles.CarTrailer) so car totals reflect drivable cars.

namespace FastBikes
{
    using Game;                // GameSystemBase
    using Game.Common;         // Deleted, Destroyed
    using Game.Prefabs;        // BicycleData, PrefabBase, PrefabRef, PrefabSystem
    using Game.Tools;          // Temp
    using Game.Vehicles;       // CarCurrentLane, CarTrailer, ParkedCar
    using System;              // DateTime, StringComparison
    using Unity.Collections;   // Allocator, NativeArray
    using Unity.Entities;      // Entity, EntityQuery, ComponentLookup, ComponentType

    public sealed partial class FastBikeStatusSystem : GameSystemBase
    {
        public readonly struct Snapshot
        {
            public readonly long BikeGroupTotal;      // Parked+Active bikes + e-scooters
            public readonly long BikeGroupParked;
            public readonly long BikeGroupActive;
            public readonly long ScooterTotal;        // ElectricScooter* subset
            public readonly long BikeOnlyTotal;       // BikeGroupTotal - ScooterTotal

            public readonly long CarGroupTotal;       // Parked+Active personal cars (excludes bike group, excludes trailers)
            public readonly long CarGroupParked;
            public readonly long CarGroupActive;

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

            // Trailers carry PersonalCar but are not a drivable "car" for status purposes.
            m_PersonalVehicleQuery = GetEntityQuery(
                ComponentType.ReadOnly<PrefabRef>(),
                ComponentType.ReadOnly<Game.Vehicles.PersonalCar>(),
                ComponentType.Exclude<CarTrailer>(),
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
                        continue;

                    Entity prefabEntity = prefabRefLookup[e].m_Prefab;
                    if (prefabEntity == Entity.Null)
                        continue;

                    bool isParked = parkedLookup.HasComponent(e);

                    // Parked wins to keep groups mutually exclusive.
                    bool isActive = !isParked && currentLaneLookup.HasComponent(e);

                    // Status UI totals intentionally exclude transitional/pending entities.
                    if (!isParked && !isActive)
                        continue;

                    bool isBikeGroup = bicycleDataLookup.HasComponent(prefabEntity);

                    if (isBikeGroup)
                    {
                        bikeGroupTotal++;

                        if (isParked)
                            bikeGroupParked++;
                        else if (isActive)
                            bikeGroupActive++;

                        if (IsElectricScooterPrefab(prefabEntity))
                            scooterTotal++;

                        continue;
                    }

                    carGroupTotal++;

                    if (isParked)
                        carGroupParked++;
                    else if (isActive)
                        carGroupActive++;
                }
            }

            long bikeOnlyTotal = bikeGroupTotal - scooterTotal;
            if (bikeOnlyTotal < 0)
                bikeOnlyTotal = 0;

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
                return false;

            string n = prefabBase.name;
            if (string.IsNullOrEmpty(n))
                return false;

            return n.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase);
        }
    }
}
