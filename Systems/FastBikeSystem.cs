// File: Systems/FastBikeSystem.cs
// Purpose: On-demand bicycle tuning on prefab entities (speed + accel/brake scaling + swaying stability).

namespace FastBikes
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                           // GameSystemBase, GameMode
    using Game.Common;                    // Deleted
    using Game.Prefabs;                   // PrefabSystem, PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData, SwayingData
    using Game.Tools;                     // Temp
    using System.Collections.Generic;     // Dictionary
    using Unity.Entities;                 // Entity, RefRW, SystemAPI
    using Unity.Mathematics;              // math.*

    public sealed partial class FastBikeSystem : GameSystemBase
    {
        private bool m_Dirty;
        private bool m_ResetVanilla;

        private PrefabSystem m_PrefabSystem = null!;

        // Key = bicycle prefab entity, Value = captured "vanilla base" SwayingData for this load session.
        private readonly Dictionary<Entity, SwayingData> m_SwayingBaseline =
            new Dictionary<Entity, SwayingData>();

        protected override void OnCreate()
        {
            base.OnCreate();

            // On-demand only: system stays disabled unless a setting change or debug dump is requested.
            Enabled = false;
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            // Parameters are required by the override; no special behavior needed per mode/purpose.
            m_SwayingBaseline.Clear(); // recapture per load session

            if (Mod.Settings != null)
            {
                ScheduleApply();
            }
        }

        public void ScheduleApply()
        {
            m_Dirty = true;
            m_ResetVanilla = false;
            Enabled = true;
        }

        public void ScheduleResetVanilla()
        {
            m_Dirty = true;
            m_ResetVanilla = true;
            Enabled = true;
        }

        protected override void OnUpdate()
        {
            // Dump can run without apply (m_Dump lives in FastBikeSystem.Dump.cs).
            if (!m_Dirty && !m_Dump)
            {
                Enabled = false;
                return;
            }

            Setting? setting = Mod.Settings;
            if (setting == null)
            {
                m_Dirty = false;
                m_ResetVanilla = false;
                m_Dump = false;
                Enabled = false;
                return;
            }

            try
            {
                if (m_Dump)
                {
                    m_Dump = false;

                    DumpBicyclePrefabs(
                        enableFastBikes: setting.EnableFastBikes,
                        speedScalar: setting.SpeedScalar,
                        stiffnessScalar: setting.StiffnessScalar,
                        dampingScalar: setting.DampingScalar);
                }

                if (!m_Dirty)
                {
                    return;
                }

                m_Dirty = false;

                bool forceVanilla = m_ResetVanilla || !setting.EnableFastBikes;
                m_ResetVanilla = false;

                float speedScalar = forceVanilla ? 1.0f : math.clamp(setting.SpeedScalar, 0.30f, 10.0f);
                float stiffnessScalar = forceVanilla ? 1.0f : math.clamp(setting.StiffnessScalar, 0.30f, 5.0f);
                float dampingScalar = forceVanilla ? 1.0f : math.clamp(setting.DampingScalar, 0.30f, 5.0f);

                int tunedCars = ApplyBicycleTuning(speedScalar);
                int tunedSway = ApplyBicycleSwaying(forceVanilla, stiffnessScalar, dampingScalar);

#if DEBUG
                Mod.LogSafe(() =>
                    $"[FB] Applied. Enabled={setting.EnableFastBikes}, Speed={speedScalar:0.##}x, " +
                    $"Stiffness={stiffnessScalar:0.##}x, Damping={dampingScalar:0.##}x, " +
                    $"CarDataUpdated={tunedCars}, SwayUpdated={tunedSway}");
#endif
            }
            catch (System.Exception ex)
            {
                Mod.WarnOnce("FB_SYSTEM_EXCEPTION", () =>
                    $"[FB] FastBikeSystem failed: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                // Run-once behavior: disable until the next explicit schedule.
                Enabled = false;
            }
        }

        /// <summary>Tunes CarData on bicycle prefab entities using BicyclePrefab authoring as baseline.</summary>
        private int ApplyBicycleTuning(float speedScalar)
        {
            float speed = math.max(0f, speedScalar);                // top speed uses scalar directly
            float accelBrake = math.sqrt(math.max(0.01f, speed));   // accel/brake use sqrt(scalar)

            int updated = 0;

            foreach ((RefRW<CarData> carRW, Entity prefabEntity) in SystemAPI.Query<RefRW<CarData>>()
                .WithAll<PrefabData, BicycleData>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                if (!TryGetBicycleBase(prefabEntity, out BicyclePrefab bicyclePrefab))
                {
                    continue;
                }

                float baseMaxMs = bicyclePrefab.m_MaxSpeed * (1f / 3.6f); // authoring km/h -> runtime m/s

                float newMaxSpeed = baseMaxMs <= 0f ? 0f : math.max(0.01f, baseMaxMs * speed);
                float newAccel = bicyclePrefab.m_Acceleration <= 0f ? 0f : bicyclePrefab.m_Acceleration * accelBrake;
                float newBrake = bicyclePrefab.m_Braking <= 0f ? 0f : bicyclePrefab.m_Braking * accelBrake;

                ref CarData car = ref carRW.ValueRW;

                if (car.m_MaxSpeed != newMaxSpeed || car.m_Acceleration != newAccel || car.m_Braking != newBrake)
                {
                    car.m_MaxSpeed = newMaxSpeed;
                    car.m_Acceleration = newAccel;
                    car.m_Braking = newBrake;
                    updated++;
                }
            }

            return updated;
        }

        /// <summary>Tunes SwayingData on bicycle prefab entities using cached per-prefab baselines.</summary>
        private int ApplyBicycleSwaying(bool forceVanilla, float stiffnessScalar, float dampingScalar)
        {
            float stiff = math.max(0.01f, stiffnessScalar);
            float damp = math.max(0.01f, dampingScalar);

            int updated = 0;

            foreach ((RefRW<SwayingData> swayRW, Entity prefabEntity) in SystemAPI.Query<RefRW<SwayingData>>()
                .WithAll<PrefabData, BicycleData>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                SwayingData current = swayRW.ValueRO;

                // Baseline = first-seen SwayingData for this prefab entity after load.
                if (!m_SwayingBaseline.TryGetValue(prefabEntity, out SwayingData baseline))
                {
                    baseline = current;
                    m_SwayingBaseline.Add(prefabEntity, baseline);
                }

                if (forceVanilla)
                {
                    if (!current.Equals(baseline))
                    {
                        swayRW.ValueRW = baseline;
                        updated++;
                    }
                    continue;
                }

                SwayingData tuned = baseline;

                tuned.m_MaxPosition = baseline.m_MaxPosition / stiff;        // higher stiffness -> smaller lean distance
                tuned.m_DampingFactors = baseline.m_DampingFactors / damp;   // higher damping -> settles faster
                tuned.m_DampingFactors = math.clamp(tuned.m_DampingFactors, 0.01f, 0.999f); // safety clamp

                if (!tuned.Equals(current))
                {
                    swayRW.ValueRW = tuned;
                    updated++;
                }
            }

            return updated;
        }

        private bool TryGetBicycleBase(Entity prefabEntity, out BicyclePrefab bicyclePrefab)
        {
            bicyclePrefab = default!;

            // Safe way to read default BicyclePrefab fields (m_MaxSpeed, m_Acceleration, m_Braking).
            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
            {
                return false;
            }

            if (prefabBase is BicyclePrefab bike)
            {
                bicyclePrefab = bike;
                return true;
            }

            return false;
        }
    }
}
