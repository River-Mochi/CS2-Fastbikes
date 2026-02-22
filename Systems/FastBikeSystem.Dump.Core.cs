// File: Systems/FastBikeSystem.Dump.Core.cs
// Purpose: Dump entrypoint + bicycle prefab sanity + scalar summary.
// Notes:
// - Dump is read-only.
// - Debug-only mismatch examples; Release logs counts only.

namespace FastBikes
{
    using Game.Common;                // Deleted, Overridden
    using Game.Prefabs;               // BicyclePrefab, BicycleData, CarData, SwayingData, PrefabBase, PrefabData, PrefabRef
    using Game.Tools;                 // Temp
    using System;                     // StringComparison
    using System.Collections.Generic; // List, HashSet
    using Unity.Entities;             // Entity, SystemAPI, RefRO
    using Unity.Mathematics;          // math, float3

    public sealed partial class FastBikeSystem
    {
        private bool m_Dump;

        private static readonly string[] s_ExpectedBicycleGroupNames =
        {
            "Bicycle01",
            "Bicycle02",
            "Bicycle03",
            "ElectricScooter01",
        };

        private const float kSpeedMismatchPct = 0.05f;
        private const float kAccelMismatchPct = 0.10f;
        private const float kBrakeMismatchPct = 0.10f;

        private const float kSwayMaxPosMismatchPct = 0.10f;
        private const float kSwayDampingMismatchPct = 0.15f;

        private const float kPathMismatchPct = 0.05f;
        private const float kCompAbsMismatchMs = 0.10f;

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

        private static string FormatIndexVersion(Entity e) => $"{e.Index}:{e.Version}";

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

