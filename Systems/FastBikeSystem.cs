// File: Systems/FastBikeSystem.cs
// Purpose: On-demand bicycle tuning on prefab entities (speed + accel/brake scaling + swaying handling).
// Notes:
// - Runs only on-demand (when settings change or on game load), then disables itself.
// - Reads vanilla baselines from PrefabSystem -> BicyclePrefab authoring (not from *Data) for speed/accel/brake.
// - Adjusts SwayingData on bicycle prefab entities using captured baseline values (patch-resilient and preserves scooter vs bike differences).
// - Touches only entities with BicycleData to avoid affecting non-bicycle swaying (props, water, etc.).

namespace FastBikes
{
    using Colossal.Serialization.Entities; // Purpose
    using Game;                           // GameSystemBase, GameMode
    using Game.Prefabs;                   // PrefabSystem, PrefabBase, BicyclePrefab, BicycleData, CarData, PrefabData, SwayingData
    using System.Collections.Generic;     // Dictionary
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
        private EntityQuery m_BicycleSwayQuery;
        private EntityQuery m_BicyclePrefabQuery;

        private readonly Dictionary<Entity, SwayingData> m_SwayingBaseline =
            new Dictionary<Entity, SwayingData>();

        protected override void OnCreate()
        {
            base.OnCreate();

            Enabled = false;

            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            // Prefab entities that represent bicycles and also have CarData.
            m_BicycleCarQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabData, BicycleData, CarData>()
                .Build();

            // Prefab entities that represent bicycles and have SwayingData.
            m_BicycleSwayQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabData, BicycleData, SwayingData>()
                .Build();

            // Prefab entities that represent bicycles (for dump).
            m_BicyclePrefabQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabData, BicycleData>()
                .Build();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);

            m_SwayingBaseline.Clear();

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
                    DumpBicyclePrefabs(setting.VerboseLogging);
                }

                bool forceVanilla = m_ResetVanilla || !setting.EnableFastBikes;
                m_ResetVanilla = false;

                float speedScalar = forceVanilla
                    ? 1.0f
                    : math.clamp(setting.SpeedScalar, 0.30f, 10.0f);

                float stiffnessScalar = forceVanilla
                    ? 1.0f
                    : math.clamp(setting.StiffnessScalar, 0.50f, 10.0f);

                float dampingScalar = forceVanilla
                    ? 1.0f
                    : math.clamp(setting.DampingScalar, 0.50f, 10.0f);

                ApplyBicycleTuning(speedScalar);
                ApplyBicycleSwaying(forceVanilla, stiffnessScalar, dampingScalar);

                if (setting.VerboseLogging)
                {
                    Mod.LogSafe(() =>
                        $"[FB] Applied. Enabled={setting.EnableFastBikes}, Speed={speedScalar:0.##}x, Stiffness={stiffnessScalar:0.##}x, Damping={dampingScalar:0.##}x");
                }
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

        private void ApplyBicycleSwaying(bool forceVanilla, float stiffnessScalar, float dampingScalar)
        {
            float stiff = math.max(0.01f, stiffnessScalar);
            float damp = math.max(0.01f, dampingScalar);

            using NativeArray<Entity> entities = m_BicycleSwayQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity prefabEntity = entities[i];

                SwayingData current = EntityManager.GetComponentData<SwayingData>(prefabEntity);

                if (!m_SwayingBaseline.TryGetValue(prefabEntity, out SwayingData baseline))
                {
                    baseline = current;
                    m_SwayingBaseline.Add(prefabEntity, baseline);
                }

                if (forceVanilla)
                {
                    if (!current.Equals(baseline))
                    {
                        EntityManager.SetComponentData(prefabEntity, baseline);
                    }
                    continue;
                }

                SwayingData tuned = baseline;

                // StiffnessScalar: higher -> less lean/sway (smaller max position).
                tuned.m_MaxPosition = baseline.m_MaxPosition / stiff;

                // DampingScalar: higher -> settles faster (smaller damping factors before pow()).
                tuned.m_DampingFactors = baseline.m_DampingFactors / damp;

                EntityManager.SetComponentData(prefabEntity, tuned);
            }
        }

        private void DumpBicyclePrefabs(bool verbose)
        {
            using NativeArray<Entity> entities = m_BicyclePrefabQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++)
            {
                Entity prefabEntity = entities[i];

                string name = "(PrefabBase not found)";
                string typeName = "(unknown)";

                if (m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    name = prefabBase.name;
                    typeName = prefabBase.GetType().Name;
                }

                Mod.LogSafe(() =>
                    $"[FB] Bicycle prefab: entity={prefabEntity.Index}:{prefabEntity.Version} name='{name}' type={typeName}");

                if (!verbose)
                {
                    continue;
                }

                if (EntityManager.HasComponent<CarData>(prefabEntity))
                {
                    CarData car = EntityManager.GetComponentData<CarData>(prefabEntity);
                    Mod.LogSafe(() =>
                        $"[FB]   CarData: MaxSpeed={car.m_MaxSpeed:0.###} m/s  Accel={car.m_Acceleration:0.###}  Brake={car.m_Braking:0.###}");
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   CarData: (missing)");
                }

                if (EntityManager.HasComponent<SwayingData>(prefabEntity))
                {
                    SwayingData sw = EntityManager.GetComponentData<SwayingData>(prefabEntity);

                    if (!m_SwayingBaseline.TryGetValue(prefabEntity, out SwayingData baseSw))
                    {
                        baseSw = sw;
                    }

                    Mod.LogSafe(() =>
                        $"[FB]   SwayingData baseline: MaxPos={baseSw.m_MaxPosition}  Damping={baseSw.m_DampingFactors}  Spring={baseSw.m_SpringFactors}");
                    Mod.LogSafe(() =>
                        $"[FB]   SwayingData current : MaxPos={sw.m_MaxPosition}  Damping={sw.m_DampingFactors}  Spring={sw.m_SpringFactors}");
                }
                else
                {
                    Mod.LogSafe(() => "[FB]   SwayingData: (missing)");
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
