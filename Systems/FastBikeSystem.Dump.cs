// File: Systems/FastBikeSystem.Dump.cs
// Purpose: Dump bicycle/scooter prefab values (authoring + current prefab-entity data) for debugging.

namespace FastBikes
{
    using Game.Common;        // Deleted, Owner
    using Game.Prefabs;       // PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData, SwayingData, PathwayPrefab, PathwayData, PathwayComposition, PrefabRef, NetCompositionData
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

        private const float kPathMismatchPct = 0.02f;   // 2%
        private const float kLaneAbsMismatchMs = 0.10f; // ~0.36 km/h

        public void ScheduleDump()
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

        private static bool TryGetSettings(out Setting settings)
        {
            if (Mod.Settings is Setting s)
            {
                settings = s;
                return true;
            }

            settings = default!;
            Mod.WarnOnce("FB_SETTINGS_NULL", () => "[FB] Settings is null; Dump using PathSpeed=1x.");
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

            Mod.LogSafe(() =>
                "[FB] Dump start. " +
                $"EnableFastBikes={enableFastBikes}, " +
                $"Speed={speedScalar:0.##} (Eff={effectiveSpeed:0.##}), " +
                $"Stiff={stiffnessScalar:0.##} (Eff={effectiveStiff:0.##}), " +
                $"Damp={dampingScalar:0.##} (Eff={effectiveDamp:0.##}), " +
                $"PathSpeed={rawPathSpeed:0.##} (Eff={pathScalar:0.##})");

            int total = 0;
            int bikes = 0;
            int scooters = 0;

            int missingPrefabBase = 0;
            int missingCarData = 0;
            int missingSwaying = 0;

            int mismatchedPrefabs = 0;

            foreach ((RefRO<Game.Prefabs.PrefabData> _, Entity prefabEntity) in SystemAPI.Query<RefRO<Game.Prefabs.PrefabData>>()
                .WithAll<Game.Prefabs.BicycleData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .WithEntityAccess())
            {
                total++;

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
                        $"MaxSpeed≈{expectedMaxMs:0.###} m/s ({MsToKmh(expectedMaxMs):0.###} km/h, {MsToMph(expectedMaxMs):0.###} mph), " +
                        $"Accel≈{expectedAccel:0.###}, Brake≈{expectedBrake:0.###}");
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   Authoring: (missing or not BicyclePrefab)");
                }

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
                "[FB] Dump bicycles complete. " +
                $"Total={total} (Bicycles={bikes}, Scooters={scooters}), " +
                $"MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwaying={missingSwaying}, " +
                $"MismatchedPrefabs={mismatchedPrefabs}");

            DumpPathSpeedReport(pathScalar);
        }

        private void DumpPathSpeedReport(float pathScalar)
        {
            int prefabs = 0;
            int prefabMissingBase = 0;
            int prefabMismatch = 0;

            float prefabMin = float.PositiveInfinity;
            float prefabMax = float.NegativeInfinity;

            foreach ((RefRO<Game.Prefabs.PathwayData> pathRO, Entity prefabEntity) in SystemAPI
                .Query<RefRO<Game.Prefabs.PathwayData>>()
                .WithAll<Game.Prefabs.PrefabData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .WithEntityAccess())
            {
                prefabs++;

                float currentMs = pathRO.ValueRO.m_SpeedLimit;

                prefabMin = math.min(prefabMin, currentMs);
                prefabMax = math.max(prefabMax, currentMs);

                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    prefabMissingBase++;
                    continue;
                }

                if (prefabBase is not PathwayPrefab pathPrefab)
                {
                    continue;
                }

                float expectedMs = KmhToMs(pathPrefab.m_SpeedLimit) * math.max(0.01f, pathScalar);

                if (RelativeDiff(expectedMs, currentMs) > kPathMismatchPct)
                {
                    prefabMismatch++;
                    Mod.LogSafe(() =>
                        "[FB] Path prefab mismatch: " +
                        $"name='{prefabBase.name}', " +
                        $"Authoring={pathPrefab.m_SpeedLimit:0.###} km/h, " +
                        $"Expected={MsToKmh(expectedMs):0.###} km/h, Current={MsToKmh(currentMs):0.###} km/h, " +
                        $"Diff={FormatPct(RelativeDiff(expectedMs, currentMs))}");
                }
            }

            Mod.LogSafe(() =>
                "[FB] Path prefabs summary: " +
                $"Count={prefabs}, MissingPrefabBase={prefabMissingBase}, Mismatch>{kPathMismatchPct * 100f:0.##}%={prefabMismatch}, " +
                $"PrefabSpeedMin={MsToKmh(prefabMin):0.###} km/h, PrefabSpeedMax={MsToKmh(prefabMax):0.###} km/h");

            int comps = 0;
            int compMismatch = 0;
            float compMin = float.PositiveInfinity;
            float compMax = float.NegativeInfinity;

            foreach ((RefRO<Game.Prefabs.PathwayComposition> compRO, RefRO<Game.Prefabs.PrefabRef> prefabRefRO) in SystemAPI
                .Query<RefRO<Game.Prefabs.PathwayComposition>, RefRO<Game.Prefabs.PrefabRef>>()
                .WithAll<Game.Prefabs.NetCompositionData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>())
            {
                comps++;

                float compMs = compRO.ValueRO.m_SpeedLimit;

                compMin = math.min(compMin, compMs);
                compMax = math.max(compMax, compMs);

                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!SystemAPI.HasComponent<Game.Prefabs.PathwayData>(prefabEntity))
                {
                    continue;
                }

                float desiredMs = SystemAPI.GetComponent<Game.Prefabs.PathwayData>(prefabEntity).m_SpeedLimit;

                if (math.abs(compMs - desiredMs) > kLaneAbsMismatchMs)
                {
                    compMismatch++;
                }
            }

            Mod.LogSafe(() =>
                "[FB] Path compositions summary: " +
                $"Count={comps}, MismatchAbs>{kLaneAbsMismatchMs:0.###} m/s={compMismatch}, " +
                $"CompSpeedMin={MsToKmh(compMin):0.###} km/h, CompSpeedMax={MsToKmh(compMax):0.###} km/h");

            int lanes = 0;
            int pathLanes = 0;
            int laneMismatch = 0;
            int laneDefaultMismatch = 0;

            float laneMin = float.PositiveInfinity;
            float laneMax = float.NegativeInfinity;

            float laneDefaultMin = float.PositiveInfinity;
            float laneDefaultMax = float.NegativeInfinity;

            foreach ((RefRO<Game.Net.CarLane> laneRO, RefRO<Game.Common.Owner> ownerRO) in SystemAPI
                .Query<RefRO<Game.Net.CarLane>, RefRO<Game.Common.Owner>>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>())
            {
                lanes++;

                Entity ownerEntity = ownerRO.ValueRO.m_Owner;

                if (!SystemAPI.HasComponent<Game.Prefabs.PrefabRef>(ownerEntity))
                {
                    continue;
                }

                Game.Prefabs.PrefabRef pref = SystemAPI.GetComponent<Game.Prefabs.PrefabRef>(ownerEntity);
                Entity prefabEntity = pref.m_Prefab;

                if (!SystemAPI.HasComponent<Game.Prefabs.PathwayData>(prefabEntity))
                {
                    continue;
                }

                pathLanes++;

                float laneMs = laneRO.ValueRO.m_SpeedLimit;
                float laneDefaultMs = laneRO.ValueRO.m_DefaultSpeedLimit;

                laneMin = math.min(laneMin, laneMs);
                laneMax = math.max(laneMax, laneMs);

                laneDefaultMin = math.min(laneDefaultMin, laneDefaultMs);
                laneDefaultMax = math.max(laneDefaultMax, laneDefaultMs);

                float desiredMs = SystemAPI.GetComponent<Game.Prefabs.PathwayData>(prefabEntity).m_SpeedLimit;

                bool speedBad = math.abs(laneMs - desiredMs) > kLaneAbsMismatchMs;
                bool defaultBad = math.abs(laneDefaultMs - desiredMs) > kLaneAbsMismatchMs;

                if (speedBad || defaultBad)
                {
                    laneMismatch++;
                }

                if (defaultBad)
                {
                    laneDefaultMismatch++;
                }
            }

            Mod.LogSafe(() =>
                "[FB] Lane speed summary: " +
                $"TotalCarLanes={lanes}, PathCarLanes={pathLanes}, " +
                $"MismatchAbs>{kLaneAbsMismatchMs:0.###} m/s={laneMismatch}, DefaultMismatchAbs>{kLaneAbsMismatchMs:0.###} m/s={laneDefaultMismatch}, " +
                $"LaneSpeedMin={MsToKmh(laneMin):0.###} km/h, LaneSpeedMax={MsToKmh(laneMax):0.###} km/h, " +
                $"LaneDefaultMin={MsToKmh(laneDefaultMin):0.###} km/h, LaneDefaultMax={MsToKmh(laneDefaultMax):0.###} km/h");
        }
    }
}