        // Signature must match call site using named args.
        private void DumpBicyclePrefabs(bool enableFastBikes, float speedScalar, float stiffnessScalar, float dampingScalar)
        {
            float effectiveSpeed = enableFastBikes ? Unity.Mathematics.math.clamp(speedScalar, 0.30f, 10.0f) : 1.0f;
            float effectiveStiff = enableFastBikes ? Unity.Mathematics.math.clamp(stiffnessScalar, 0.30f, 5.0f) : 1.0f;
            float effectiveDamp = enableFastBikes ? Unity.Mathematics.math.clamp(dampingScalar, 0.30f, 5.0f) : 1.0f;

            float effectiveAccelBrake = Unity.Mathematics.math.sqrt(Unity.Mathematics.math.max(0.01f, effectiveSpeed));

            float rawPathSpeed = -1f;
            float pathScalar = 1.0f;

            if (enableFastBikes && TryGetSettings(out Setting settings))
            {
                rawPathSpeed = settings.PathSpeedScalar;
                pathScalar = Unity.Mathematics.math.clamp(rawPathSpeed, 1.0f, 5.0f);
            }

            string pathBatchLine = IsPathBatchActive()
                ? $"Path lane batch: RUNNING (EdgesRemaining={m_PathEdgeEntities.Length - m_PathEdgeIndex}/{m_PathEdgeEntities.Length})"
                : "Path lane batch: IDLE (no queued work)";

            Mod.LogSafe(( ) =>
                "\n==================== [FB] Scalar SUMMARY ====================\n" +
                "Meaning: quick sanity of scalars + group membership + patch/custom changes.\n" +
                $"EnableFastBikes={enableFastBikes}\n" +
                $"Bike Speed Scalar={speedScalar:0.##} (Effective={effectiveSpeed:0.##})\n" +
                $"Stiffness Scalar={stiffnessScalar:0.##} (Effective={effectiveStiff:0.##})\n" +
                $"Damping Scalar={dampingScalar:0.##} (Effective={effectiveDamp:0.##})\n" +
                $"Path Speed Scalar={rawPathSpeed:0.##} (Effective={pathScalar:0.##})\n" +
                pathBatchLine);

            var groupNames = new List<string>(16);
            var groupNameSet = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

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

#if DEBUG
            int examplesLogged = 0;
#endif

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                total++;

                string name = "(PrefabBase not found)";
                string typeName = "(unknown)";

                bool hasPrefabBase = m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase);
                if (hasPrefabBase)
                {
                    name = prefabBase.name ?? "(null name)";
                    typeName = prefabBase.GetType().Name;

                    if (groupNameSet.Add(name))
                    {
                        groupNames.Add(name);
                    }
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

                bool hasAuthoring = hasPrefabBase && prefabBase is BicyclePrefab;
                BicyclePrefab bikeAuthoring = hasAuthoring ? (BicyclePrefab)prefabBase : null!;

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

                bool hasCarData = SystemAPI.HasComponent<CarData>(prefabEntity);
                CarData car = default;

                bool speedMismatch = false;
                bool accelMismatch = false;
                bool brakeMismatch = false;

                if (hasCarData)
                {
                    car = SystemAPI.GetComponent<CarData>(prefabEntity);

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

                bool hasSway = SystemAPI.HasComponent<SwayingData>(prefabEntity);
                SwayingData sw = default;

                bool baselineCached = false;
                SwayingData baselineSw = default;

                bool swayMaxPosMismatch = false;
                bool swayDampingMismatch = false;

                Unity.Mathematics.float3 expectedMaxPos = default;
                Unity.Mathematics.float3 expectedDamping = default;

                if (hasSway)
                {
                    sw = SystemAPI.GetComponent<SwayingData>(prefabEntity);

                    baselineCached = m_SwayingBaseline.TryGetValue(prefabEntity, out baselineSw);
                    if (!baselineCached)
                    {
                        baselineSw = sw;
                    }

                    expectedMaxPos = enableFastBikes
                        ? (baselineSw.m_MaxPosition / Unity.Mathematics.math.max(0.01f, effectiveStiff))
                        : baselineSw.m_MaxPosition;

                    expectedDamping = enableFastBikes
                        ? (baselineSw.m_DampingFactors / Unity.Mathematics.math.max(0.01f, effectiveDamp))
                        : baselineSw.m_DampingFactors;

                    expectedDamping = Unity.Mathematics.math.clamp(expectedDamping, 0.01f, 0.999f);

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
                    mismatchSpeed++;
                if (accelMismatch)
                    mismatchAccel++;
                if (brakeMismatch)
                    mismatchBrake++;
                if (swayMaxPosMismatch)
                    mismatchSwayPos++;
                if (swayDampingMismatch)
                    mismatchSwayDamp++;

#if DEBUG
                if (examplesLogged >= kMaxMismatchExamples)
                {
                    continue;
                }

                examplesLogged++;

                string kind = isScooter ? "Scooter" : "Bicycle";

                Mod.LogSafe(( ) =>
                    "-------------------- [FB] BIKE MISMATCH (DEBUG) --------------------\n" +
                    "Meaning: mismatch suggests overwrite-after-run, mod not running, or CO behavior change.\n" +
                    $"Kind={kind} Name='{name}' Type={typeName} Entity={FormatIndexVersion(prefabEntity)}");

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
#endif
            }

            groupNames.Sort(System.StringComparer.OrdinalIgnoreCase);

            var missingExpected = new List<string>(4);
            for (int i = 0; i < s_ExpectedBicycleGroupNames.Length; i++)
            {
                if (!groupNameSet.Contains(s_ExpectedBicycleGroupNames[i]))
                {
                    missingExpected.Add(s_ExpectedBicycleGroupNames[i]);
                }
            }

            // IMPORTANT: extras are allowed (DLC/custom assets). Only missing expected names are flagged.
            bool clean =
                missingExpected.Count == 0 &&
                missingPrefabBase == 0 &&
                missingCarData == 0 &&
                missingSwaying == 0 &&
                mismatchAny == 0;

            if (clean)
            {
                Mod.LogSafe(( ) =>
                    $"[FB] BIKE SUMMARY: ALL GOOD (Total={total}, Bicycles={bikes}, Scooters={scooters}).");

#if DEBUG
                Mod.LogSafe(( ) =>
                    $"[FB] BIKE SUMMARY (DEBUG): Names={string.Join(", ", groupNames)}");
#endif
            }
            else
            {
#if DEBUG
                Mod.LogSafe(( ) =>
                {
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("==================== [FB] BIKE SUMMARY (DEBUG) ====================");
                    sb.AppendLine("Meaning: group membership + missing expected names + mismatch counts.");
                    sb.AppendLine($"Total={total} (Bicycles={bikes}, Scooters={scooters})");
                    sb.AppendLine($"BicycleGroupNames={string.Join(", ", groupNames)}");

                    if (missingExpected.Count > 0)
                    {
                        sb.AppendLine($"WARNING MissingExpectedNames={string.Join(", ", missingExpected)}");
                    }

                    sb.AppendLine($"MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwayingData={missingSwaying}");
                    sb.AppendLine($"MismatchAny={mismatchAny} (Speed={mismatchSpeed}, Accel={mismatchAccel}, Brake={mismatchBrake}, SwayMaxPos={mismatchSwayPos}, SwayDamping={mismatchSwayDamp})");
                    sb.AppendLine($"DEBUG MismatchExamplesLogged={examplesLogged}/{kMaxMismatchExamples}");
                    return sb.ToString();
                });
#else
                Mod.LogSafe(( ) =>
                    $"[FB] BIKE SUMMARY: ISSUES (Total={total}, MissingExpected={missingExpected.Count}, MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwayingData={missingSwaying}).");
#endif
            }

            DumpPathSpeedReport(pathScalar);
            DumpScooter01Report();
            DumpCitizenVehicleEligibilityReport();

            // Defined in Systems/FastBikeSystem.BikeInstances.cs (keep logic there).
            DumpBikeInstancesReport();
            DumpCarGroupInstancesReport();
        }

        private static bool IsExpectedBicycleGroupName(string name)
        {
            for (int i = 0; i < s_ExpectedBicycleGroupNames.Length; i++)
            {
                if (string.Equals(name, s_ExpectedBicycleGroupNames[i], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
