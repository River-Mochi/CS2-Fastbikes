// File: Systems/FastBikeSystem.BikeInstances.cs
// Purpose: Dump bicycle-group instance counts and sanity buckets for runtime state classification.
// Notes:
// - Read-only.


namespace FastBikes
{
    using Game.Common;            // Deleted, Destroyed, Overridden
    using Game.Objects;           // Moving, Stopped, Unspawned
    using Game.Prefabs;           // PrefabBase, PrefabData, PrefabRef, BicycleData
    using Game.Tools;             // Temp
    using Game.Vehicles;          // Car, CarCurrentLane, ParkedCar
    using System.Collections.Generic;
    using Unity.Entities;


    public sealed partial class FastBikeSystem
    {
        private const int kMaxSamplesPerBucket = 20;


        private void DumpCarGroupInstancesReport( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] CAR GROUP INSTANCES (LIVE) ====================\n" +
                "Meaning: live PersonalCar instances excluding BicycleData prefabs.\n" +
                "Live excludes: Deleted, Temp, Destroyed.\n" +
                "Status classification:\n" +
                "  Parked = ParkedCar\n" +
                "  Active = CarCurrentLane (Parked wins)\n" +
                "  Pending = neither ParkedCar nor CarCurrentLane (diagnostic only; not intended for UI totals)\n");

            // Build bicycle-group prefab set (BicycleData on prefab).
            var bikeGroupPrefabs = new HashSet<Entity>();

            foreach ((RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                bikeGroupPrefabs.Add(prefabEntity);
            }

            int allLiveTotal = 0;
            int parked = 0;
            int active = 0;
            int pending = 0;

            int statusTotal = 0; // parked + active only

            int unspawnedTotal = 0;
            int parkedAndUnspawned = 0;
            int activeAndUnspawned = 0;
            int pendingAndUnspawned = 0;

            int pendingAndAccident = 0;
            int movingNoLane = 0;
            int parkedAndLane = 0;

#if DEBUG
            const int kMaxSamples = 12;
            var sampleParkedUnspawned = new List<Entity>(kMaxSamples);
            var samplePending = new List<Entity>(kMaxSamples);
            var samplePendingAccident = new List<Entity>(kMaxSamples);
            var sampleMovingNoLane = new List<Entity>(kMaxSamples);
#endif

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity e) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.PersonalCar>()
                .WithNone<Deleted, Temp, Destroyed>()
                .WithEntityAccess())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                // Exclude bicycles + e-scooters (bike row owns those).
                if (bikeGroupPrefabs.Contains(prefabEntity))
                {
                    continue;
                }

                bool hasParked = SystemAPI.HasComponent<ParkedCar>(e);
                bool hasLane = SystemAPI.HasComponent<CarCurrentLane>(e);

                bool hasUnspawned = SystemAPI.HasComponent<Unspawned>(e);
                bool hasMoving = SystemAPI.HasComponent<Moving>(e);
                bool hasAccident = SystemAPI.HasComponent<Game.Events.InvolvedInAccident>(e);

                // Status-aligned classification (Parked wins)
                bool isParked = hasParked;
                bool isActive = !isParked && hasLane;
                bool isPending = !isParked && !isActive;

                allLiveTotal++;

                if (isParked)
                    parked++;
                else if (isActive)
                    active++;
                else
                    pending++;

                // UI-intended total (conservative): parked + active only.
                if (!isPending)
                    statusTotal++;

                if (hasUnspawned)
                {
                    unspawnedTotal++;

                    if (isParked)
                        parkedAndUnspawned++;
                    else if (isActive)
                        activeAndUnspawned++;
                    else
                        pendingAndUnspawned++;
                }

                if (hasParked && hasLane)
                {
                    parkedAndLane++;
                }

                // Diagnostic: cars that are moving but have no lane often correlate with accidents/cleanup oddities.
                if (hasMoving && !hasLane)
                {
                    movingNoLane++;
#if DEBUG
                    if (sampleMovingNoLane.Count < kMaxSamples)
                        sampleMovingNoLane.Add(e);
#endif
                }

                if (isPending && hasAccident)
                {
                    pendingAndAccident++;
#if DEBUG
                    if (samplePendingAccident.Count < kMaxSamples)
                        samplePendingAccident.Add(e);
#endif
                }

