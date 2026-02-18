// File: Systems/FastBikeSystem.Dump.cs
// Purpose: Dump bicycle-group prefab names + patch-day sanity + Scooter01 instance count + sample entity ids.
// Notes:
// - Dump is read-only.
// - No Burst usage.
// - No chunk jobs.
// - Debug-only mismatch examples; Release logs counts only.

namespace FastBikes
{
    using Colossal.Entities;          // EntityManager
    using Game.Common;                // Deleted, Overridden
    using Game.Prefabs;               // PrefabBase, BicyclePrefab, BicycleData, CarData, SwayingData, PathwayPrefab, PathwayData, PathwayComposition, PrefabData, PrefabRef, NetCompositionData, RoadData
    using Game.Tools;                 // Temp
    using Game.Vehicles;              // Car, ParkedCar
    using System;
    using System.Collections.Generic;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine.Profiling;

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

            bool pathBatchActive = IsPathBatchActive();
            string pathBatchLine = pathBatchActive
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
                    scooters++;
                else
                    bikes++;

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

                float3 expectedMaxPos = default;
                float3 expectedDamping = default;

                if (hasSway)
                {
                    sw = SystemAPI.GetComponent<SwayingData>(prefabEntity);

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

                Mod.LogSafe(() =>
                    "-------------------- [FB] BIKE MISMATCH (DEBUG) --------------------\n" +
                    "Meaning: mismatch suggests overwrite-after-run, mod not running, or CO behavior change.\n" +
                    $"Kind={kind} Name='{name}' Type={typeName} Entity={FormatIndexVersion(prefabEntity)}");

                if (hasAuthoring)
                {
                    Mod.LogSafe(() =>
                        $"AuthoringMax={bikeAuthoring.m_MaxSpeed:0.###} km/h ({KmhToMph(bikeAuthoring.m_MaxSpeed):0.###} mph)\n" +
                        $"ExpectedMax≈{MsToKmh(expectedMaxMs):0.###} km/h ({MsToMph(expectedMaxMs):0.###} mph)");
                }
                else
                {
                    Mod.LogSafe(() => "Authoring: (missing or not BicyclePrefab)");
                }

                if (hasCarData && hasAuthoring)
                {
                    Mod.LogSafe(() =>
                        $"CarDataMax={MsToKmh(car.m_MaxSpeed):0.###} km/h ({MsToMph(car.m_MaxSpeed):0.###} mph)\n" +
                        $"Diffs: Speed={FormatPct(RelativeDiff(expectedMaxMs, car.m_MaxSpeed))}, " +
                        $"Accel={FormatPct(RelativeDiff(expectedAccel, car.m_Acceleration))}, " +
                        $"Brake={FormatPct(RelativeDiff(expectedBrake, car.m_Braking))}");
                }
                else if (!hasCarData)
                {
                    Mod.LogSafe(() => "CarData: (missing)");
                }

                if (hasSway)
                {
                    Mod.LogSafe(() =>
                        $"SwayBaselineCached={baselineCached}\n" +
                        $"Baseline MaxPos={F3(baselineSw.m_MaxPosition)} Damping={F3(baselineSw.m_DampingFactors)}\n" +
                        $"Current  MaxPos={F3(sw.m_MaxPosition)} Damping={F3(sw.m_DampingFactors)}");

                    if (baselineCached)
                    {
                        Mod.LogSafe(() =>
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

            var extras = new List<string>(16);
            for (int i = 0; i < groupNames.Count; i++)
            {
                if (!IsExpectedBicycleGroupName(groupNames[i]))
                {
                    extras.Add(groupNames[i]);
                }
            }

            bool clean =
                missingExpected.Count == 0 &&
                extras.Count == 0 &&
                missingPrefabBase == 0 &&
                missingCarData == 0 &&
                missingSwaying == 0 &&
                mismatchAny == 0;

#if !DEBUG
            if (clean)
            {
                Mod.LogSafe(( ) =>
                    $"[FB] BIKE SUMMARY: ALL GOOD (Total={total}, Bicycles={bikes}, Scooters={scooters}).");
            }
            else
            {
                Mod.LogSafe(( ) =>
                {
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("==================== [FB] BIKE SUMMARY ====================");
                    sb.AppendLine("Meaning: group membership + missing/extra names + mismatch counts.");
                    sb.AppendLine($"Total={total} (Bicycles={bikes}, Scooters={scooters})");
                    sb.AppendLine($"BicycleGroupNames={string.Join(", ", groupNames)}");

                    if (missingExpected.Count > 0)
                    {
                        sb.AppendLine($"WARNING MissingExpectedNames={string.Join(", ", missingExpected)}");
                    }

                    if (extras.Count > 0)
                    {
                        sb.AppendLine($"NOTE ExtraNames(patch/custom assets)={string.Join(", ", extras)}");
                    }

                    sb.AppendLine($"MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwayingData={missingSwaying}");
                    sb.AppendLine($"MismatchAny={mismatchAny} (Speed={mismatchSpeed}, Accel={mismatchAccel}, Brake={mismatchBrake}, SwayMaxPos={mismatchSwayPos}, SwayDamping={mismatchSwayDamp})");
                    sb.AppendLine("Release build: mismatch details suppressed (counts only).");

                    return sb.ToString();
                });
            }
#else
            Mod.LogSafe(() =>
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("==================== [FB] BIKE SUMMARY ====================");
                sb.AppendLine("Meaning: group membership + missing/extra names + mismatch counts.");
                sb.AppendLine($"Total={total} (Bicycles={bikes}, Scooters={scooters})");
                sb.AppendLine($"BicycleGroupNames={string.Join(", ", groupNames)}");

                if (missingExpected.Count > 0)
                {
                    sb.AppendLine($"WARNING MissingExpectedNames={string.Join(", ", missingExpected)}");
                }

                if (extras.Count > 0)
                {
                    sb.AppendLine($"NOTE ExtraNames(patch/custom assets)={string.Join(", ", extras)}");
                }

                sb.AppendLine($"MissingPrefabBase={missingPrefabBase}, MissingCarData={missingCarData}, MissingSwayingData={missingSwaying}");
                sb.AppendLine($"MismatchAny={mismatchAny} (Speed={mismatchSpeed}, Accel={mismatchAccel}, Brake={mismatchBrake}, SwayMaxPos={mismatchSwayPos}, SwayDamping={mismatchSwayDamp})");
                sb.AppendLine($"DEBUG MismatchExamplesLogged={examplesLogged}/{kMaxMismatchExamples}");

                return sb.ToString();
            });
#endif

            DumpPathSpeedReport(pathScalar);
            DumpScooter01Report();
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


        private void DumpScooter01Report( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] SCOOTER01 (FUEL) ====================\n" +
                "Usage: locate Scooter01 prefab by name, count live instances, log up to 15 samples for SE Mod.");

            const int kMaxSamples = 15; // 15 sameple entities.
            Entity scooterPrefabEntity = Entity.Null;

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithNone<Deleted, Temp, Overridden>()
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

            var samples = new System.Collections.Generic.List<Entity>(kMaxSamples);

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity vehicleEntity) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Car>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                if (prefabRefRO.ValueRO.m_Prefab != scooterPrefabEntity)
                {
                    continue;
                }

                total++;

                bool isParked = SystemAPI.HasComponent<ParkedCar>(vehicleEntity);
                bool isActive = SystemAPI.HasComponent<CarCurrentLane>(vehicleEntity);

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
                    other++;
                }

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

