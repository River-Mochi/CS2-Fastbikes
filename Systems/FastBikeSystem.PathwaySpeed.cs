// File: Systems/FastBikeSystem.PathwaySpeed.cs
// Purpose: pathway speed-limit scaling (PathwayData) using PathwayPrefab authoring as baseline.

namespace FastBikes
{
    using Game.Common;    // Deleted
    using Game.Prefabs;   // PrefabSystem, PrefabBase, PathwayPrefab, PathwayData, PrefabData
    using Game.Tools;     // Temp
    using Unity.Entities; // Entity, RefRW, SystemAPI
    using Unity.Mathematics; // math.*

    public sealed partial class FastBikeSystem
    {
        /// <summary>
        /// Tunes PathwayData speed limits on pathway prefab entities using PathwayPrefab authoring as baseline.
        /// Authoring is km/h; PathwayData is runtime m/s.
        /// </summary>
        private int ApplyPathwaySpeedLimit(float scalar)
        {
            float s = math.max(0.01f, scalar);
            int updated = 0;

            foreach ((RefRW<PathwayData> pathRW, Entity prefabEntity) in SystemAPI.Query<RefRW<PathwayData>>()
                .WithAll<PrefabData>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                if (!TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                float newMs = baseMs <= 0f ? 0f : baseMs * s;

                ref PathwayData data = ref pathRW.ValueRW;
                if (data.m_SpeedLimit != newMs)
                {
                    data.m_SpeedLimit = newMs;
                    updated++;
                }
            }

            return updated;
        }

        private bool TryGetPathwayBase(Entity prefabEntity, out PathwayPrefab pathwayPrefab)
        {
            pathwayPrefab = default!;

            if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
            {
                return false;
            }

            if (prefabBase is PathwayPrefab path)
            {
                pathwayPrefab = path;
                return true;
            }

            return false;
        }
    }
}
