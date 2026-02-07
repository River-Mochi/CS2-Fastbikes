// File: Systems/FastBikeSystem.Dump.cs
// Purpose: Dump bicycle/scooter prefab values (authoring + current prefab-entity data) for debugging.

namespace FastBikes
{
    using Game.Common;        // Deleted
    using Game.Prefabs;       // PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData, SwayingData
    using Game.Tools;         // Temp
    using System;             // StringComparison
    using Unity.Entities;     // Entity, RefRO, SystemAPI
    using Unity.Mathematics;  // math.*

    public sealed partial class FastBikeSystem
    {
        private bool m_Dump;

        // Mismatch thresholds (relative / percent of expected).
        private const float kSpeedMismatchPct = 0.05f;   // 5%
        private const float kAccelMismatchPct = 0.10f;   // 10%
        private const float kBrakeMismatchPct = 0.10f;   // 10%

        private const float kSwayMaxPosMismatchPct = 0.10f;   // 10%
        private const float kSwayDampingMismatchPct = 0.15f;  // 15%

        public void ScheduleDump()
        {
            m_Dump = true;
            Enabled = true;
        }

        private static float MsToKmh(float ms) => ms * 3.6f;
        private static float MsToMph(float ms) => ms * 2.23693629f;

        private static float KmhToMs(float kmh) => kmh * (1f / 3.6f);
        private static float KmhToMph(float kmh) => kmh * 0.621371f;

        private static string F3(float3 v)
        {
            return $"({v.x:0.###},{v.y:0.###},{v.z:0.###})";
        }

        private static bool IsScooterName(string name)
        {
            return !string.IsNullOrEmpty(name) &&
                   name.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase);
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
            return math.cmax(rel); // max component diff
        }

        private static string FormatPct(float value01)
        {
            return (value01 * 100f).ToString("0.##") + "%";
        }

        private static string BuildMismatchTags(
            bool speedMismatch,
            bool accelMismatch,
            bool brakeMismatch,
            bool swayMaxPosMismatch,
            bool swayDampingMismatch)
        {
            string tags = string.Empty;

            if (speedMismatch) { tags += "Speed,"; }
            if (accelMismatch) { tags += "Accel,"; }
            if (brakeMismatch) { tags += "Brake,"; }
            if (swayMaxPosMismatch) { tags += "SwayMaxPos,"; }
            if (swayDampingMismatch) { tags += "SwayDamping,"; }

            if (string.IsNullOrEmpty(tags))
            {
                return string.Empty;
            }

            return "MISMATCH(" + tags.TrimEnd(',') + ")";
        }

        private void DumpBicyclePrefabs(bool enableFastBikes, float speedScalar, float stiffnessScalar, float dampingScalar)
        {
            // Match Apply() behavior: toggle OFF -> 1.0, toggle ON -> clamped sliders.
            float effectiveSpeed = enableFastBikes ? math.clamp(speedScalar, 0.30f, 10.0f) : 1.0f;
            float effectiveStiff = enableFastBikes ? math.clamp(stiffnessScalar, 0.30f, 5.0f) : 1.0f;
            float effectiveDamp = enableFastBikes ? math.clamp(dampingScalar, 0.30f, 5.0f) : 1.0f;

            float effectiveAccelBrake = math.sqrt(math.max(0.01f, effectiveSpeed));

            Mod.LogSafe(() =>
                "[FB] Dump start. " +
                $"EnableFastBikes={enableFastBikes}, " +
                $"Speed={speedScalar:0.##} (Eff={effectiveSpeed:0.##}), " +
                $"Stiff={stiffnessScalar:0.##} (Eff={effectiveStiff:0.##}), " +
                $"Damp={dampingScalar:0.##} (Eff={effectiveDamp:0.##})");

            int total = 0;
            int bikes = 0;
            int scooters = 0;

            int missingPrefabBase = 0;
            int missingCarData = 0;
            int missingSwaying = 0;

            int mismatchedPrefabs = 0;

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI.Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                total++;

                // Resolve prefab name/type via PrefabSystem -> PrefabBase.
                string name = "(PrefabBase not found)";
                string typeName = "(unknown)";

                bool hasPrefabBase = m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase);
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
                if (isScooter) { scooters++; }
                else { bikes++; }

                string kind = isScooter ? "Scooter" : "Bicycle";

                // Authoring (BicyclePrefab) drives the expected CarData numbers.
                bool hasAuthoring = hasPrefabBase && prefabBase is BicyclePrefab;
                BicyclePrefab bikeAuthoring = hasAuthoring ? (BicyclePrefab)prefabBase : null!;

                float expectedMaxMs = 0f;
                float expectedAccel = 0f;
                float expectedBrake = 0f;

                if (hasAuthoring)
                {
                    float baseMaxMs = KmhToMs(bikeAuthoring.m_MaxSpeed); // authoring km/h -> runtime m/s
                    expectedMaxMs = baseMaxMs * effectiveSpeed;
                    expectedAccel = bikeAuthoring.m_Acceleration * effectiveAccelBrake;
                    expectedBrake = bikeAuthoring.m_Braking * effectiveAccelBrake;
                }

                // Current CarData (prefab entity component).
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

