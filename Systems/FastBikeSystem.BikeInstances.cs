// File: Systems/FastBikeSystem.BikeInstances.cs
// Purpose: Dump bicycle-group instance counts and car-group runtime classification (incl. OC-hidden split).
// Notes: Read-only.

namespace FastBikes
{
    using Game.Common;            // Deleted, Destroyed, Overridden, Owner
    using Game.Net;                  // OutsideConnection, ConnectionLane
    using Game.Objects;           // Moving, Stopped, Unspawned
    using Game.Prefabs;           // BicycleData
    using Game.Tools;             // Temp
    using Game.Vehicles;          // Car, CarCurrentLane, ParkedCar
    using System.Collections.Generic;
    using System.Text;
    using Unity.Collections;
    using Unity.Entities;

    public sealed partial class FastBikeSystem
    {
        private const int kSampleMax = 10;

        private void DumpCarGroupInstancesReport( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] CAR GROUP INSTANCES (LIVE) ====================\n" +
                "Meaning: PersonalCar instances excluding BicycleData prefabs.\n" +
                "Status classification:\n" +
                "  Parked  = ParkedCar\n" +
                "  Active  = CarCurrentLane (Parked wins)\n" +
                "  Pending = neither ParkedCar nor CarCurrentLane (diagnostic only)\n" +
                "Hidden types (subset of Parked):\n" +
                "  Hidden in buildings = ParkedCar + Unspawned + Owner + NOT OC lane\n" +
                "  Hidden at border OC = ParkedCar + Unspawned + Owner + OC lane\n" +
                "Notes:\n" +
                "  - Trailers (CarTrailer) are excluded from car-group counts and logged separately.\n");

