// File: Systems/FastBikeSystem.cs
// Purpose: On-demand bicycle tuning on prefab entities (speed + accel/brake scaling).
// Notes:
// - Runs only on-demand (when settings change or on game load), then disables itself.
// - Reads vanilla baselines from PrefabSystem -> BicyclePrefab authoring (not from *Data).
// - Writes changes to Game.Prefabs.CarData on bicycle prefab entities.
// - Handling sliders are saved/UI-visible but not applied until the swaying component is confirmed.

namespace FastBikes
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                           // GameSystemBase, GameMode
    using Game.Prefabs;                   // PrefabSystem, PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData
    using Unity.Collections;              // Allocator, NativeArray
    using Unity.Entities;                 // Entity, EntityQuery, SystemAPI
    using Unity.Mathematics;              // math.*

    public sealed partial class FastBikeSystem : GameSystemBase
    {
        private bool m_Dirty;
        private bool m_ResetVanilla;
        private bool m_Dump;

        private PrefabSystem m_PrefabSystem = null!;

        private EntityQuery m_BicycleCarQuery;
        private EntityQuery m_BicyclePrefabQuery;

        protected override void OnCreate()
        {
            base.OnCreate();

            Enabled = false;

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Prefab entities that represent bicycles and also have CarData.
            m_BicycleCarQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabData, BicycleData, CarData>()
                .Build();

            // Prefab entities that represent bicycles (for dump).
            m_BicyclePrefabQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabData, BicycleData>()
                .Build();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

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

        public void ScheduleDump()
        {
            m_Dirty = true;
            m_Dump = true;
            Enabled = true;
        }

        // Older name kept for compatibility with earlier drafts.
        public void ScheduleReapply()
        {
            ScheduleApply();
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

            try
            {
                if (m_Dump)
                {
                    m_Dump = false;
                    DumpBicyclePrefabs();
                }

                bool forceVanilla = m_ResetVanilla || !setting.EnableFastBikes;
                m_ResetVanilla = false;

                float speedScalar = 1f;

                if (!forceVanilla)
                {
                    // SpeedScalar is percent (100 = vanilla).
                    // Slider range in Setting.cs is 50..300 => 0.5x..3.0x.
                    speedScalar = math.clamp(setting.SpeedScalar * 0.01f, 0.5f, 3.0f);
                }

                ApplyBicycleTuning(speedScalar);

                bool anyHandlingNonVanilla =
                    setting.StiffnessScalar != 100 ||
                    setting.SpringScalar != 100 ||
                    setting.DampingScalar != 100;

                if (!forceVanilla && anyHandlingNonVanilla)
                {
                    Mod.WarnOnce("FB_HANDLING_NOT_WIRED", () =>
                        "[FB] Handling sliders are saved and shown in UI, but handling tuning is not applied yet (pending confirmed swaying component/fields).");
                }

#if DEBUG
                if (setting.VerboseLogging)
                {
                    Mod.LogSafe(() => $"[FB] Applied. Enabled={setting.EnableFastBikes}, SpeedScalar={speedScalar:0.###}x");
                }
#endif
            }
            catch (System.Exception ex)
            {
                Mod.WarnOnce("FB_SYSTEM_EXCEPTION", () =>
                    $"[FB] FastBikeSystem failed: {ex.GetType().Name}: {ex.Message}");
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

            using NativeArray<Entity> entities = m_BicycleCarQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity prefabEntity = entities[i];

                if (!TryGetBicycleBase(prefabEntity, out BicyclePrefab bicyclePrefab))
                {
                    continue;
                }

                // Authoring max speed is km/h; runtime CarData max speed is m/s.
                float baseMaxSpeedMs = bicyclePrefab.m_MaxSpeed * (1f / 3.6f);

                CarData car = EntityManager.GetComponentData<CarData>(prefabEntity);

                car.m_MaxSpeed = baseMaxSpeedMs <= 0f
                    ? 0f
                    : math.max(0.01f, baseMaxSpeedMs * clampedSpeedScalar);

                car.m_Acceleration = bicyclePrefab.m_Acceleration <= 0f
                    ? 0f
                    : bicyclePrefab.m_Acceleration * accelBrakeScalar;

                car.m_Braking = bicyclePrefab.m_Braking <= 0f
                    ? 0f
                    : bicyclePrefab.m_Braking * accelBrakeScalar;

                EntityManager.SetComponentData(prefabEntity, car);
            }
        }

        private void DumpBicyclePrefabs()
        {
            using NativeArray<Entity> entities = m_BicyclePrefabQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity prefabEntity = entities[i];

                if (m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    Mod.LogSafe(() => $"[FB] Bicycle prefab: entity={prefabEntity.Index}:{prefabEntity.Version} name='{prefabBase.name}' type={prefabBase.GetType().Name}");
                }
                else
                {
                    Mod.LogSafe(() => $"[FB] Bicycle prefab: entity={prefabEntity.Index}:{prefabEntity.Version} (PrefabBase not found)");
                }
            }

            Mod.LogSafe(() => $"[FB] Dump complete. BicycleData prefabs={entities.Length}");
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