#if DEBUG
                if (isPending && samplePending.Count < kMaxSamples)
                    samplePending.Add(e);

                if (hasUnspawned && isParked && sampleParkedUnspawned.Count < kMaxSamples)
                    sampleParkedUnspawned.Add(e);
#endif
            }

            Mod.LogSafe(( ) =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[FB] CarGroup AllLive: Total={allLiveTotal}, Parked={parked}, Active={active}, Pending={pending}");
                sb.AppendLine($"[FB] CarGroup StatusTotal(Parked+Active)={statusTotal}");
                sb.AppendLine($"[FB] Unspawned: Total={unspawnedTotal} (Parked={parkedAndUnspawned}, Active={activeAndUnspawned}, Pending={pendingAndUnspawned})");
                sb.AppendLine($"[FB] Pending diagnostics: Pending&&InvolvedInAccident={pendingAndAccident}, Moving&&!CarCurrentLane={movingNoLane}");
                sb.AppendLine($"[FB] Sanity: ParkedCar&&CarCurrentLane={parkedAndLane}");

#if DEBUG
                sb.AppendLine("[FB] DEBUG Samples (Index:Version):");

                sb.Append("  Parked&&Unspawned: ");
                AppendSamples(sb, sampleParkedUnspawned);
                sb.AppendLine();

                sb.Append("  Pending: ");
                AppendSamples(sb, samplePending);
                sb.AppendLine();

                sb.Append("  Pending&&Accident: ");
                AppendSamples(sb, samplePendingAccident);
                sb.AppendLine();

                sb.Append("  MovingNoLane: ");
                AppendSamples(sb, sampleMovingNoLane);
                sb.AppendLine();
#endif

                return sb.ToString();
            });
        }


        // ====== BIKES Stuff ======
        private void DumpBikeInstancesReport( )
        {
            Mod.LogSafe(( ) =>
                "\n==================== [FB] BIKE INSTANCES (LIVE) ====================\n" +
                "Meaning: counts of live bicycle-group vehicles in the city (bikes + e-scooters).\n" +
                "Live excludes: Deleted, Temp, Destroyed.\n" +
                "Classification:\n" +
                "  Parked = ParkedCar\n" +
                "  Active = CarCurrentLane (Parked wins if both)\n" +
                "  Other  = neither ParkedCar nor CarCurrentLane\n" +
                "Sanity buckets log counts for unusual state combos.\n");

            // ------------------------------------------------------------
            // 1) Build bicycle-group prefab set (BicycleData on prefab).
            // ------------------------------------------------------------
            var groupPrefabs = new HashSet<Entity>();
            var scooterPrefabs = new HashSet<Entity>();

            int groupPrefabCount = 0;
            int missingPrefabBase = 0;

            foreach ((Unity.Entities.RefRO<PrefabData> _, Entity prefabEntity) in SystemAPI
                .Query<RefRO<PrefabData>>()
                .WithAll<Game.Prefabs.BicycleData>()
                .WithNone<Deleted, Temp, Overridden>()
                .WithEntityAccess())
            {
                groupPrefabCount++;
                groupPrefabs.Add(prefabEntity);

                if (!m_PrefabSystem.TryGetPrefab(prefabEntity, out PrefabBase prefabBase))
                {
                    missingPrefabBase++;
                    continue;
                }

                string name = prefabBase.name ?? string.Empty;
                if (IsScooterName(name))
                {
                    scooterPrefabs.Add(prefabEntity);
                }
            }

            if (groupPrefabs.Count == 0)
            {
                Mod.LogSafe(( ) => "[FB] Bike instances: BicycleData prefab set is empty.");
                return;
            }

            // ------------------------------------------------------------
            // 2) Count instances in one pass.
            // ------------------------------------------------------------
            int total = 0, active = 0, parked = 0, other = 0;

            int bikesTotal = 0, scootersTotal = 0;
            int bikesActive = 0, bikesParked = 0, bikesOther = 0;
            int scootersActive = 0, scootersParked = 0, scootersOther = 0;

            int unspawnedTotal = 0;
            int parkedUnspawned = 0;

            int parkedAndLane = 0;          // ParkedCar && CarCurrentLane
            int movingNoLane = 0;           // Moving && !CarCurrentLane
            int laneNoMoving = 0;           // CarCurrentLane && !Moving
            int parkedNotStopped = 0;       // ParkedCar && !Stopped
            int activeUnspawned = 0;        // CarCurrentLane && Unspawned
            int otherButMoving = 0;         // Other && Moving

#if DEBUG
            var sampleActive = new List<Entity>(kMaxSamplesPerBucket);
            var sampleParked = new List<Entity>(kMaxSamplesPerBucket);
            var sampleOther = new List<Entity>(kMaxSamplesPerBucket);

            var sampleParkedAndLane = new List<Entity>(kMaxSamplesPerBucket);
            var sampleMovingNoLane = new List<Entity>(kMaxSamplesPerBucket);
            var sampleParkedNotStopped = new List<Entity>(kMaxSamplesPerBucket);
            var sampleActiveUnspawned = new List<Entity>(kMaxSamplesPerBucket);
#endif

            foreach ((RefRO<PrefabRef> prefabRefRO, Entity e) in SystemAPI
                .Query<RefRO<PrefabRef>>()
                .WithAll<Game.Vehicles.Car>()
                .WithNone<Deleted, Temp, Game.Common.Destroyed>()
                .WithEntityAccess())
            {
                Entity prefabEntity = prefabRefRO.ValueRO.m_Prefab;

                if (!groupPrefabs.Contains(prefabEntity))
                {
                    continue;
                }

                bool isScooter = scooterPrefabs.Contains(prefabEntity);

                bool hasParked = SystemAPI.HasComponent<ParkedCar>(e);
                bool hasLane = SystemAPI.HasComponent<CarCurrentLane>(e);

                bool hasMoving = SystemAPI.HasComponent<Moving>(e);
                bool hasStopped = SystemAPI.HasComponent<Stopped>(e);
                bool hasUnspawned = SystemAPI.HasComponent<Unspawned>(e);

                // Status-aligned classification (Parked wins)
                bool isParked = hasParked;
                bool isActive = !isParked && hasLane;

                total++;

                if (isScooter)
                {
                    scootersTotal++;
                }
                else
                {
                    bikesTotal++;
                }

                if (isParked)
                {
                    parked++;

                    if (isScooter)
                        scootersParked++;
                    else
                        bikesParked++;
                }
                else if (isActive)
                {
                    active++;

                    if (isScooter)
                        scootersActive++;
                    else
                        bikesActive++;
                }
                else
                {
                    other++;

                    if (isScooter)
                        scootersOther++;
                    else
                        bikesOther++;
                }

                if (hasUnspawned)
                {
                    unspawnedTotal++;

                    if (isParked)
                    {
                        parkedUnspawned++;
                    }
                }

                if (hasParked && hasLane)
                {
                    parkedAndLane++;
#if DEBUG
                    if (sampleParkedAndLane.Count < kMaxSamplesPerBucket)
                        sampleParkedAndLane.Add(e);
#endif
                }

                if (hasMoving && !hasLane)
                {
                    movingNoLane++;
#if DEBUG
                    if (sampleMovingNoLane.Count < kMaxSamplesPerBucket)
                        sampleMovingNoLane.Add(e);
#endif
                }

                if (hasLane && !hasMoving)
                {
                    laneNoMoving++;
                }

                if (hasParked && !hasStopped)
                {
                    parkedNotStopped++;
#if DEBUG
                    if (sampleParkedNotStopped.Count < kMaxSamplesPerBucket)
                        sampleParkedNotStopped.Add(e);
#endif
                }

                if (hasLane && hasUnspawned)
                {
                    activeUnspawned++;
#if DEBUG
                    if (sampleActiveUnspawned.Count < kMaxSamplesPerBucket)
                        sampleActiveUnspawned.Add(e);
#endif
                }

                if (!isParked && !isActive && hasMoving)
                {
                    otherButMoving++;
                }

#if DEBUG
                if (isActive && sampleActive.Count < kMaxSamplesPerBucket)
                    sampleActive.Add(e);
                if (isParked && sampleParked.Count < kMaxSamplesPerBucket)
                    sampleParked.Add(e);
                if (!isParked && !isActive && sampleOther.Count < kMaxSamplesPerBucket)
                    sampleOther.Add(e);
#endif
            }

            Mod.LogSafe(( ) =>
            {
                var sb = new System.Text.StringBuilder();

                sb.AppendLine($"[FB] Bicycle-group prefabs: Count={groupPrefabCount}, MissingPrefabBase={missingPrefabBase}");

                sb.AppendLine($"[FB] BikeGroup: Total={total}, Active={active}, Parked={parked}, Other={other}");
                sb.AppendLine($"[FB] BikeGroup split: Bikes={bikesTotal} (A={bikesActive}, P={bikesParked}, O={bikesOther}), " +
                              $"Scooters={scootersTotal} (A={scootersActive}, P={scootersParked}, O={scootersOther})");

                sb.AppendLine($"[FB] Unspawned: TotalWithUnspawned={unspawnedTotal}, ParkedWithUnspawned={parkedUnspawned}");

                sb.AppendLine("---- Sanity buckets ----");
                sb.AppendLine($"[FB] ParkedCar && CarCurrentLane = {parkedAndLane}");
                sb.AppendLine($"[FB] Moving && !CarCurrentLane   = {movingNoLane}");
                sb.AppendLine($"[FB] CarCurrentLane && !Moving   = {laneNoMoving}");
                sb.AppendLine($"[FB] ParkedCar && !Stopped       = {parkedNotStopped}");
                sb.AppendLine($"[FB] CarCurrentLane && Unspawned = {activeUnspawned}");
                sb.AppendLine($"[FB] Other && Moving             = {otherButMoving}");

#if DEBUG
                sb.AppendLine("[FB] DEBUG Samples (Index:Version):");

                sb.Append("  Active: ");
                AppendSamples(sb, sampleActive);
                sb.AppendLine();

                sb.Append("  Parked: ");
                AppendSamples(sb, sampleParked);
                sb.AppendLine();

                sb.Append("  Other : ");
                AppendSamples(sb, sampleOther);
                sb.AppendLine();

                sb.Append("  Parked&&Lane: ");
                AppendSamples(sb, sampleParkedAndLane);
                sb.AppendLine();

                sb.Append("  MovingNoLane: ");
                AppendSamples(sb, sampleMovingNoLane);
                sb.AppendLine();

                sb.Append("  ParkedNotStopped: ");
                AppendSamples(sb, sampleParkedNotStopped);
                sb.AppendLine();

                sb.Append("  ActiveUnspawned: ");
                AppendSamples(sb, sampleActiveUnspawned);
                sb.AppendLine();
#endif

                return sb.ToString();
            });
        }

#if DEBUG
        private static void AppendSamples(System.Text.StringBuilder sb, List<Entity> samples)
        {
            if (samples.Count == 0)
            {
                sb.Append("<none>");
                return;
            }

            for (int i = 0; i < samples.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(FormatIndexVersion(samples[i]));
            }
        }
#endif
    }
}
