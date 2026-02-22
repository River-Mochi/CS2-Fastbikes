// File: Systems/FastBikeSystem.cs
// Purpose: On-demand bicycle tuning on prefab entities (speed + accel/brake scaling + swaying stability) + pathway speed scaling.

namespace FastBikes
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                           // GameSystemBase, GameMode
    using Game.Common;                    // Deleted, Overridden
    using Game.Prefabs;                   // PrefabSystem, PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData, SwayingData
    using Game.Tools;                     // Temp
    using System.Collections.Generic;     // Dictionary
    using Unity.Entities;                 // Entity, RefRW, SystemAPI
    using Unity.Mathematics;              // math.*

    public sealed partial class FastBikeSystem : GameSystemBase
    {
        // Dirty flags schedule work into the next OnUpdate.
        // System remains disabled when no work is queued.
        private bool m_BikeDirty;
        private bool m_StabilityDirty;
        private bool m_PathDirty;
        private bool m_ResetVanilla;

        private PrefabSystem m_PrefabSystem = null!;

        // Key = bicycle prefab entity, Value = captured baseline SwayingData for this load session.
        // Baseline must be per-load because prefab entities and data are recreated on world/city load.
        private readonly Dictionary<Entity, SwayingData> m_SwayingBaseline =
            new Dictionary<Entity, SwayingData>();

        protected override void OnCreate( )
        {
            base.OnCreate();

            // Run-once behavior: system stays disabled unless settings schedule work.
            Enabled = false;

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnDestroy( )
        {
            DisposePathBatch();

            base.OnDestroy();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            // City load recreates prefab entities and runtime lanes.
            // Cached baselines and batch snapshots must be cleared.
            m_SwayingBaseline.Clear();
            DisposePathBatch();

            // Apply current settings on each city load (including city switching without restarting).
            if (Mod.Settings != null)
            {
                ScheduleApplyAll();
            }
        }

        // --------------------------------------------------------------------
        // Scheduling
        // --------------------------------------------------------------------

        public void ScheduleApplyAll( )
        {
            m_BikeDirty = true;
            m_StabilityDirty = true;
            m_PathDirty = true;
            m_ResetVanilla = false;

            DisposePathBatch();

            Enabled = true;
        }

        public void ScheduleApplyBicyclesAndStability( )
        {
            m_BikeDirty = true;
            m_StabilityDirty = true;
            m_ResetVanilla = false;

            Enabled = true;
        }

        public void ScheduleApplyPaths( )
        {
            m_PathDirty = true;
            m_ResetVanilla = false;

            DisposePathBatch();

            Enabled = true;
        }

        public void ScheduleResetVanillaAll( )
        {
            m_BikeDirty = true;
            m_StabilityDirty = true;
            m_PathDirty = true;
            m_ResetVanilla = true;

            DisposePathBatch();

            Enabled = true;
        }

        // --------------------------------------------------------------------
        // Update
        // --------------------------------------------------------------------

        protected override void OnUpdate( )
        {
            // Fast exit: no scheduled work and no active batch.
            // Dump can run without apply (m_Dump lives in FastBikeSystem.Dump.cs).
            if (!m_BikeDirty && !m_StabilityDirty && !m_PathDirty && !m_ResetVanilla && !m_Dump && !IsPathBatchActive())
            {
                Enabled = false;
                return;
            }

            Setting? setting = Mod.Settings;
            if (setting == null)
            {
                // Settings not available: clear all work and stop.
                m_BikeDirty = false;
                m_StabilityDirty = false;
                m_PathDirty = false;
                m_ResetVanilla = false;
                m_Dump = false;

                DisposePathBatch();
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

                // Reset must win over all other state.
                // Disable (EnableFastBikes=false) also forces vanilla.
                bool forceVanilla = m_ResetVanilla || !setting.EnableFastBikes;
                m_ResetVanilla = false;

                float speedScalar = forceVanilla ? 1.0f : math.clamp(setting.SpeedScalar, 0.30f, 10.0f);
                float stiffnessScalar = forceVanilla ? 1.0f : math.clamp(setting.StiffnessScalar, 0.30f, 5.0f);
                float dampingScalar = forceVanilla ? 1.0f : math.clamp(setting.DampingScalar, 0.30f, 5.0f);

                // Bicycle tuning (prefab entity writes; no structural changes).
                if (m_BikeDirty)
                {
                    m_BikeDirty = false;
                    ApplyBicycleTuning(speedScalar);
                }

                // Stability (prefab entity writes; per-load baselines).
                if (m_StabilityDirty)
                {
                    m_StabilityDirty = false;
                    ApplyBicycleSwaying(forceVanilla, stiffnessScalar, dampingScalar);
                }

                // Path updates:
                // - Prefab + composition writes are done once per apply.
                // - Runtime lane speed updates are chunked over multiple updates using a persistent snapshot.
                if (m_PathDirty && !IsPathBatchActive())
                {
                    m_PathDirty = false;

                    float pathScalar = forceVanilla ? 1.0f : math.clamp(setting.PathSpeedScalar, 1.0f, 5.0f);
                    ApplyPathwayPrefabAndComposition(pathScalar);
                    BeginPathLaneBatch();
                }

                if (IsPathBatchActive())
                {
                    ContinuePathLaneBatch();
                }
            }
            catch (System.Exception ex)
            {
                Mod.WarnOnce("FB_SYSTEM_EXCEPTION", ( ) =>
                    $"[FB] FastBikeSystem failed: {ex.GetType().Name}: {ex.Message}");

                DisposePathBatch();
            }
            finally
            {
                // Disable until next explicit schedule, unless an active batch remains.
                if (!m_BikeDirty && !m_StabilityDirty && !m_PathDirty && !m_ResetVanilla && !m_Dump && !IsPathBatchActive())
                {
                    Enabled = false;
                }
                else
                {
                    Enabled = true;
                }
            }
        }

        /// <summary>Tunes CarData on bicycle prefab entities using BicyclePrefab authoring as baseline.</summary>
        private int ApplyBicycleTuning(float speedScalar)
        {
            float speed = Unity.Mathematics.math.max(0f, speedScalar);

            // Accel/brake scale uses sqrt() to avoid extreme values at high speed.
            float accelBrake = Unity.Mathematics.math.sqrt(Unity.Mathematics.math.max(0.01f, speed));

            int updated = 0;

            foreach ((RefRW<CarData> carRW, Entity prefabEntity) in SystemAPI.Query<RefRW<CarData>>()
                .WithAll<PrefabData, BicycleData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp, Game.Common.Overridden>()
                .WithEntityAccess())
            {
                if (!TryGetBicycleBase(prefabEntity, out BicyclePrefab bicyclePrefab))
                {
                    continue;
                }

                float baseMaxMs = bicyclePrefab.m_MaxSpeed * (1f / 3.6f);

                float newMaxSpeed = baseMaxMs <= 0f ? 0f : Unity.Mathematics.math.max(0.01f, baseMaxMs * speed);
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
            float stiff = Unity.Mathematics.math.max(0.01f, stiffnessScalar);
            float damp = Unity.Mathematics.math.max(0.01f, dampingScalar);

            int updated = 0;

            foreach ((RefRW<SwayingData> swayRW, Entity prefabEntity) in SystemAPI.Query<RefRW<SwayingData>>()
                .WithAll<PrefabData, BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                SwayingData current = swayRW.ValueRO;

                // Baseline capture: first seen value becomes baseline for this load session.
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

                // Higher stiffness reduces sway amplitude.
                tuned.m_MaxPosition = baseline.m_MaxPosition / stiff;

                // Higher damping settles sway faster.
                tuned.m_DampingFactors = baseline.m_DampingFactors / damp;
                tuned.m_DampingFactors = Unity.Mathematics.math.clamp(tuned.m_DampingFactors, 0.01f, 0.999f);

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
