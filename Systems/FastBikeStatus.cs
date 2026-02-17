// File: Systems/FastBikeStatus.cs
// Purpose: UI-facing cached status lines for Options UI.
// Notes:
// - Refresh driven by Setting.cs getters (runs only while Options is open).
// - Cache invalidates on main-menu <-> city transitions (prevents stale after city switching).
// - Localization + formatting safety is handled here; FastBikeStatusSystem returns raw numbers only.

namespace FastBikes
{
    using Colossal.Localization;
    using Game;               // IsGame()
    using Game.SceneFlow;     // GameManager
    using System;             // DateTime, TimeSpan, Math, Exception, FormatException
    using Unity.Entities;     // World
    using UnityEngine;        // Time.frameCount

    public static class FastBikeStatus
    {
        // Throttle refresh while Options UI is open.
        // NOTE: Do not set to 0 (would refresh every UI poll).
        public static int RefreshIntervalSeconds { get; set; } = 10;

        internal const string KeyStatusNotLoaded = "FAST_STATUS_NOT_LOADED";
        internal const string KeyStatsNotAvail = "FAST_STATS_NOT_AVAIL";
        internal const string KeyBikesRow = "FAST_STATS_BIKES_ROW1";
        internal const string KeyCarsRow = "FAST_STATS_CARS_ROW2";

        private const string FallbackStatusNotLoaded = "Status not loaded.";
        private const string FallbackStatsNotAvail = "No city... ¯\\_(ツ)_/¯ ...No stats";

        private const string FallbackBikesRow =
            "{0}% ({1}) bikes | {2}% ({3}) scooter | {4} / {5} parked/total";

        private const string FallbackCarsRow =
            "{0}% ({1}) runs | {2} / {3} parked/total | updated {5}";


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

            BikesRow = Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
            CarsRow = string.Empty;
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

            // Frame guard: multiple Setting.cs getters can call this in the same UI draw.
            int frame = Time.frameCount;
            if (frame == s_LastUiFrame)
            {
                return;
            }

            s_LastUiFrame = frame;

            if (string.IsNullOrEmpty(BikesRow))
            {
                BikesRow = Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
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
                BikesRow = Localize(KeyStatsNotAvail, FallbackStatsNotAvail);
                CarsRow = Localize(KeyStatsNotAvail, FallbackStatsNotAvail);
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
                BikesRow = Localize(KeyStatusNotLoaded, FallbackStatusNotLoaded);
                CarsRow = string.Empty;
            }
        }

        private static void BuildAndApplySnapshot(World world)
        {
            FastBikeStatusSystem sys = world.GetOrCreateSystemManaged<FastBikeStatusSystem>();
            FastBikeStatusSystem.Snapshot snap = sys.BuildSnapshot();

            long total = snap.TotalPersonalVehicles;

            int pctBikes = Percent(snap.BikeOnlyTotal, total);
            int pctScooters = Percent(snap.ScooterTotal, total);
            int pctCarRuns = Percent(snap.CarGroupRunning, total);
            int pctCarParked = Percent(snap.CarGroupParked, total);

            string updated = snap.SnapshotTimeLocal.ToString("HH:mm:ss");

            BikesRow = SafeFormat(
                KeyBikesRow,
                fallbackFormat: FallbackBikesRow,
                pctBikes,                     // {0}
                Format0(snap.BikeOnlyTotal),  // {1}
                pctScooters,                  // {2}
                Format0(snap.ScooterTotal),   // {3}
                Format0(snap.BikeGroupParked),// {4}
                Format0(snap.BikeGroupTotal)  // {5}
            );

            CarsRow = SafeFormat(
                KeyCarsRow,
                fallbackFormat: FallbackCarsRow,
                pctCarRuns,                    // {0}
                Format0(snap.CarGroupRunning), // {1}
                pctCarParked,                  // {2}
                Format0(snap.CarGroupParked),  // {3}
                Format0(snap.CarGroupTotal),   // {4}
                updated                        // {5}
            );
        }

        private static int Percent(long part, long total)
        {
            if (total <= 0)
            {
                return 0;
            }

            double v = 100.0 * part / total;
            return (int) Math.Round(v, MidpointRounding.AwayFromZero);
        }

        private static string Localize(string entryId, string fallback)
        {
            LocalizationDictionary? dict = GameManager.instance?.localizationManager?.activeDictionary;
            if (dict != null && dict.TryGetValue(entryId, out string value) && !string.IsNullOrEmpty(value))
            {
                return value;
            }

            return fallback;
        }

        private static string SafeFormat(string key, string fallbackFormat, params object[] args)
        {
            string format = Localize(key, fallbackFormat);

            try
            {
                return string.Format(format, args);
            }
            catch (FormatException)
            {
                try
                {
                    return string.Format(fallbackFormat, args);
                }
                catch
                {
                    return fallbackFormat;
                }
            }
            catch
            {
                return fallbackFormat;
            }
        }

        private static string Format0(long v) => v.ToString("N0");
    }
}
