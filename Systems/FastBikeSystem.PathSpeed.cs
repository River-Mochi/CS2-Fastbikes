// File: Systems/FastBikeSystem.PathSpeed.cs
// Purpose: Pathway speed-limit scaling for:
//          - Prefab layer: PathwayData + PathwayComposition (affects newly drawn paths)
//          - Runtime layer: existing placed path lanes (Game.Net.CarLane) via path-only edges
// Notes:
// - Authoring speed limits are km/h; runtime values are m/s.

namespace FastBikes
{
    using Game.Common;               // Deleted, Overridden
    using Game.Net;                  // Edge, Road, CarLane, SubLane
    using Game.Prefabs;              // PathwayPrefab, PathwayData, PathwayComposition, PrefabData, PrefabRef, NetCompositionData
    using Game.Tools;                // Temp
    using Unity.Collections;         // NativeArray, NativeList, Allocator
    using Unity.Entities;            // Entity, RefRO, RefRW, SystemAPI, BufferLookup, ComponentLookup
    using Unity.Mathematics;         // math.*

    public sealed partial class FastBikeSystem
    {
        private const int kEdgeBatchSize = 4096;

        private NativeArray<Entity> m_PathEdgeEntities;
        private int m_PathEdgeIndex;

        private bool IsPathBatchActive( )
        {
            return m_PathEdgeEntities.IsCreated;
        }

        private void DisposePathBatch( )
        {
            if (m_PathEdgeEntities.IsCreated)
            {
                m_PathEdgeEntities.Dispose();
                m_PathEdgeEntities = default;
            }

            m_PathEdgeIndex = 0;
        }

        private int ApplyPathwayPrefabAndComposition(float scalar)
        {
            float s = Unity.Mathematics.math.max(0.01f, scalar);
            int updated = 0;

            // Prefab layer: PathwayData (m/s). Affects newly drawn paths after Apply.
            foreach ((RefRW<PathwayData> pathRW, Entity prefabEntity) in SystemAPI
                .Query<RefRW<PathwayData>>()
                .WithAll<PrefabData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                if (!TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                if (baseMs <= 0f)
                {
                    continue;
                }

                float newMs = baseMs * s;

                ref PathwayData data = ref pathRW.ValueRW;
                if (data.m_SpeedLimit != newMs)
                {
                    data.m_SpeedLimit = newMs;
                    updated++;
                }
            }

            // Composition layer: PathwayComposition (m/s). Keep aligned with prefab layer.
            foreach ((RefRW<PathwayComposition> compRW, RefRO<PrefabRef> prefabRefRO) in SystemAPI
                .Query<RefRW<PathwayComposition>, RefRO<PrefabRef>>()
                .WithAll<NetCompositionData>()
                .WithNone<Deleted, Temp, Overridden>())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!TryGetPathwayBase(prefabEntity, out PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                if (baseMs <= 0f)
                {
                    continue;
                }

                float newMs = baseMs * s;

                ref PathwayComposition comp = ref compRW.ValueRW;
                if (comp.m_SpeedLimit != newMs)
                {
                    comp.m_SpeedLimit = newMs;
                    updated++;
                }
            }

            return updated;
        }

        private void BeginPathLaneBatch( )
        {
            DisposePathBatch();

            ComponentLookup<PathwayData> pathwayDataLookup = SystemAPI.GetComponentLookup<PathwayData>(isReadOnly: true);

            using NativeList<Entity> pathEdges = new NativeList<Entity>(Allocator.Temp);
            foreach ((RefRO<PrefabRef> prefabRefRO, DynamicBuffer<Game.Net.SubLane> subLanes, Entity edgeEntity) in SystemAPI
                .Query<RefRO<PrefabRef>, DynamicBuffer<Game.Net.SubLane>>()
                .WithAll<Edge>()
                .WithNone<Road>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                if (subLanes.Length == 0)
                {
                    continue;
                }

                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                // Path-only gate: paths have PathwayData; roads do not.
                if (!pathwayDataLookup.HasComponent(prefabEntity))
                {
                    continue;
                }

                pathEdges.Add(edgeEntity);
            }

            if (pathEdges.Length == 0)
            {
                return;
            }

            m_PathEdgeEntities = new NativeArray<Entity>(pathEdges.Length, Allocator.Persistent);
            NativeArray<Entity>.Copy(pathEdges.AsArray(), m_PathEdgeEntities);
            m_PathEdgeIndex = 0;
        }

        private void ContinuePathLaneBatch( )
        {
            if (!m_PathEdgeEntities.IsCreated)
            {
                return;
            }

            int remaining = m_PathEdgeEntities.Length - m_PathEdgeIndex;
            if (remaining <= 0)
            {
                DisposePathBatch();
                return;
            }

            int count = Unity.Mathematics.math.min(kEdgeBatchSize, remaining);

            ComponentLookup<PrefabRef> prefabRefLookup = SystemAPI.GetComponentLookup<PrefabRef>(isReadOnly: true);
            ComponentLookup<PathwayData> pathwayDataLookup = SystemAPI.GetComponentLookup<PathwayData>(isReadOnly: true);
            BufferLookup<Game.Net.SubLane> subLaneLookup = SystemAPI.GetBufferLookup<Game.Net.SubLane>(isReadOnly: true);
            ComponentLookup<Game.Net.CarLane> laneLookup = SystemAPI.GetComponentLookup<Game.Net.CarLane>(isReadOnly: false);

            for (int i = 0; i < count; i++)
            {
                Entity edgeEntity = m_PathEdgeEntities[m_PathEdgeIndex++];

                if (!prefabRefLookup.HasComponent(edgeEntity))
                {
                    continue;
                }

                if (!subLaneLookup.HasBuffer(edgeEntity))
                {
                    continue;
                }

                Entity prefabEntity = prefabRefLookup[edgeEntity].m_Prefab;

                if (!pathwayDataLookup.HasComponent(prefabEntity))
                {
                    continue;
                }

                float desiredMs = pathwayDataLookup[prefabEntity].m_SpeedLimit;
                if (desiredMs <= 0f)
                {
                    continue;
                }

                DynamicBuffer<Game.Net.SubLane> subLanes = subLaneLookup[edgeEntity];

                for (int j = 0; j < subLanes.Length; j++)
                {
                    Entity laneEntity = subLanes[j].m_SubLane;

                    if (!laneLookup.HasComponent(laneEntity))
                    {
                        continue;
                    }

                    RefRW<Game.Net.CarLane> laneRW = laneLookup.GetRefRW(laneEntity);
                    ref Game.Net.CarLane lane = ref laneRW.ValueRW;

                    if (lane.m_SpeedLimit != desiredMs)
                    {
                        lane.m_SpeedLimit = desiredMs;
                    }

                    if (lane.m_DefaultSpeedLimit != desiredMs)
                    {
                        lane.m_DefaultSpeedLimit = desiredMs;
                    }
                }
            }

            if (m_PathEdgeIndex >= m_PathEdgeEntities.Length)
            {
                DisposePathBatch();
            }
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
