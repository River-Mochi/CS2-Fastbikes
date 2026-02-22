// File: Systems/FastBikeSystem.Dump.cs
// Purpose: Dump bicycle/scooter prefab values (authoring + current prefab-entity data) and path prefab/composition speed limits.
// Notes:
// - Dump is read-only.
// - No runtime lane (Game.Net.*) scans are performed here; runtime verification remains via SE mod.
// - Output is summary-first and logs per-item only when mismatches are detected.

namespace FastBikes
{
    using Game.Common;        // Deleted, Overridden
    using Game.Prefabs;       // PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData, SwayingData, PathwayPrefab, PathwayData, PathwayComposition, PrefabRef, NetCompositionData, RoadData
    using Game.Tools;         // Temp
    using System;             // StringComparison
    using Unity.Entities;     // Entity, RefRO, SystemAPI
    using Unity.Mathematics;  // math.*

    public sealed partial class FastBikeSystem
    {
        private bool m_Dump;

        // Mismatch thresholds.
        private const float kSpeedMismatchPct = 0.05f;   // 5%
        private const float kAccelMismatchPct = 0.10f;   // 10%
        private const float kBrakeMismatchPct = 0.10f;   // 10%

        private const float kSwayMaxPosMismatchPct = 0.10f;   // 10%
        private const float kSwayDampingMismatchPct = 0.15f;  // 15%

        private const float kPathMismatchPct = 0.02f;         // 2%
        private const float kCompAbsMismatchMs = 0.10f;       // ~0.36 km/h

        // Limits log spam; summary still reports totals.
        private const int kMaxMismatchExamples = 12;

        public void ScheduleDump( )
        {
            m_Dump = true;
            Enabled = true;
        }

        private static float MsToKmh(float ms) => ms * 3.6f;
        private static float MsToMph(float ms) => ms * 2.23693629f;

        private static float KmhToMs(float kmh) => kmh * (1f / 3.6f);
        private static float KmhToMph(float kmh) => kmh * 0.621371f;

        private static string F3(float3 v) => $"({v.x:0.###},{v.y:0.###},{v.z:0.###})";