            // -----------------------------------------------------
            // Build bicycle-group prefab set (BicycleData on prefab).
            // -----------------------------------------------------
            HashSet<Entity> bikeGroupPrefabs = new HashSet<Entity>();

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                bikeGroupPrefabs.Add(prefabEntity);
            }

            // -----------------------------------------------------------
            // Trailer totals (separate; trailers are not drivable "cars").
            // -----------------------------------------------------------
            int trailerTotal = 0;
            int trailerUnspawned = 0;

            EntityQuery trailerQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Vehicles.PersonalCar, Game.Vehicles.CarTrailer, PrefabRef>()
                .WithNone<Deleted, Temp, Destroyed>()
                .Build();

            using (NativeArray<Entity> trailerEntities = trailerQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < trailerEntities.Length; i++)
                {
                    Entity e = trailerEntities[i];
                    trailerTotal++;

                    if (SystemAPI.HasComponent<Unspawned>(e))
                    {
                        trailerUnspawned++;
                    }
                }
            }

            // --------------------------------------------------------
            // Car-group totals (exclude bike group + exclude trailers).
            // --------------------------------------------------------
            int allLiveTotal = 0;
            int parked = 0;
            int active = 0;
            int pending = 0;

            int statusTotal = 0; // parked + active only

            int unspawnedTotal = 0;
            int unspawnedWithOwner = 0;

            int parkedUnspawnedTotal = 0;
            int hiddenInBuildings = 0;
            int hiddenAtBorderOc = 0;

            List<Entity> hiddenAtBorderHead = new List<Entity>(kSampleMax);
            List<Entity> hiddenAtBorderTail = new List<Entity>(kSampleMax);

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity e) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.PersonalCar>()
                .WithNone<Game.Vehicles.CarTrailer>()
                .WithNone<Deleted, Temp, Destroyed>()
                .WithEntityAccess())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                // Exclude bicycles + e-scooters (bike row owns those).
                if (bikeGroupPrefabs.Contains(prefabEntity))
                {
                    continue;
                }

                bool hasParked = SystemAPI.HasComponent<ParkedCar>(e);
                bool hasLane = SystemAPI.HasComponent<CarCurrentLane>(e);

                bool hasUnspawned = SystemAPI.HasComponent<Unspawned>(e);

                bool hasOwner = false;
                if (SystemAPI.HasComponent<Owner>(e))
                {
                    Owner o = SystemAPI.GetComponent<Owner>(e);
                    hasOwner = o.m_Owner != Entity.Null;
                }

                bool isParked = hasParked;
                bool isActive = !isParked && hasLane;
                bool isPending = !isParked && !isActive;

                allLiveTotal++;

                if (isParked)
                {
                    parked++;
                }
                else if (isActive)
                {
                    active++;
                }
                else
                {
                    pending++;
                }

                if (!isPending)
                {
                    statusTotal++;
                }

                if (hasUnspawned)
                {
                    unspawnedTotal++;

                    if (hasOwner)
                    {
                        unspawnedWithOwner++;
                    }
                }

                // Hidden parked classification
                if (isParked && hasUnspawned && hasOwner)
                {
                    parkedUnspawnedTotal++;

                    ParkedCar pc = SystemAPI.GetComponent<ParkedCar>(e);
                    Entity lane = pc.m_Lane;

                    bool isOcLane = IsOutsideConnectionLane(lane);

                    if (isOcLane)
                    {
                        hiddenAtBorderOc++;
                        AddHeadTailSample(e, hiddenAtBorderHead, hiddenAtBorderTail, kSampleMax, kSampleMax);
                    }
                    else
                    {
                        hiddenInBuildings++;
                    }
                }
            }

            Mod.LogSafe(( ) =>
            {
                StringBuilder sb = new System.Text.StringBuilder();

                sb.AppendLine($"[FB] Trailers: Total={trailerTotal}, Unspawned={trailerUnspawned}");
                sb.AppendLine($"[FB] CarGroup AllLive: Total={allLiveTotal}, Parked={parked}, Active={active}, Pending={pending}");
                sb.AppendLine($"[FB] CarGroup StatusTotal(Parked+Active)={statusTotal}");

                sb.AppendLine($"[FB] Unspawned: Total={unspawnedTotal}, WithOwner={unspawnedWithOwner}");

                sb.AppendLine($"[FB] Parked&&Unspawned&&Owner: Total={parkedUnspawnedTotal}");
                sb.AppendLine($"[FB] Hidden split: Buildings={hiddenInBuildings}, BorderOC={hiddenAtBorderOc}");

                sb.Append("[FB] BorderOC samples Head (Index:Version): ");
                AppendEntitySamples(sb, hiddenAtBorderHead);
                sb.AppendLine();

                sb.Append("[FB] BorderOC samples Tail (Index:Version): ");
                AppendEntitySamples(sb, hiddenAtBorderTail);
                sb.AppendLine();

                return sb.ToString();
            });
        }

        private void DumpBikeInstancesReport( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] BIKE INSTANCES (LIVE) ====================\n" +
                "Meaning: live BicycleData vehicles (bikes + e-scooters).\n" +
                "Live excludes: Deleted, Temp, Destroyed.\n" +
                "Classification:\n" +
                "  Parked = ParkedCar\n" +
                "  Active = CarCurrentLane (Parked wins if both)\n" +
                "  Other  = neither ParkedCar nor CarCurrentLane\n");

            HashSet<Entity> groupPrefabs = new HashSet<Entity>();
            HashSet<Entity> scooterPrefabs = new HashSet<Entity>();

            int groupPrefabCount = 0;
            int missingPrefabBase = 0;

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                groupPrefabCount++;
                groupPrefabs.Add(prefabEntity);

                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    missingPrefabBase++;
                    continue;
                }

                string name = prefabBase.name ?? string.Empty;
                if (IsScooterName(name))
                {
                    scooterPrefabs.Add(prefabEntity);
                }
            }

            if (groupPrefabs.Count == 0)
            {
                Mod.LogSafe(( ) => "[FB] Bike instances: BicycleData prefab set is empty.");
                return;
            }

            int total = 0;
            int active = 0;
            int parked = 0;
            int other = 0;

            int bikesTotal = 0;
            int scootersTotal = 0;

            int bikesActive = 0;
            int bikesParked = 0;

            int scootersActive = 0;
            int scootersParked = 0;

            int unspawnedTotal = 0;
            int parkedUnspawned = 0;
            int activeUnspawned = 0;

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity e) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Car>()
                .WithNone<Deleted, Temp, Destroyed>()
                .WithEntityAccess())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!groupPrefabs.Contains(prefabEntity))
                {
                    continue;
                }

                bool isScooter = scooterPrefabs.Contains(prefabEntity);

                bool hasParked = SystemAPI.HasComponent<ParkedCar>(e);
                bool hasLane = SystemAPI.HasComponent<CarCurrentLane>(e);
                bool hasUnspawned = SystemAPI.HasComponent<Unspawned>(e);

                bool isParked = hasParked;
                bool isActive = !isParked && hasLane;

                total++;

                if (isScooter)
                {
                    scootersTotal++;
                }
                else
                {
                    bikesTotal++;
                }

                if (isParked)
                {
                    parked++;

                    if (isScooter)
                    {
                        scootersParked++;
                    }
                    else
                    {
                        bikesParked++;
                    }
                }
                else if (isActive)
                {
                    active++;

                    if (isScooter)
                    {
                        scootersActive++;
                    }
                    else
                    {
                        bikesActive++;
                    }
                }
                else
                {
                    other++;
                }

                if (hasUnspawned)
                {
                    unspawnedTotal++;

                    if (isParked)
                    {
                        parkedUnspawned++;
                    }
                    else if (isActive)
                    {
                        activeUnspawned++;
                    }
                }
            }

            Mod.LogSafe(( ) =>
            {
                StringBuilder sb = new System.Text.StringBuilder();

                sb.AppendLine($"[FB] Bicycle-group prefabs: Count={groupPrefabCount}, MissingPrefabBase={missingPrefabBase}");
                sb.AppendLine($"[FB] BikeGroup: Total={total}, Active={active}, Parked={parked}, Other={other}");
                sb.AppendLine($"[FB] Split: Bikes={bikesTotal} (A={bikesActive}, P={bikesParked}), Scooters={scootersTotal} (A={scootersActive}, P={scootersParked})");
                sb.AppendLine($"[FB] Unspawned: Total={unspawnedTotal}, Parked={parkedUnspawned}, Active={activeUnspawned}");
                return sb.ToString();
            });
        }

        private bool IsOutsideConnectionLane(Entity laneEntity)
        {
            if (laneEntity == Entity.Null)
            {
                return false;
            }

            // Direct marker seen in SE screenshots.
            if (SystemAPI.HasComponent<Game.Net.OutsideConnection>(laneEntity))
            {
                return true;
            }

            // Lane itself often has ConnectionLane component.
            if (!SystemAPI.HasComponent<Game.Net.ConnectionLane>(laneEntity))
            {
                return false;
            }

            // Owner-chain probe (limited) for OutsideConnection marker.
            Entity cur = laneEntity;
            for (int i = 0; i < 6; i++)
            {
                if (SystemAPI.HasComponent<Game.Net.OutsideConnection>(cur))
                {
                    return true;
                }

                if (!SystemAPI.HasComponent<Owner>(cur))
                {
                    break;
                }

                Owner o = SystemAPI.GetComponent<Owner>(cur);
                if (o.m_Owner == Entity.Null)
                {
                    break;
                }

                cur = o.m_Owner;
            }

            return false;
        }

        private static void AddHeadTailSample(Entity e, List<Entity> head, List<Entity> tail, int headMax, int tailMax)
        {
            if (head.Count < headMax)
            {
                head.Add(e);
            }

            if (tail.Count == tailMax)
            {
                tail.RemoveAt(0);
            }

            tail.Add(e);
        }

        private static void AppendEntitySamples(System.Text.StringBuilder sb, List<Entity> items)
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
}
