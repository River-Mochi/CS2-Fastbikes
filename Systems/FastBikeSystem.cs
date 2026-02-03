// File: Systems/FastBikeSystem.cs
// Purpose: On-demand bicycle tuning on prefab entities (speed + gentle accel/brake scaling).
// Notes:
// - Runs only on-demand (settings change or game load), then disables itself.
// - Reads TRUE vanilla baselines from PrefabSystem -> BicyclePrefab authoring (NOT PrefabRef data).
// - Writes changes to Game.Prefabs.CarData on bicycle prefab entities.

namespace FastBikes
{
    using Colossal.Serialization.Entities; // Purpose
    using CS2HonuShared;                
    using Game;                           // GameSystemBase, GameMode
    using Game.Prefabs;                   // PrefabSystem, PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData
    using Unity.Entities;                 // Entity, RefRW, SystemAPI
    using Unity.Mathematics;              // math.*

    public sealed partial class FastBikeSystem : GameSystemBase
    {
        private bool m_Dirty;
        private PrefabSystem m_PrefabSystem = null!;

        protected override void OnCreate()
        {
            base.OnCreate();

            Enabled = false;
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            if (Mod.Settings != null)
            {
                ScheduleReapply();
            }
        }

        public void ScheduleReapply()
        {
            m_Dirty = true;
            Enabled = true;
        }

        protected override void OnUpdate()
        {
            if (!m_Dirty)
            {
                Enabled = false;
                return;
            }

            m_Dirty = false;

            Setting? setting = Mod.Settings;
            if (setting == null)
            {
                Enabled = false;
                return;
            }

            float speedScalar = 1f;

            if (setting.EnableFastBikes)
            {
                speedScalar = math.clamp(setting.BikeSpeedScalar * 0.01f, 0.5f, 20f);
            }

            try
            {
                ApplyBicycleTuning(speedScalar);
            }
            catch (System.Exception ex)
            {
                Mod.WarnOnce("FB_SYSTEM_EXCEPTION", () =>
                    $"[FB] Apply failed: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                Enabled = false;
            }
        }

        private void ApplyBicycleTuning(float speedScalar)
        {
            float clampedSpeedScalar = math.max(0f, speedScalar);
            float accelBrakeScalar = math.sqrt(math.max(0.01f, clampedSpeedScalar));

            foreach ((RefRW<CarData> car, Entity prefabEntity) in SystemAPI
                         .Query<RefRW<CarData>>()
                         .WithAll<PrefabData, BicycleData>()
                         .WithEntityAccess())
            {
                if (!TryGetBicycleBase(prefabEntity, out BicyclePrefab bicyclePrefab))
                {
                    continue;
                }

                // Authoring max speed is km/h; runtime CarData max speed is m/s.
                float baseMaxSpeedMs = bicyclePrefab.m_MaxSpeed * (1f / 3.6f);

                CarData tuned = car.ValueRO;

                tuned.m_MaxSpeed = baseMaxSpeedMs <= 0f
                    ? 0f
                    : math.max(0.01f, baseMaxSpeedMs * clampedSpeedScalar);

                tuned.m_Acceleration = bicyclePrefab.m_Acceleration <= 0f
                    ? 0f
                    : bicyclePrefab.m_Acceleration * accelBrakeScalar;

                tuned.m_Braking = bicyclePrefab.m_Braking <= 0f
                    ? 0f
                    : bicyclePrefab.m_Braking * accelBrakeScalar;

                car.ValueRW = tuned;
            }
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