                // Current SwayingData (prefab entity component) + expected derived from cached baseline.
                bool hasSway = SystemAPI.HasComponent<SwayingData>(prefabEntity);
                SwayingData sw = default;

                bool baselineCached = false;
                SwayingData baselineSw = default;

                bool swayMaxPosMismatch = false;
                bool swayDampingMismatch = false;

                float3 expectedMaxPos = default;
                float3 expectedDamping = default;

                if (hasSway)
                {
                    sw = SystemAPI.GetComponent<SwayingData>(prefabEntity);

                    // Baseline is the first-seen SwayingData cached by ApplyBicycleSwaying().
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

                    // Same safety clamp as ApplyBicycleSwaying().
                    expectedDamping = math.clamp(expectedDamping, 0.01f, 0.999f);

                    // Only flag mismatch when the baseline was actually cached (otherwise expected==current).
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

                if (anyMismatch)
                {
                    mismatchedPrefabs++;
                }

                string mismatchTags = BuildMismatchTags(
                    speedMismatch,
                    accelMismatch,
                    brakeMismatch,
                    swayMaxPosMismatch,
                    swayDampingMismatch);

                Mod.LogSafe(() =>
                    $"[FB] {kind} prefab: entity={prefabEntity.Index}:{prefabEntity.Version} name='{name}' type={typeName}" +
                    (string.IsNullOrEmpty(mismatchTags) ? string.Empty : " [" + mismatchTags + "]"));

                // Authoring (PrefabBase)
                if (hasAuthoring)
                {
                    Mod.LogSafe(() =>
                        "[FB]   Authoring: " +
                        $"MaxSpeed={bikeAuthoring.m_MaxSpeed:0.###} km/h ({KmhToMph(bikeAuthoring.m_MaxSpeed):0.###} mph), " +
                        $"Accel={bikeAuthoring.m_Acceleration:0.###}, Brake={bikeAuthoring.m_Braking:0.###}, " +
                        $"Turning=({bikeAuthoring.m_Turning.x:0.###},{bikeAuthoring.m_Turning.y:0.###}), " +
                        $"Stiffness={bikeAuthoring.m_Stiffness:0.###}");

                    Mod.LogSafe(() =>
                        "[FB]   Expected: " +
                        $"MaxSpeed≈{expectedMaxMs:0.###} m/s ({MsToMph(expectedMaxMs):0.###} mph), " +
                        $"Accel≈{expectedAccel:0.###}, Brake≈{expectedBrake:0.###}");
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   Authoring: (missing or not BicyclePrefab)");
                }

                // Current CarData
                if (hasCarData)
                {
                    float carMs = car.m_MaxSpeed;

                    Mod.LogSafe(() =>
                        "[FB]   CarData: " +
                        $"MaxSpeed={carMs:0.###} m/s ({MsToKmh(carMs):0.###} km/h, {MsToMph(carMs):0.###} mph), " +
                        $"Accel={car.m_Acceleration:0.###}, Brake={car.m_Braking:0.###}");

                    if (hasAuthoring)
                    {
                        Mod.LogSafe(() =>
                            "[FB]   Car diffs: " +
                            $"Speed={FormatPct(RelativeDiff(expectedMaxMs, car.m_MaxSpeed))}, " +
                            $"Accel={FormatPct(RelativeDiff(expectedAccel, car.m_Acceleration))}, " +
                            $"Brake={FormatPct(RelativeDiff(expectedBrake, car.m_Braking))}");
                    }
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   CarData: (missing)");
                }

                // SwayingData (float3 fields)
                if (hasSway)
                {
                    Mod.LogSafe(() =>
                        "[FB]   Sway baseline: " +
                        $"Cached={baselineCached}, MaxPos={F3(baselineSw.m_MaxPosition)}, " +
                        $"Damping={F3(baselineSw.m_DampingFactors)}, Spring={F3(baselineSw.m_SpringFactors)}");

                    Mod.LogSafe(() =>
                        "[FB]   Sway current : " +
                        $"MaxPos={F3(sw.m_MaxPosition)}, Damping={F3(sw.m_DampingFactors)}, Spring={F3(sw.m_SpringFactors)}");

                    if (baselineCached)
                    {
                        Mod.LogSafe(() =>
                            "[FB]   Sway expected: " +
                            $"MaxPos≈{F3(expectedMaxPos)}, Damping≈{F3(expectedDamping)}");

                        Mod.LogSafe(() =>
                            "[FB]   Sway diffs  : " +
                            $"MaxPos(max)={FormatPct(RelativeDiffMax(expectedMaxPos, sw.m_MaxPosition))}, " +
                            $"Damping(max)={FormatPct(RelativeDiffMax(expectedDamping, sw.m_DampingFactors))}");
                    }
                    else
                    {
                        Mod.LogSafe(() =>
                            "[FB]   Sway expected: (baseline not cached yet; run Apply once, then Dump again for comparisons)");
                    }
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   SwayingData: (missing)");
                }
            }

            Mod.LogSafe(() =>
                "[FB] Dump complete. " +
                $"Total={total} (Bicycles={bikes}, Scooters={scooters}), " +
                $"MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwaying={missingSwaying}, " +
                $"MismatchedPrefabs={mismatchedPrefabs}");
        }
    }
}
