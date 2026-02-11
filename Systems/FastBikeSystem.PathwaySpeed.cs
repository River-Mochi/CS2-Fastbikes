// File: Systems/FastBikeSystem.PathwaySpeed.cs
// Purpose: pathway speed-limit scaling (PathwayData + PathwayComposition + existing lane CarLane) using PathwayPrefab authoring as baseline.
// CS2 Authoring is km/h; runtime fields are m/s.

namespace FastBikes
{
    using Game.Common;      // Deleted, Owner
    using Game.Prefabs;     // PrefabSystem, PrefabBase, PathwayPrefab, PathwayData, PathwayComposition, PrefabData, PrefabRef, NetCompositionData
    using Game.Tools;       // Temp
    using Unity.Entities;   // Entity, RefRO, RefRW, SystemAPI
    using Unity.Mathematics;// math.*

    public sealed partial class FastBikeSystem
    {
        /// <summary>
        /// Tunes pathway speed limits on:
        /// - PathwayData (prefab entity runtime data)
        /// - PathwayComposition (net composition copies used by net systems)
        /// - Game.Net.CarLane (existing runtime lane entities) to avoid 30 km/h caps persisting on placed networks
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
            foreach ((RefRW<PathwayComposition> compRW, RefRO<PrefabRef> prefabRefRO) in SystemAPI
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

            // 3) Existing runtime lane entities frequently keep old limits (Game.Net.CarLane),
            // which can cause bikes to keep capping at 30 km/h even after prefab/composition updates.
            int laneUpdated = 0;

            foreach ((RefRW<Game.Net.CarLane> laneRW, RefRO<Owner> ownerRO) in SystemAPI
                .Query<RefRW<Game.Net.CarLane>, RefRO<Game.Common.Owner>>()
                .WithNone<Deleted, Temp>())
            {
                Entity ownerEntity = ownerRO.ValueRO.m_Owner;

                if (!SystemAPI.HasComponent<PrefabRef>(ownerEntity))
                {
                    continue;
                }

                PrefabRef pref = SystemAPI.GetComponent<PrefabRef>(ownerEntity);
                Entity prefabEntity = pref.m_Prefab;

                float desiredMs;

                if (SystemAPI.HasComponent<PathwayData>(prefabEntity))
                {
                    desiredMs = SystemAPI.GetComponent<PathwayData>(prefabEntity).m_SpeedLimit;
                }
                else
                {
                    if (!TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
                    {
                        continue;
                    }

                    float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                    desiredMs = (baseMs <= 0f) ? 0f : baseMs * s;
                }

                ref Game.Net.CarLane lane = ref laneRW.ValueRW;

                if (lane.m_SpeedLimit != desiredMs || lane.m_DefaultSpeedLimit != desiredMs)
                {
                    lane.m_SpeedLimit = desiredMs;
                    lane.m_DefaultSpeedLimit = desiredMs;
                    laneUpdated++;
                }
            }

            updated += laneUpdated;
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
