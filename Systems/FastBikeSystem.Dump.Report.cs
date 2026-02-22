// File: Systems/FastBikeSystem.Dump.Reports.cs
// Purpose: Dump sub-reports (Path speeds, Scooter01 instances, citizen eligibility stats).

namespace FastBikes
{
    using Game.Citizens;              // BicycleOwner, CarKeeper, Citizen
    using Game.Common;                // Deleted, Overridden
    using Game.Prefabs;               // PathwayPrefab, PathwayData, PathwayComposition, NetCompositionData, RoadData, PrefabBase, PrefabData, PrefabRef
    using Game.Tools;                 // Temp
    using Game.Vehicles;              // Car, ParkedCar
    using System.Collections.Generic; // List
    using Unity.Collections;          // Allocator, NativeArray
    using Unity.Entities;             // Entity, EntityQuery, SystemAPI, RefRO
    using Unity.Mathematics;          // math

    public sealed partial class FastBikeSystem
    {
        private void DumpCitizenVehicleEligibilityReport( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] CITIZEN VEHICLE ELIGIBILITY ====================\n" +
                "Meaning: counts citizens and whether BicycleOwner / CarKeeper components are present+enabled.\n" +
                "This does NOT measure bike usage; it only shows eligibility flags.\n");

            const int kMaxSamples = 10;

            int citizenTotal = 0;

            int bicycleOwnerPresent = 0;
            int bicycleOwnerEnabled = 0;
            int bicycleOwnerEnabledNullBike = 0;

            int carKeeperPresent = 0;
            int carKeeperEnabled = 0;

            int bothPresent = 0;
            int bothEnabled = 0;

            var sampleBikeEnabled = new List<Entity>(kMaxSamples);
            var sampleCarEnabled = new List<Entity>(kMaxSamples);
            var sampleBothEnabled = new List<Entity>(kMaxSamples);

            EntityQuery q = SystemAPI.QueryBuilder()
                .WithAll<Game.Citizens.Citizen>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Destroyed>()
                .Build();

