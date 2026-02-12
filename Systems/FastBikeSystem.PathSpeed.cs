// File: Systems/FastBikeSystem.PathSpeed.cs
// Purpose: pathway speed-limit scaling (PathwayData + PathwayComposition + existing lane CarLane) using PathwayPrefab authoring as baseline.
// CS2 Authoring is km/h; runtime fields are m/s.

namespace FastBikes
{
    using Game.Common;              // Deleted, Owner
    using Game.Prefabs;             // PrefabBase, PrefabData, PrefabRef, PathwayPrefab, PathwayData, PathwayComposition, NetCompositionData
    using Game.Tools;               // Temp
    using System.Collections.Generic; // Dictionary
    using Unity.Collections;        // NativeArray, Allocator
    using Unity.Entities;           // Entity, EntityQuery, RefRO, RefRW, SystemAPI
    using Unity.Mathematics;        // math.*

    public sealed partial class FastBikeSystem
    {
        // Budget for runtime lane updates per system update.
        private const int kLaneBatchSize = 2048;

        private EntityQuery m_PathLaneQuery;

        private NativeArray<Entity> m_PathLaneEntities;
        private int m_PathLaneIndex;
        private float m_PathLaneScalar;

        private readonly Dictionary<Entity, float> m_PathDesiredMsCache =
            new Dictionary<Entity, float>();

        private void CreatePathQueries()
        {
            // Cached lane query used only when PathSpeedScalar changes (snapshot + batch update).
            // QueryBuilder avoids EntityQueryDesc/ComponentType arrays and keeps generator-safe type names.
            m_PathLaneQuery = SystemAPI.QueryBuilder()
                .WithAll<Game.Net.CarLane, Game.Common.Owner>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .Build();
        }

        private bool IsPathBatchActive()
        {
            return m_PathLaneEntities.IsCreated;
        }

        private void DisposePathBatch()
        {
            if (m_PathLaneEntities.IsCreated)
            {
                m_PathLaneEntities.Dispose();
            }

            m_PathLaneIndex = 0;
            m_PathLaneScalar = 1.0f;
            m_PathDesiredMsCache.Clear();
        }

        /// <summary>
        /// Updates pathway speed limits on:
        /// - PathwayData (prefab entity runtime data)
        /// - PathwayComposition (net composition copies used by net systems)
        /// </summary>
        private int ApplyPathwayPrefabAndComposition(float scalar)
        {
            float s = math.max(0.01f, scalar);
            int updated = 0;

            // 1) PathwayData on pathway prefab entities.
            foreach ((RefRW<PathwayData> pathRW, Entity prefabEntity) in SystemAPI
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
                .Query<RefRW<PathwayComposition>, RefRO<PrefabRef>>()
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

        private void BeginPathLaneBatch(float scalar)
        {
            DisposePathBatch();

            m_PathLaneScalar = math.max(0.01f, scalar);

            // Snapshot lanes once per apply, then update in small batches.
            m_PathLaneEntities = m_PathLaneQuery.ToEntityArray(Allocator.Persistent);
            m_PathLaneIndex = 0;
        }

        private void ContinuePathLaneBatch()
        {
            if (!m_PathLaneEntities.IsCreated)
            {
                return;
            }

            int remaining = m_PathLaneEntities.Length - m_PathLaneIndex;
            if (remaining <= 0)
            {
                DisposePathBatch();
                return;
            }

            int count = math.min(kLaneBatchSize, remaining);

            ComponentLookup<Game.Net.CarLane> laneLookup = SystemAPI.GetComponentLookup<Game.Net.CarLane>(isReadOnly: false);
            ComponentLookup<Game.Common.Owner> ownerLookup = SystemAPI.GetComponentLookup<Game.Common.Owner>(isReadOnly: true);
            ComponentLookup<PrefabRef> prefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<PathwayData> pathwayDataLookup = SystemAPI.GetComponentLookup<PathwayData>(isReadOnly: true);

            for (int i = 0; i < count; i++)
            {
                Entity laneEntity = m_PathLaneEntities[m_PathLaneIndex++];

                if (!ownerLookup.HasComponent(laneEntity))
                {
                    continue;
                }

                Entity ownerEntity = ownerLookup[laneEntity].m_Owner;

                if (!prefabRefLookup.HasComponent(ownerEntity))
                {
                    continue;
                }

                Entity prefabEntity = prefabRefLookup[ownerEntity].m_Prefab;

                float desiredMs = GetDesiredPathSpeedMs(prefabEntity, pathwayDataLookup, m_PathLaneScalar);

                if (!laneLookup.HasComponent(laneEntity))
                {
                    continue;
                }

                ref Game.Net.CarLane lane = ref laneLookup.GetRefRW(laneEntity).ValueRW;

                if (lane.m_SpeedLimit != desiredMs || lane.m_DefaultSpeedLimit != desiredMs)
                {
                    lane.m_SpeedLimit = desiredMs;
                    lane.m_DefaultSpeedLimit = desiredMs;
                }
            }

            if (m_PathLaneIndex >= m_PathLaneEntities.Length)
            {
                DisposePathBatch();
            }
        }

        private float GetDesiredPathSpeedMs(Entity prefabEntity, ComponentLookup<PathwayData> pathwayDataLookup, float scalar)
        {
            if (pathwayDataLookup.HasComponent(prefabEntity))
            {
                return pathwayDataLookup[prefabEntity].m_SpeedLimit;
            }

            if (m_PathDesiredMsCache.TryGetValue(prefabEntity, out float cached))
            {
                return cached;
            }

            float desiredMs = 0f;

            if (TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
            {
                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                desiredMs = (baseMs <= 0f) ? 0f : baseMs * math.max(0.01f, scalar);
            }

            m_PathDesiredMsCache[prefabEntity] = desiredMs;
            return desiredMs;
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
