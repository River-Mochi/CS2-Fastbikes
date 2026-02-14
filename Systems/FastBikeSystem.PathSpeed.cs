// File: Systems/FastBikeSystem.PathSpeed.cs
// Purpose: Pathway speed-limit scaling (PathwayData + PathwayComposition) using PathwayPrefab authoring as baseline,
//          plus path-only runtime lane updates so already-placed paths get updated.
// CS2 authoring is km/h; runtime fields are m/s.

namespace FastBikes
{
    using Game.Common;        // Deleted, Overridden
    using Game.Net;           // Edge, Road, CarLane, SubLane
    using Game.Prefabs;       // PrefabBase, PrefabData, PrefabRef, PathwayPrefab, PathwayData, PathwayComposition, NetCompositionData
    using Game.Tools;         // Temp
    using Unity.Collections;  // NativeArray, NativeList, Allocator
    using Unity.Entities;     // Entity, DynamicBuffer, RefRO, RefRW, SystemAPI, BufferLookup, ComponentLookup
    using Unity.Mathematics;  // math.*

    public sealed partial class FastBikeSystem
    {
        // Budget for runtime edge processing per system update.
        private const int kEdgeBatchSize = 512;

        // Persistent snapshot of PATH edge entities only (roads excluded up front).
        private NativeArray<Entity> m_PathEdgeEntities;
        private int m_PathEdgeIndex;

        private void CreatePathQueries( )
        {
            // No cached EntityQuery required.
            // Path edge snapshot is built on-demand when BeginPathLaneBatch runs.
        }

        private bool IsPathBatchActive( )
        {
            return m_PathEdgeEntities.IsCreated;
        }

        private void DisposePathBatch( )
        {
            if (m_PathEdgeEntities.IsCreated)
            {
                m_PathEdgeEntities.Dispose();
                m_PathEdgeEntities = default; // Clear to avoid accidental reuse.
            }

            m_PathEdgeIndex = 0;
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
            foreach ((RefRW<Game.Prefabs.PathwayData> pathRW, Unity.Entities.Entity prefabEntity) in SystemAPI
                .Query<RefRW<Game.Prefabs.PathwayData>>()
                .WithAll<Game.Prefabs.PrefabData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .WithEntityAccess())
            {
                if (!TryGetPathwayBase(prefabEntity, out Game.Prefabs.PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                if (baseMs <= 0f)
                {
                    // Safety: no write when baseline is invalid.
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
                if (baseMs <= 0f)
                {
                    // Safety: no write when baseline is invalid.
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

        /// <summary>
        /// Builds a snapshot of runtime PATH edge entities only.
        /// Selection:
        /// - Edge has PrefabRef
        /// - Edge does not have Road
        /// - Edge prefab entity has PathwayData
        /// Lanes are collected later from each edge's SubLane buffer.
        /// </summary>
        private void BeginPathLaneBatch( )
        {
            DisposePathBatch();

            ComponentLookup<PathwayData> pathwayDataLookup =
                SystemAPI.GetComponentLookup<PathwayData>(isReadOnly: true);

            Unity.Collections.NativeList<Entity> pathEdges = new Unity.Collections.NativeList<Entity>(Unity.Collections.Allocator.Temp);

            try
            {
                foreach ((RefRO<PrefabRef> prefabRefRO, DynamicBuffer<Game.Net.SubLane> subLanes, Unity.Entities.Entity edgeEntity) in SystemAPI
                    .Query<RefRO<PrefabRef>, DynamicBuffer<Game.Net.SubLane>>()
                    .WithAll<Game.Net.Edge>()
                    .WithNone<Game.Net.Road>()
                    .WithNone<Deleted, Temp, Overridden>()
                    .WithEntityAccess())
                {
                    if (subLanes.Length == 0)
                    {
                        continue;
                    }

                    Unity.Entities.Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                    // Primary gate: path edges reference a prefab entity with PathwayData.
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

                m_PathEdgeEntities = new Unity.Collections.NativeArray<Entity>(pathEdges.Length, Unity.Collections.Allocator.Persistent);
                Unity.Collections.NativeArray<Entity>.Copy(pathEdges.AsArray(), m_PathEdgeEntities);
                m_PathEdgeIndex = 0;
            }
            finally
            {
                pathEdges.Dispose();    // Dispose temp list even if we early-out with no path edges found.
            }
        }

        /// <summary>
        /// Applies updated speed limits to runtime path lanes in small batches.
        /// Speed source of truth: PathwayData on the edge prefab entity.
        /// </summary>
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

            int count = math.min(kEdgeBatchSize, remaining);

            ComponentLookup<PrefabRef> prefabRefLookup =
                SystemAPI.GetComponentLookup<PrefabRef>(isReadOnly: true);

            ComponentLookup<PathwayData> pathwayDataLookup =
                SystemAPI.GetComponentLookup<PathwayData>(isReadOnly: true);

            BufferLookup<Game.Net.SubLane> subLaneLookup =
                SystemAPI.GetBufferLookup<Game.Net.SubLane>(isReadOnly: true);

            ComponentLookup<Game.Net.CarLane> laneLookup =
                SystemAPI.GetComponentLookup<Game.Net.CarLane>(isReadOnly: false);

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

                // Safety: never write 0 or negative speeds.
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

                    ref Game.Net.CarLane lane = ref laneLookup.GetRefRW(laneEntity).ValueRW;

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
