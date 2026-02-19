// File: Systems/FastBikeSystem.PathSpeed.cs
// Purpose: Pathway speed-limit scaling for:
//          - Prefab layer: PathwayData + PathwayComposition (affects newly drawn paths)
//          - Runtime layer: existing placed path lanes (Game.Net.CarLane) via path-only edges
// Notes:
// - Authoring speed limits are km/h; runtime values are m/s.

namespace FastBikes
{
    using Game.Common;        // Deleted, Overridden
    using Game.Net;           // Edge, Road, CarLane, SubLane
    using Game.Prefabs;       // PathwayPrefab, PathwayData, PathwayComposition, NetCompositionData
    using Game.Tools;         // Temp
    using System.Collections.Generic; // HashSet<>, List<>
    using Unity.Collections;  // NativeArray, NativeList, Allocator
    using Unity.Entities;     // RefRO, RefRW, SystemAPI, BufferLookup, ComponentLookup
    using Unity.Mathematics;  // math.*

    public sealed partial class FastBikeSystem
    {
        // Runtime update budget per frame (keeps Apply from doing a huge spike in one OnUpdate).
        private const int kEdgeBatchSize = 4096;

        // Persistent snapshot of *path edges only* (roads excluded before snapshot is stored).
        private NativeArray<Unity.Entities.Entity> m_PathEdgeEntities;
        private int m_PathEdgeIndex;

        private bool IsPathBatchActive( )
        {
            return m_PathEdgeEntities.IsCreated;
        }

