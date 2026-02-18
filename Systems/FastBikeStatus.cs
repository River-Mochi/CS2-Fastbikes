// File: Systems/FastBikeStatus.cs
// Purpose: UI-facing cached status lines for Options UI.
// Notes:
// - Refresh driven by Setting.cs getters (runs only while Options is open).
// - Cache invalidates on main-menu <-> city transitions (prevents stale after city switching).
// - Localization + formatting safety is handled here; FastBikeStatusSystem returns raw numbers only.

namespace FastBikes
{
    using CS2HonuShared;          // LocaleUtils
    using Game;                   // IsGame()
    using Game.SceneFlow;         // GameManager
    using System;                 // DateTime, TimeSpan
    using Unity.Entities;         // World
    using UnityEngine;            // Time.frameCount

    public static class FastBikeStatus
    {
        public static int RefreshIntervalSeconds { get; set; } = 10;

        internal const string KeyStatusNotLoaded = "FAST_STATUS_NOT_LOADED";
        internal const string KeyStatsNotAvail = "FAST_STATS_NOT_AVAIL";
        internal const string KeyCarsNotAvail = "FAST_STATS_CARS_NOT_AVAIL";
        internal const string KeyBikesRow = "FAST_STATS_BIKES_ROW1";
        internal const string KeyCarsRow = "FAST_STATS_CARS_ROW2";

        private const string FallbackStatusNotLoaded = "Status not loaded.";
        private const string FallbackStatsNotAvail = "No city... ¯\\_(ツ)_/¯ ...No stats";
        private const string FallbackCarsNotAvail = "run the city a few minutes for data.";

        private const string FallbackBikesRow =
            "{0} active | {1} bikes | {2} e-scooter | {3} / {4} parked/total";

        private const string FallbackCarsRow =
            "{0} active | {1} parked | {2} total | updated {3}";

        public static string BikesRow { get; private set; } = string.Empty;
        public static string CarsRow { get; private set; } = string.Empty;

        private static bool s_WasInGame;
        private static bool s_HasSnapshotThisCity;
        private static long s_LastRefreshTicksUtc;
        private static int s_LastUiFrame = -1;

        public static void InvalidateCache( )
        {
            s_HasSnapshotThisCity = false;
            s_LastRefreshTicksUtc = 0;
            s_LastUiFrame = -1;

            BikesRow = LocaleUtils.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            CarsRow = LocaleUtils.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
        }

        public static void MarkDirty( )
        {
            s_HasSnapshotThisCity = false;
            s_LastRefreshTicksUtc = 0;
        }

        public static void RefreshIfNeeded( )
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return;
            }

            int frame = Time.frameCount;
            if (frame == s_LastUiFrame)
            {
                return;
            }

            s_LastUiFrame = frame;

            if (string.IsNullOrEmpty(BikesRow))
            {
                BikesRow = LocaleUtils.SafeFormat(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }

            if (string.IsNullOrEmpty(CarsRow))
            {
                CarsRow = LocaleUtils.SafeFormat(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }

            GameManager gm = GameManager.instance;
            bool isGame = (gm != null && gm.gameMode.IsGame());

            if (isGame != s_WasInGame)
            {
                s_WasInGame = isGame;
                InvalidateCache();
            }

            if (!isGame)
            {
                BikesRow = LocaleUtils.SafeFormat(KeyStatsNotAvail, FallbackStatsNotAvail);
                CarsRow = LocaleUtils.SafeFormat(KeyCarsNotAvail, FallbackCarsNotAvail);
                return;
            }

            long nowUtc = DateTime.UtcNow.Ticks;

            if (!s_HasSnapshotThisCity)
            {
                BuildSnapshotSafe(world);
                s_HasSnapshotThisCity = true;
                s_LastRefreshTicksUtc = nowUtc;
                return;
            }

            int interval = RefreshIntervalSeconds;
            if (interval < 1)
            {
                interval = 5;
            }

            long nextAllowed = s_LastRefreshTicksUtc + TimeSpan.FromSeconds(interval).Ticks;
            if (nowUtc < nextAllowed)
            {
                return;
            }

            BuildSnapshotSafe(world);
            s_LastRefreshTicksUtc = nowUtc;
        }

        private static void BuildSnapshotSafe(World world)
        {
            try
            {
                BuildAndApplySnapshot(world);
            }
            catch
            {
                BikesRow = LocaleUtils.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
                CarsRow = LocaleUtils.Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            }
        }

        private static void BuildAndApplySnapshot(World world)
        {
            FastBikeStatusSystem sys = world.GetOrCreateSystemManaged<FastBikeStatusSystem>();
            FastBikeStatusSystem.Snapshot snap = sys.BuildSnapshot();

            BikesRow = LocaleUtils.SafeFormat(
                KeyBikesRow,
                fallbackFormat: FallbackBikesRow,
                LocaleUtils.FormatN0(snap.BikeGroupActive),    // {0}
                LocaleUtils.FormatN0(snap.BikeOnlyTotal),      // {1}
                LocaleUtils.FormatN0(snap.ScooterTotal),       // {2}
                LocaleUtils.FormatN0(snap.BikeGroupParked),    // {3}
                LocaleUtils.FormatN0(snap.BikeGroupTotal)      // {4}
            );

            string updated = snap.SnapshotTimeLocal.ToString("HH:mm:ss");

            CarsRow = LocaleUtils.SafeFormat(
                KeyCarsRow,
                fallbackFormat: FallbackCarsRow,
                LocaleUtils.FormatN0(snap.CarGroupActive),     // {0}
                LocaleUtils.FormatN0(snap.CarGroupParked),     // {1}
                LocaleUtils.FormatN0(snap.CarGroupTotal),      // {2}
                updated                                        // {3}
            );
        }
    }
}