        private static bool IsScooterName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            return name.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase) ||
                   name.StartsWith("Scooter", StringComparison.OrdinalIgnoreCase);
        }

        private static float RelativeDiff(float expected, float actual)
        {
            float denom = math.max(1e-6f, math.abs(expected));
            return math.abs(actual - expected) / denom;
        }

        private static float RelativeDiffMax(float3 expected, float3 actual)
        {
            float3 denom = math.max(new float3(1e-6f), math.abs(expected));
            float3 rel = math.abs(actual - expected) / denom;
            return math.cmax(rel);
        }

        private static string FormatPct(float value01) => (value01 * 100f).ToString("0.##") + "%";

        private static bool TryGetSettings(out Setting settings)
        {
            if (Mod.Settings is Setting s)
            {
                settings = s;
                return true;
            }

            settings = default!;
            Mod.WarnOnce("FB_SETTINGS_NULL", ( ) => "[FB] Settings is null; Dump using PathSpeed=1x.");
            return false;
        }

        private void DumpBicyclePrefabs(bool enableFastBikes, float speedScalar, float stiffnessScalar, float dampingScalar)
        {
            float effectiveSpeed = enableFastBikes ? math.clamp(speedScalar, 0.30f, 10.0f) : 1.0f;
            float effectiveStiff = enableFastBikes ? math.clamp(stiffnessScalar, 0.30f, 5.0f) : 1.0f;
            float effectiveDamp = enableFastBikes ? math.clamp(dampingScalar, 0.30f, 5.0f) : 1.0f;

            float effectiveAccelBrake = math.sqrt(math.max(0.01f, effectiveSpeed));

            float rawPathSpeed = -1f;
            float pathScalar = 1.0f;

            if (enableFastBikes && TryGetSettings(out Setting settings))
            {
                rawPathSpeed = settings.PathSpeedScalar;
                pathScalar = math.clamp(rawPathSpeed, 1.0f, 5.0f);
            }

            Mod.LogSafe(( ) =>
                "==================== [FB] DUMP SUMMARY ====================\n" +
                $"EnableFastBikes={enableFastBikes}\n" +
                $"BikeSpeedScalar={speedScalar:0.##} (Effective={effectiveSpeed:0.##})\n" +
                $"StiffnessScalar={stiffnessScalar:0.##} (Effective={effectiveStiff:0.##})\n" +
                $"DampingScalar={dampingScalar:0.##} (Effective={effectiveDamp:0.##})\n" +
                $"PathSpeedScalar={rawPathSpeed:0.##} (Effective={pathScalar:0.##})\n" +
                $"PathLaneBatchActive={IsPathBatchActive()}" +
                (IsPathBatchActive() ? $" (EdgesRemaining={m_PathEdgeEntities.Length - m_PathEdgeIndex}/{m_PathEdgeEntities.Length})" : string.Empty));

            int total = 0;
            int bikes = 0;
            int scooters = 0;

            int missingPrefabBase = 0;
            int missingCarData = 0;
            int missingSwaying = 0;

            int mismatchAny = 0;
            int mismatchSpeed = 0;
            int mismatchAccel = 0;
            int mismatchBrake = 0;
            int mismatchSwayPos = 0;
            int mismatchSwayDamp = 0;

            int examples = 0;

            foreach ((Unity.Entities.RefRO<Game.Prefabs.PrefabData> _, Unity.Entities.Entity prefabEntity) in SystemAPI
                .Query<Unity.Entities.RefRO<Game.Prefabs.PrefabData>>()
                .WithAll<Game.Prefabs.BicycleData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>()
                .WithEntityAccess())
            {
                total++;

                string name = "(PrefabBase not found)";
                string typeName = "(unknown)";

                bool hasPrefabBase = m_PrefabSystem.TryGetPrefab(prefabEntity, out Game.Prefabs.PrefabBase prefabBase);
                if (hasPrefabBase)
                {
                    name = prefabBase.name;
                    typeName = prefabBase.GetType().Name;
                }
                else
                {
                    missingPrefabBase++;
                }

                bool isScooter = IsScooterName(name);
                if (isScooter)
                {
                    scooters++;
                }
                else
                {
                    bikes++;
                }

                bool hasAuthoring = hasPrefabBase && prefabBase is Game.Prefabs.BicyclePrefab;
                Game.Prefabs.BicyclePrefab bikeAuthoring = hasAuthoring ? (Game.Prefabs.BicyclePrefab)prefabBase : null!;

                float expectedMaxMs = 0f;
                float expectedAccel = 0f;
                float expectedBrake = 0f;

                if (hasAuthoring)
                {
                    float baseMaxMs = KmhToMs(bikeAuthoring.m_MaxSpeed);
                    expectedMaxMs = baseMaxMs * effectiveSpeed;
                    expectedAccel = bikeAuthoring.m_Acceleration * effectiveAccelBrake;
                    expectedBrake = bikeAuthoring.m_Braking * effectiveAccelBrake;
                }

                bool hasCarData = SystemAPI.HasComponent<Game.Prefabs.CarData>(prefabEntity);
                Game.Prefabs.CarData car = default;

                bool speedMismatch = false;
                bool accelMismatch = false;
                bool brakeMismatch = false;

                if (hasCarData)
                {
                    car = SystemAPI.GetComponent<Game.Prefabs.CarData>(prefabEntity);

                    if (hasAuthoring)
                    {
                        speedMismatch = RelativeDiff(expectedMaxMs, car.m_MaxSpeed) > kSpeedMismatchPct;
                        accelMismatch = RelativeDiff(expectedAccel, car.m_Acceleration) > kAccelMismatchPct;
                        brakeMismatch = RelativeDiff(expectedBrake, car.m_Braking) > kBrakeMismatchPct;
                    }
                }
                else
                {
                    missingCarData++;
                }

                bool hasSway = SystemAPI.HasComponent<Game.Prefabs.SwayingData>(prefabEntity);
                Game.Prefabs.SwayingData sw = default;

                bool baselineCached = false;
                Game.Prefabs.SwayingData baselineSw = default;

                bool swayMaxPosMismatch = false;
                bool swayDampingMismatch = false;

                float3 expectedMaxPos = default;
                float3 expectedDamping = default;

                if (hasSway)
                {
                    sw = SystemAPI.GetComponent<Game.Prefabs.SwayingData>(prefabEntity);

                    baselineCached = m_SwayingBaseline.TryGetValue(prefabEntity, out baselineSw);
                    if (!baselineCached)
                    {
                        baselineSw = sw;
                    }

                    expectedMaxPos = enableFastBikes
                        ? (baselineSw.m_MaxPosition / math.max(0.01f, effectiveStiff))
                        : baselineSw.m_MaxPosition;

                    expectedDamping = enableFastBikes
                        ? (baselineSw.m_DampingFactors / math.max(0.01f, effectiveDamp))
                        : baselineSw.m_DampingFactors;

                    expectedDamping = math.clamp(expectedDamping, 0.01f, 0.999f);

                    if (baselineCached)
                    {
                        swayMaxPosMismatch = RelativeDiffMax(expectedMaxPos, sw.m_MaxPosition) > kSwayMaxPosMismatchPct;
                        swayDampingMismatch = RelativeDiffMax(expectedDamping, sw.m_DampingFactors) > kSwayDampingMismatchPct;
                    }
                }
                else
                {
                    missingSwaying++;
                }

                bool anyMismatch =
                    speedMismatch || accelMismatch || brakeMismatch ||
                    swayMaxPosMismatch || swayDampingMismatch;

                if (!anyMismatch)
                {
                    continue;
                }

                mismatchAny++;
                if (speedMismatch)
                {
                    mismatchSpeed++;
                }
                if (accelMismatch)
                {
                    mismatchAccel++;
                }
                if (brakeMismatch)
                {
                    mismatchBrake++;
                }
                if (swayMaxPosMismatch)
                {
                    mismatchSwayPos++;
                }
                if (swayDampingMismatch)
                {
                    mismatchSwayDamp++;
                }

                if (examples >= kMaxMismatchExamples)
                {
                    continue;
                }

                examples++;

                string kind = isScooter ? "Scooter" : "Bicycle";

                Mod.LogSafe(( ) =>
                    "-------------------- [FB] BIKE MISMATCH --------------------\n" +
                    $"Kind={kind} Name='{name}' Type={typeName} Entity={prefabEntity.Index}:{prefabEntity.Version}");

                if (hasAuthoring)
                {
                    Mod.LogSafe(( ) =>
                        $"AuthoringMax={bikeAuthoring.m_MaxSpeed:0.###} km/h ({KmhToMph(bikeAuthoring.m_MaxSpeed):0.###} mph)\n" +
                        $"ExpectedMax≈{MsToKmh(expectedMaxMs):0.###} km/h ({MsToMph(expectedMaxMs):0.###} mph)");
                }
                else
                {
                    Mod.LogSafe(( ) => "Authoring: (missing or not BicyclePrefab)");
                }

                if (hasCarData && hasAuthoring)
                {
                    Mod.LogSafe(( ) =>
                        $"CarDataMax={MsToKmh(car.m_MaxSpeed):0.###} km/h ({MsToMph(car.m_MaxSpeed):0.###} mph)\n" +
                        $"Diffs: Speed={FormatPct(RelativeDiff(expectedMaxMs, car.m_MaxSpeed))}, " +
                        $"Accel={FormatPct(RelativeDiff(expectedAccel, car.m_Acceleration))}, " +
                        $"Brake={FormatPct(RelativeDiff(expectedBrake, car.m_Braking))}");
                }
                else if (!hasCarData)
                {
                    Mod.LogSafe(( ) => "CarData: (missing)");
                }

                if (hasSway)
                {
                    Mod.LogSafe(( ) =>
                        $"SwayBaselineCached={baselineCached}\n" +
                        $"Baseline MaxPos={F3(baselineSw.m_MaxPosition)} Damping={F3(baselineSw.m_DampingFactors)}\n" +
                        $"Current  MaxPos={F3(sw.m_MaxPosition)} Damping={F3(sw.m_DampingFactors)}");

                    if (baselineCached)
                    {
                        Mod.LogSafe(( ) =>
                            $"Expected MaxPos≈{F3(expectedMaxPos)} Damping≈{F3(expectedDamping)}\n" +
                            $"Diffs: MaxPos(max)={FormatPct(RelativeDiffMax(expectedMaxPos, sw.m_MaxPosition))}, " +
                            $"Damping(max)={FormatPct(RelativeDiffMax(expectedDamping, sw.m_DampingFactors))}");
                    }
                }
            }

            Mod.LogSafe(( ) =>
                "==================== [FB] BIKE SUMMARY ====================\n" +
                $"Total={total} (Bicycles={bikes}, Scooters={scooters})\n" +
                $"MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwayingData={missingSwaying}\n" +
                $"MismatchAny={mismatchAny} (Speed={mismatchSpeed}, Accel={mismatchAccel}, Brake={mismatchBrake}, " +
                $"SwayMaxPos={mismatchSwayPos}, SwayDamping={mismatchSwayDamp})\n" +
                $"MismatchExamplesLogged={examples}/{kMaxMismatchExamples}");

            DumpPathSpeedReport(pathScalar);
        }

        private void DumpPathSpeedReport(float pathScalar)
        {
            Mod.LogSafe(( ) =>
                "==================== [FB] PATH SPEED SUMMARY ====================\n" +
                $"EffectivePathScalar={pathScalar:0.##}");

            // -------------------------------
            // PathwayData (prefab entities)
            // -------------------------------
            int prefabs = 0;
            int prefabMissingBase = 0;
            int prefabNotPathwayPrefab = 0;
            int prefabInvalidAuthoring = 0;
            int prefabMismatch = 0;

            // Read-only contamination checks.
            int prefabAlsoHasRoadData = 0;

            float prefabMin = float.PositiveInfinity;
            float prefabMax = float.NegativeInfinity;

            int prefabMismatchExamples = 0;

            foreach ((Unity.Entities.RefRO<Game.Prefabs.PathwayData> pathRO, Unity.Entities.Entity prefabEntity) in SystemAPI
                .Query<Unity.Entities.RefRO<Game.Prefabs.PathwayData>>()
                .WithAll<Game.Prefabs.PrefabData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>()
                .WithEntityAccess())
            {
                prefabs++;

                // Current runtime-on-prefab value (m/s).
                float currentMs = pathRO.ValueRO.m_SpeedLimit;

                prefabMin = math.min(prefabMin, currentMs);
                prefabMax = math.max(prefabMax, currentMs);

                if (SystemAPI.HasComponent<Game.Prefabs.RoadData>(prefabEntity))
                {
                    prefabAlsoHasRoadData++;
                }

                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out Game.Prefabs.PrefabBase prefabBase))
                {
                    prefabMissingBase++;
                    continue;
                }

                if (prefabBase is not Game.Prefabs.PathwayPrefab pathPrefab)
                {
                    prefabNotPathwayPrefab++;
                    continue;
                }

                // Authoring baseline (km/h) must be valid.
                if (pathPrefab.m_SpeedLimit <= 0f)
                {
                    prefabInvalidAuthoring++;
                    continue;
                }

                float expectedMs = KmhToMs(pathPrefab.m_SpeedLimit) * math.max(0.01f, pathScalar);

                if (RelativeDiff(expectedMs, currentMs) > kPathMismatchPct)
                {
                    prefabMismatch++;

                    if (prefabMismatchExamples < kMaxMismatchExamples)
                    {
                        prefabMismatchExamples++;

                        Mod.LogSafe(( ) =>
                            "-------------------- [FB] PATH PREFAB MISMATCH --------------------\n" +
                            $"Name='{prefabBase.name}'\n" +
                            $"Authoring={pathPrefab.m_SpeedLimit:0.###} km/h\n" +
                            $"Expected={MsToKmh(expectedMs):0.###} km/h\n" +
                            $"Current ={MsToKmh(currentMs):0.###} km/h\n" +
                            $"Diff    ={FormatPct(RelativeDiff(expectedMs, currentMs))}");
                    }
                }
            }

            Mod.LogSafe(( ) =>
                "-------------------- [FB] PATH PREFABS --------------------\n" +
                $"Count={prefabs}\n" +
                $"SpeedMin={MsToKmh(prefabMin):0.###} km/h, SpeedMax={MsToKmh(prefabMax):0.###} km/h\n" +
                $"MissingPrefabBase={prefabMissingBase}, NotPathwayPrefab={prefabNotPathwayPrefab}, InvalidAuthoring={prefabInvalidAuthoring}\n" +
                $"Mismatch>{kPathMismatchPct * 100f:0.##}%={prefabMismatch} (ExamplesLogged={prefabMismatchExamples}/{kMaxMismatchExamples})\n" +
                $"ContaminationCheck: PathwayDataPrefabsAlsoHaveRoadData={prefabAlsoHasRoadData}");

            // -------------------------------
            // PathwayComposition (net composition entities)
            // -------------------------------
            int comps = 0;
            int compMissingPathwayData = 0;
            int compMismatch = 0;

            float compMin = float.PositiveInfinity;
            float compMax = float.NegativeInfinity;

            int compMismatchExamples = 0;

            foreach ((Unity.Entities.RefRO<Game.Prefabs.PathwayComposition> compRO, Unity.Entities.RefRO<Game.Prefabs.PrefabRef> prefabRefRO) in SystemAPI
                .Query<Unity.Entities.RefRO<Game.Prefabs.PathwayComposition>, Unity.Entities.RefRO<Game.Prefabs.PrefabRef>>()
                .WithAll<Game.Prefabs.NetCompositionData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>())
            {
                comps++;

                float compMs = compRO.ValueRO.m_SpeedLimit;

                compMin = math.min(compMin, compMs);
                compMax = math.max(compMax, compMs);

                Unity.Entities.Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!SystemAPI.HasComponent<Game.Prefabs.PathwayData>(prefabEntity))
                {
                    compMissingPathwayData++;
                    continue;
                }

                float desiredMs = SystemAPI.GetComponent<Game.Prefabs.PathwayData>(prefabEntity).m_SpeedLimit;

                if (math.abs(compMs - desiredMs) > kCompAbsMismatchMs)
                {
                    compMismatch++;

                    if (compMismatchExamples < kMaxMismatchExamples)
                    {
                        compMismatchExamples++;

                        Mod.LogSafe(( ) =>
                            "-------------------- [FB] PATH COMPOSITION MISMATCH --------------------\n" +
                            $"Composition={MsToKmh(compMs):0.###} km/h\n" +
                            $"Desired    ={MsToKmh(desiredMs):0.###} km/h\n" +
                            $"AbsDiff    ={math.abs(compMs - desiredMs):0.###} m/s");
                    }
                }
            }

            Mod.LogSafe(( ) =>
                "-------------------- [FB] PATH COMPOSITIONS --------------------\n" +
                $"Count={comps}\n" +
                $"SpeedMin={MsToKmh(compMin):0.###} km/h, SpeedMax={MsToKmh(compMax):0.###} km/h\n" +
                $"MissingPathwayDataOnPrefabRef={compMissingPathwayData}\n" +
                $"MismatchAbs>{kCompAbsMismatchMs:0.###} m/s={compMismatch} (ExamplesLogged={compMismatchExamples}/{kMaxMismatchExamples})");
        }
    }
}
