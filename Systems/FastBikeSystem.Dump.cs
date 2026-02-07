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

        public void ScheduleDump()
        {
            m_Dump = true;
            Enabled = true; // OnUpdate() will run once and call DumpBicyclePrefabs(...)
        }

        private static float MsToKmh(float ms) => ms * 3.6f;
        private static float MsToMph(float ms) => ms * 2.23693629f;
        private static float KmhToMs(float kmh) => kmh * (1f / 3.6f);
        private static float KmhToMph(float kmh) => kmh * 0.621371f;

        private static bool IsScooterName(string name)
        {
            // Current vanilla naming: ElectricScooter01. Prefix match keeps this future-proof.
            return !string.IsNullOrEmpty(name) &&
                   name.StartsWith("ElectricScooter", StringComparison.OrdinalIgnoreCase);
        }

        private void DumpBicyclePrefabs(bool enableFastBikes, float speedScalar, float stiffnessScalar, float dampingScalar)
        {
            // These match the system’s apply behavior: when OFF, effective scalars are 1.0.
            float effectiveSpeed = enableFastBikes ? math.clamp(speedScalar, 0.30f, 10.0f) : 1.0f;
            float effectiveAccelBrake = math.sqrt(math.max(0.01f, effectiveSpeed)); // same curve as ApplyBicycleTuning()

            Mod.LogSafe(() =>
                $"[FB] Dump start. EnableFastBikes={enableFastBikes}, " +
                $"Speed={speedScalar:0.##} (Effective={effectiveSpeed:0.##}), " +
                $"Stiffness={stiffnessScalar:0.##}, Damping={dampingScalar:0.##}");

            int total = 0;
            int bikes = 0;
            int scooters = 0;

            // Prefab entities only. Excludes Temp/Deleted to avoid odd transitional entities.
            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI.Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                total++;

                string name = "(PrefabBase not found)";
                string typeName = "(unknown)";

                if (m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    name = prefabBase.name;
                    typeName = prefabBase.GetType().Name;
                }

                bool isScooter = IsScooterName(name);
                if (isScooter) { scooters++; }
                else { bikes++; }

                string kind = isScooter ? "Scooter" : "Bicycle";

                Mod.LogSafe(() =>
                    $"[FB] {kind} prefab: entity={prefabEntity.Index}:{prefabEntity.Version} name='{name}' type={typeName}");

                // Need PrefabBase to read BicyclePrefab authoring numbers (max speed/accel/brake/turning).
                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase2))
                {
                    Mod.LogSafe(() => "[FB]   Authoring: (PrefabBase not found)");
                    continue;
                }

                if (prefabBase2 is BicyclePrefab bike)
                {
                    // Authoring: km/h. Runtime CarData: m/s.
                    float baseMaxMs = KmhToMs(bike.m_MaxSpeed);

                    // “Expected” is what ApplyBicycleTuning() should produce for this session’s scalars.
                    float expectedMaxMs = baseMaxMs * effectiveSpeed;
                    float expectedAccel = bike.m_Acceleration * effectiveAccelBrake;
                    float expectedBrake = bike.m_Braking * effectiveAccelBrake;

                    Mod.LogSafe(() =>
                        $"[FB]   Authoring: MaxSpeed={bike.m_MaxSpeed:0.###} km/h ({KmhToMph(bike.m_MaxSpeed):0.###} mph)  " +
                        $"Accel={bike.m_Acceleration:0.###}  Brake={bike.m_Braking:0.###}  " +
                        $"Turning=({bike.m_Turning.x:0.###},{bike.m_Turning.y:0.###})  Stiffness={bike.m_Stiffness:0.###}");

                    Mod.LogSafe(() =>
                        $"[FB]   Expected: MaxSpeed≈{expectedMaxMs:0.###} m/s ({MsToMph(expectedMaxMs):0.###} mph)  " +
                        $"Accel≈{expectedAccel:0.###}  Brake≈{expectedBrake:0.###}");

                    // Current prefab-entity CarData (what the game is using right now).
                    if (SystemAPI.HasComponent<CarData>(prefabEntity))
                    {
                        CarData car = SystemAPI.GetComponent<CarData>(prefabEntity);

                        float carMs = car.m_MaxSpeed;
                        Mod.LogSafe(() =>
                            $"[FB]   CarData: MaxSpeed={carMs:0.###} m/s ({MsToKmh(carMs):0.###} km/h, {MsToMph(carMs):0.###} mph)  " +
                            $"Accel={car.m_Acceleration:0.###}  Brake={car.m_Braking:0.###}");
                    }
                    else
                    {
                        Mod.LogSafe(() => "[FB]   CarData: (missing)");
                    }
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   Authoring: (PrefabBase is not BicyclePrefab)");
                }

                // SwayingData lives on the prefab entity. Baseline is cached on first-seen per entity each load session.
                if (SystemAPI.HasComponent<SwayingData>(prefabEntity))
                {
                    SwayingData sw = SystemAPI.GetComponent<SwayingData>(prefabEntity);

                    bool cached = m_SwayingBaseline.TryGetValue(prefabEntity, out SwayingData baseSw);
                    if (!cached)
                    {
                        baseSw = sw; // baseline not cached yet in this load session
                    }

                    Mod.LogSafe(() =>
                        $"[FB]   Sway baseline: Cached={cached}  MaxPos={baseSw.m_MaxPosition}  " +
                        $"Damping={baseSw.m_DampingFactors}  Spring={baseSw.m_SpringFactors}");

                    Mod.LogSafe(() =>
                        $"[FB]   Sway current : MaxPos={sw.m_MaxPosition}  " +
                        $"Damping={sw.m_DampingFactors}  Spring={sw.m_SpringFactors}");
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   SwayingData: (missing)");
                }
            }

            Mod.LogSafe(() =>
                $"[FB] Dump complete. Total={total} (Bicycles={bikes}, Scooters={scooters})");
        }
    }
}