        private void DisposePathBatch( )
        {
            // Persistent allocations must be released when:
            // - a new apply starts
            // - the batch completes
            // - the world unloads / system destroys
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

            // -------------------------------
            // Prefab layer: PathwayData
            // -------------------------------
            // Updates prefab-entity PathwayData.m_SpeedLimit (m/s).
            // Only affects NEW placed path segments after Apply.
            foreach ((Unity.Entities.RefRW<Game.Prefabs.PathwayData> pathRW, Entity prefabEntity) in SystemAPI
                .Query<RefRW<PathwayData>>()
                .WithAll<PrefabData>()
                .WithNone<Deleted, Temp>()
                .WithNone<Overridden>()
                .WithEntityAccess())
            {
                // Baseline comes from authoring PathwayPrefab (km/h).
                if (!TryGetPathwayBase(prefabEntity, out Game.Prefabs.PathwayPrefab pathwayPrefab))
                {
                    continue;
                }

                float baseMs = pathwayPrefab.m_SpeedLimit * (1f / 3.6f);
                if (baseMs <= 0f)
                {
                    continue; // Safety: never write invalid/zero speed limits.
                }

                float newMs = baseMs * s;

                ref PathwayData data = ref pathRW.ValueRW;
                if (data.m_SpeedLimit != newMs)
                {
                    data.m_SpeedLimit = newMs;
                    updated++;
                }
            }

            // -------------------------------
            // Prefab composition layer: PathwayComposition
            // -------------------------------
            // Net composition entities keep a copied speed limit (m/s) used by net systems.
            // Keeping this aligned avoids “prefab says X but composition says Y” drift.
            foreach ((Unity.Entities.RefRW<Game.Prefabs.PathwayComposition> compRW, Unity.Entities.RefRO<Game.Prefabs.PrefabRef> prefabRefRO) in SystemAPI
                .Query<Unity.Entities.RefRW<Game.Prefabs.PathwayComposition>, Unity.Entities.RefRO<Game.Prefabs.PrefabRef>>()
                .WithAll<Game.Prefabs.NetCompositionData>()
                .WithNone<Game.Common.Deleted, Game.Tools.Temp>()
                .WithNone<Game.Common.Overridden>())
            {
                Unity.Entities.Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

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

                ref Game.Prefabs.PathwayComposition comp = ref compRW.ValueRW;
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
            // New ApplyPaths request: discard any in-progress batch first.
            DisposePathBatch();

            // Lookup used to classify an edge as a path:
            // edge PrefabRef.m_Prefab must have PathwayData.
            Unity.Entities.ComponentLookup<Game.Prefabs.PathwayData> pathwayDataLookup =
                SystemAPI.GetComponentLookup<PathwayData>(isReadOnly: true);

            // Temp list here only used during snapshot construction (same frame).
            Unity.Collections.NativeList<Entity> pathEdges =
                new Unity.Collections.NativeList<Entity>(Unity.Collections.Allocator.Temp);

            try
            {
                // 1) Start at runtime edges, not lanes:
                //    - Edge container owns the SubLane buffer.
                // 2) Exclude runtime Roads early WithNone<Game.Net.Road>.
                // 3) Require the edge prefab to be a path prefab (PathwayData gate).
                foreach ((Unity.Entities.RefRO<Game.Prefabs.PrefabRef> prefabRefRO, Unity.Entities.DynamicBuffer<Game.Net.SubLane> subLanes, Unity.Entities.Entity edgeEntity) in SystemAPI
                    .Query<Unity.Entities.RefRO<Game.Prefabs.PrefabRef>, Unity.Entities.DynamicBuffer<Game.Net.SubLane>>()
                    .WithAll<Game.Net.Edge>()
                    .WithNone<Road>()
                    .WithNone<Deleted, Temp, Overridden>()
                    .WithEntityAccess())
                {
                    // No sublanes -> nothing to update.
                    if (subLanes.Length == 0)
                    {
                        continue;
                    }

                    // PrefabRef on the edge points to the prefab entity describing this edge type.
                    Unity.Entities.Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                    // Path-only gate: paths have PathwayData; roads do not.
                    if (!pathwayDataLookup.HasComponent(prefabEntity))
                    {
                        continue;
                    }

                    // Edge is confirmed as “path edge”; store it for batched processing.
                    pathEdges.Add(edgeEntity);
                }

                if (pathEdges.Length == 0)
                {
                    return;
                }

                // Persistent snapshot: required because ContinuePathLaneBatch consumes this across multiple OnUpdate calls.
                m_PathEdgeEntities = new Unity.Collections.NativeArray<Unity.Entities.Entity>(pathEdges.Length, Unity.Collections.Allocator.Persistent);
                Unity.Collections.NativeArray<Unity.Entities.Entity>.Copy(pathEdges.AsArray(), m_PathEdgeEntities);
                m_PathEdgeIndex = 0;
            }
            finally
            {
                pathEdges.Dispose(); // Always dispose Temp list (even on early return or exceptions).
            }
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

            // Process a slice of edges this frame.
            int count = Unity.Mathematics.math.min(kEdgeBatchSize, remaining);

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

                // Defensive: edge must still have PrefabRef and SubLane buffer.
                if (!prefabRefLookup.HasComponent(edgeEntity))
                {
                    continue;
                }

                if (!subLaneLookup.HasBuffer(edgeEntity))
                {
                    continue;
                }

                Unity.Entities.Entity prefabEntity = prefabRefLookup[edgeEntity].m_Prefab;

                // Redundant safety: edge prefab must still be a path prefab.
                if (!pathwayDataLookup.HasComponent(prefabEntity))
                {
                    continue;
                }

                // Desired speed comes from prefab layer (PathwayData already scaled during ApplyPathwayPrefabAndComposition).
                float desiredMs = pathwayDataLookup[prefabEntity].m_SpeedLimit;

                // Safety: do not write invalid/zero speeds into runtime lanes.
                if (desiredMs <= 0f)
                {
                    continue;
                }

                // Edge -> sublanes -> lane entities.
                Unity.Entities.DynamicBuffer<Game.Net.SubLane> subLanes = subLaneLookup[edgeEntity];

                for (int j = 0; j < subLanes.Length; j++)
                {
                    Unity.Entities.Entity laneEntity = subLanes[j].m_SubLane;

                    // Only update lanes that with CarLane component.
                    if (!laneLookup.HasComponent(laneEntity))
                    {
                        continue;
                    }

                    RefRW<Game.Net.CarLane> laneRW = laneLookup.GetRefRW(laneEntity);
                    // Writable ref to the CarLane component stored in ECS chunk memory (in-place writes, no copy).
                    ref Game.Net.CarLane lane = ref laneRW.ValueRW;

                    // Write both current and default so the lane stays consistent.
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

            // Batch complete.
            if (m_PathEdgeIndex >= m_PathEdgeEntities.Length)
            {
                DisposePathBatch();
            }
        }

        private bool TryGetPathwayBase(Entity prefabEntity, out PathwayPrefab pathwayPrefab)
        {
            pathwayPrefab = default!;

            // Returns PrefabBase (source of truth baseline).
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
