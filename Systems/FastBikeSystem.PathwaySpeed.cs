// File: Systems/FastBikeSystem.PathwaySpeed.cs
// Purpose: pathway speed-limit scaling (PathwayData + PathwayComposition) using PathwayPrefab authoring as baseline.

namespace FastBikes
{
    using Game.Common;              // Deleted
    using Game.Prefabs;             // PrefabSystem, PrefabBase, PathwayPrefab, PathwayData, PathwayComposition, PrefabData, PrefabRef, NetCompositionData
    using Game.Tools;               // Temp
    using Unity.Entities;           // Entity, RefRO, RefRW, SystemAPI
    using Unity.Mathematics;        // math.*

    public sealed partial class FastBikeSystem
    {
        /// <summary>
        /// Tunes pathway speed limits on:
        /// - PathwayData (prefab entity runtime data)
        /// - PathwayComposition (net composition copies used by net systems)
        /// Authoring is km/h; runtime fields are m/s.
        /// </summary>
        private int ApplyPathwaySpeedLimit(float scalar)
        {
            float s = math.max(0.01f, scalar);
            int updated = 0;

            // 1) PathwayData on pathway prefab entities.
            foreach (var (pathRW, prefabEntity) in SystemAPI
                .Query<RefRW<PathwayData>>()
                .WithAll<PrefabData>()
                .WithNone<Deleted, Temp>()
                .WithEntityAccess())
            {
                if (!TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                float newMs = (baseMs <= 0f) ? 0f : baseMs * s;

                ref PathwayData data = ref pathRW.ValueRW;
                if (data.m_SpeedLimit != newMs)
                {
                    data.m_SpeedLimit = newMs;
                    updated++;
                }
            }

            // 2) PathwayComposition on composition entities.
            // WithEntityAccess removed: query yields a 2-tuple (compRW, prefabRefRO).
            foreach (var (compRW, prefabRefRO) in SystemAPI
                .Query<RefRW<Game.Prefabs.PathwayComposition>, RefRO<PrefabRef>>()
                .WithAll<NetCompositionData>()
                .WithNone<Deleted, Temp>())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                float newMs = (baseMs <= 0f) ? 0f : baseMs * s;

                ref PathwayComposition comp = ref compRW.ValueRW;
                if (comp.m_SpeedLimit != newMs)
                {
                    comp.m_SpeedLimit = newMs;
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