            using (NativeArray<Entity> citizens = q.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < citizens.Length; i++)
                {
                    Entity c = citizens[i];
                    citizenTotal++;

                    bool hasBikeOwner = EntityManager.HasComponent<Game.Citizens.BicycleOwner>(c);
                    bool hasCarKeeper = EntityManager.HasComponent<Game.Citizens.CarKeeper>(c);

                    if (hasBikeOwner)
                    {
                        bicycleOwnerPresent++;

                        if (EntityManager.IsComponentEnabled<Game.Citizens.BicycleOwner>(c))
                        {
                            bicycleOwnerEnabled++;

                            Game.Citizens.BicycleOwner bo = EntityManager.GetComponentData<Game.Citizens.BicycleOwner>(c);
                            if (bo.m_Bicycle == Entity.Null)
                            {
                                bicycleOwnerEnabledNullBike++;
                            }

                            if (sampleBikeEnabled.Count < kMaxSamples)
                            {
                                sampleBikeEnabled.Add(c);
                            }
                        }
                    }

                    if (hasCarKeeper)
                    {
                        carKeeperPresent++;

                        if (EntityManager.IsComponentEnabled<Game.Citizens.CarKeeper>(c))
                        {
                            carKeeperEnabled++;

                            if (sampleCarEnabled.Count < kMaxSamples)
                            {
                                sampleCarEnabled.Add(c);
                            }
                        }
                    }

                    if (hasBikeOwner && hasCarKeeper)
                    {
                        bothPresent++;

                        bool bikeEnabledNow = EntityManager.IsComponentEnabled<Game.Citizens.BicycleOwner>(c);
                        bool carEnabledNow = EntityManager.IsComponentEnabled<Game.Citizens.CarKeeper>(c);

                        if (bikeEnabledNow && carEnabledNow)
                        {
                            bothEnabled++;

                            if (sampleBothEnabled.Count < kMaxSamples)
                            {
                                sampleBothEnabled.Add(c);
                            }
                        }
                    }
                }
            }

            Mod.LogSafe(( ) =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[FB] Citizens: Total={citizenTotal}");

                sb.AppendLine($"[FB] BicycleOwner: Present={bicycleOwnerPresent}, Enabled={bicycleOwnerEnabled}, EnabledWithNullBike={bicycleOwnerEnabledNullBike}");
                sb.AppendLine($"[FB] CarKeeper: Present={carKeeperPresent}, Enabled={carKeeperEnabled}");

                sb.AppendLine($"[FB] Both components: Present={bothPresent}, BothEnabled={bothEnabled}");

                sb.Append("[FB] Sample BicycleOwner Enabled (Citizen Index:Version): ");
                AppendEntitySamples(sb, sampleBikeEnabled);
                sb.AppendLine();

                sb.Append("[FB] Sample CarKeeper Enabled (Citizen Index:Version): ");
                AppendEntitySamples(sb, sampleCarEnabled);
                sb.AppendLine();

                sb.Append("[FB] Sample BothEnabled (Citizen Index:Version): ");
                AppendEntitySamples(sb, sampleBothEnabled);
                sb.AppendLine();

                return sb.ToString();
            });

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

        private void DumpScooter01Report( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] SCOOTER01 (FUEL) ====================\n" +
                "Usage: locate Scooter01 prefab by name, count live instances, log up to 10 samples for SE Mod.");

            const int kMaxSamples = 10;
            Entity scooterPrefabEntity = Entity.Null;

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>()
                .WithEntityAccess())
            {
                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    continue;
                }

                if (string.Equals(prefabBase.name, "Scooter01", System.StringComparison.OrdinalIgnoreCase))
                {
                    scooterPrefabEntity = prefabEntity;
                    break;
                }
            }

            if (scooterPrefabEntity == Entity.Null)
            {
                Mod.LogSafe(( ) => "[FB] Scooter01 prefab not found (PrefabSystem).");
                return;
            }

            int total = 0;
            int parked = 0;
            int active = 0;
            int other = 0;

            var samples = new List<Entity>(kMaxSamples);

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity vehicleEntity) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Car>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .WithEntityAccess())
            {
                if (prefabRefRO.ValueRO.m_Prefab != scooterPrefabEntity)
                {
                    continue;
                }

                total++;

                bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicleEntity);
                bool isActive = SystemAPI.HasComponent<Game.Vehicles.CarCurrentLane>(vehicleEntity);

                if (isParked)
                    parked++;
                else if (isActive)
                    active++;
                else
                    other++;

                if (samples.Count < kMaxSamples)
                {
                    samples.Add(vehicleEntity);
                }
            }

            Mod.LogSafe(( ) =>
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"[FB] Scooter01 prefab entity={FormatIndexVersion(scooterPrefabEntity)}");
                sb.AppendLine($"[FB] Scooter01 instances: Total={total}, Active={active}, Parked={parked}, Other={other}");

                sb.Append("[FB] Scooter01 sample Index:Version = ");
                if (samples.Count == 0)
                {
                    sb.AppendLine("<none>");
                }
                else
                {
                    for (int i = 0; i < samples.Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(FormatIndexVersion(samples[i]));
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            });
        }

        private void DumpPathSpeedReport(float pathScalar)
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] PATH SPEED SUMMARY ====================\n" +
                "Meaning: PathwayData speed limit tracks authoring * scalar.\n" +
                $"Path Scalar={pathScalar:0.##}");

            int prefabs = 0;
            int prefabMissingBase = 0;
            int prefabNotPathwayPrefab = 0;
            int prefabInvalidAuthoring = 0;
            int prefabMismatch = 0;

            int prefabAlsoHasRoadData = 0;

            float prefabMin = float.PositiveInfinity;
            float prefabMax = float.NegativeInfinity;

#if DEBUG
            int prefabMismatchExamples = 0;