                if (samples.Count == 0)
                {
                    sb.AppendLine("[FB] Scooter01 sample Index:Version = <none>");
                }
                else
                {
                    sb.Append("[FB] Scooter01 sample Index:Version = ");
                    for (int i = 0; i < samples.Count; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(FormatIndexVersion(samples[i])); // prints Index:Version for SE mod use.
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
                $"EffectivePathScalar={pathScalar:0.##}");

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
                .WithAll<PrefabData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                prefabs++;

                float currentMs = pathRO.ValueRO.m_SpeedLimit;

                prefabMin = math.min(prefabMin, currentMs);
                prefabMax = math.max(prefabMax, currentMs);

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

                float expectedMs = KmhToMs(pathPrefab.m_SpeedLimit) * math.max(0.01f, pathScalar);

                if (RelativeDiff(expectedMs, currentMs) > kPathMismatchPct)
                {
                    prefabMismatch++;

#if DEBUG
                    if (prefabMismatchExamples < 3)
                    {
                        prefabMismatchExamples++;

                        Mod.LogSafe(() =>
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
                .WithNone<Deleted, Temp, Overridden>())
            {
                comps++;

                float compMs = compRO.ValueRO.m_SpeedLimit;

                compMin = math.min(compMin, compMs);
                compMax = math.max(compMax, compMs);

                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!SystemAPI.HasComponent<PathwayData>(prefabEntity))
                {
                    compMissingPathwayData++;
                    continue;
                }

                float desiredMs = SystemAPI.GetComponent<PathwayData>(prefabEntity).m_SpeedLimit;

                if (math.abs(compMs - desiredMs) > kCompAbsMismatchMs)
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