#endif

            foreach ((RefRO<PathwayData> pathRO, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PathwayData>>()
                .WithAll<Game.Prefabs.PrefabData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>()
                .WithEntityAccess())
            {
                prefabs++;

                float currentMs = pathRO.ValueRO.m_SpeedLimit;

                prefabMin = Unity.Mathematics.math.min(prefabMin, currentMs);
                prefabMax = Unity.Mathematics.math.max(prefabMax, currentMs);

                if (SystemAPI.HasComponent<RoadData>(prefabEntity))
                {
                    prefabAlsoHasRoadData++;
                }

                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    prefabMissingBase++;
                    continue;
                }

                if (prefabBase is not PathwayPrefab pathPrefab)
                {
                    prefabNotPathwayPrefab++;
                    continue;
                }

                if (pathPrefab.m_SpeedLimit <= 0f)
                {
                    prefabInvalidAuthoring++;
                    continue;
                }

                float expectedMs = KmhToMs(pathPrefab.m_SpeedLimit) * Unity.Mathematics.math.max(0.01f, pathScalar);

                if (RelativeDiff(expectedMs, currentMs) > kPathMismatchPct)
                {
                    prefabMismatch++;

#if DEBUG
                    if (prefabMismatchExamples < 3)
                    {
                        prefabMismatchExamples++;

                        Mod.LogSafe(( ) =>
                            "-------------------- [FB] PATH PREFAB MISMATCH (DEBUG) --------------------\n" +
                            $"Name='{prefabBase.name}'\n" +
                            $"Authoring={pathPrefab.m_SpeedLimit:0.###} km/h\n" +
                            $"Expected={MsToKmh(expectedMs):0.###} km/h\n" +
                            $"Current ={MsToKmh(currentMs):0.###} km/h\n" +
                            $"Diff    ={FormatPct(RelativeDiff(expectedMs, currentMs))}");
                    }
#endif
                }
            }

            Mod.LogSafe(( ) =>
                "\n-------------------- [FB] PATH PREFABS --------------------\n" +
                $"Count={prefabs}\n" +
                $"SpeedMin={MsToKmh(prefabMin):0.###} km/h, SpeedMax={MsToKmh(prefabMax):0.###} km/h\n" +
                $"MissingPrefabBase={prefabMissingBase}, NotPathwayPrefab={prefabNotPathwayPrefab}, InvalidAuthoring={prefabInvalidAuthoring}\n" +
                $"Mismatch>{kPathMismatchPct * 100f:0.##}%={prefabMismatch}\n" +
                $"ContaminationCheck: PathwayDataPrefabsAlsoHaveRoadData={prefabAlsoHasRoadData}");

            int comps = 0;
            int compMissingPathwayData = 0;
            int compMismatch = 0;

            float compMin = float.PositiveInfinity;
            float compMax = float.NegativeInfinity;

            foreach ((RefRO<PathwayComposition> compRO, RefRO<PrefabRef> prefabRefRO) in SystemAPI
                .Query<RefRO<PathwayComposition>, RefRO<PrefabRef>>()
                .WithAll<NetCompositionData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>())
            {
                comps++;

                float compMs = compRO.ValueRO.m_SpeedLimit;

                compMin = Unity.Mathematics.math.min(compMin, compMs);
                compMax = Unity.Mathematics.math.max(compMax, compMs);

                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!SystemAPI.HasComponent<PathwayData>(prefabEntity))
                {
                    compMissingPathwayData++;
                    continue;
                }

                float desiredMs = SystemAPI.GetComponent<PathwayData>(prefabEntity).m_SpeedLimit;

                if (Unity.Mathematics.math.abs(compMs - desiredMs) > kCompAbsMismatchMs)
                {
                    compMismatch++;
                }
            }

            Mod.LogSafe(( ) =>
                "\n-------------------- [FB] PATH COMPOSITIONS --------------------\n" +
                $"Count={comps}\n" +
                $"SpeedMin={MsToKmh(compMin):0.###} km/h, SpeedMax={MsToKmh(compMax):0.###} km/h\n" +
                $"MissingPathwayDataOnPrefabRef={compMissingPathwayData}\n" +
                $"MismatchAbs>{kCompAbsMismatchMs:0.###} m/s={compMismatch}");
        }
    }
}
