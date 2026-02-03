// File: Mod.cs
// Entrypoint: registers settings, locales, ECS system.

namespace FastBikes
{
    using System;                    // Exception, Func<T>
    using System.Reflection;         // Assembly for version number
    using Colossal;                  // IDictionarySource
    using Colossal.IO.AssetDatabase; // AssetDatabase.LoadSettings
    using Colossal.Localization;     // LocalizationManager
    using Colossal.Logging;          // ILog, LogManager
    using CS2HonuShared;             // LogUtils
    using Game;                      // UpdateSystem, SystemUpdatePhase
    using Game.Modding;              // IMod
    using Game.SceneFlow;            // GameManager

    public sealed class Mod : IMod
    {
        public const string ModName = "Fast Bikes";
        public const string ModId = "FastBikes";
        public const string ModTag = "[FB]";

        public static readonly string ModVersion =
            Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        private static bool s_BannerLogged;

#if DEBUG
        private const string buildTag = "[DEBUG]";
#else
        private const string buildTag = "[RELEASE]";
#endif

        public static readonly ILog s_Log =
            LogManager.GetLogger(ModId).SetShowsErrorsInUI(
#if DEBUG
                true
#else
                false
#endif
            );

        public static Setting? Settings;

        public void OnLoad(UpdateSystem updateSystem)
        {
            if (!s_BannerLogged)
            {
                s_BannerLogged = true;
                LogSafe(() => $"{ModName} v{ModVersion} OnLoad {buildTag}");
            }

            if (GameManager.instance == null)
            {
                WarnSafe(() => "GameManager.instance is null in Mod.OnLoad.");
                return;
            }

            var setting = new Setting(this);
            Settings = setting;

            AddLocaleSource("en-US", new LocaleEN(setting));

            try
            {
                AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));
                setting.RegisterInOptionsUI();
            }
            catch (Exception ex)
            {
                WarnSafe(() => $"Settings/UI init failed: {ex.GetType().Name}: {ex.Message}");
            }

            try
            {
                updateSystem.UpdateAfter<FastBikeSystem>(SystemUpdatePhase.PrefabUpdate);
                updateSystem.World.GetOrCreateSystemManaged<FastBikeSystem>();
            }
            catch (Exception ex)
            {
                WarnSafe(() => $"System scheduling/init failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        public void OnDispose()
        {
            LogSafe(() => "OnDispose");

            if (Settings != null)
            {
                try
                {
                    Settings.UnregisterInOptionsUI();
                }
                catch (Exception ex) { WarnSafe(() => $"UnregisterInOptionsUI failed: {ex.GetType().Name}: {ex.Message}"); }

                Settings = null;
            }
        }

        public static void LogSafe(Func<string> messageFactory) => LogUtils.TryLog(s_Log, Level.Info, messageFactory);
        public static void WarnSafe(Func<string> messageFactory) => LogUtils.TryLog(s_Log, Level.Warn, messageFactory);
        public static void WarnOnce(string key, Func<string> messageFactory) => LogUtils.WarnOnce(s_Log, key, messageFactory);

        private static void AddLocaleSource(string localeId, IDictionarySource source)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                return;
            }

            LocalizationManager? lm = GameManager.instance?.localizationManager;
            if (lm == null)
            {
                WarnSafe(() => $"AddLocaleSource: No LocalizationManager; cannot add source for '{localeId}'.");
                return;
            }

            try
            {
                lm.AddSource(localeId, source);
            }
            catch (Exception ex)
            {
                WarnSafe(() => $"AddLocaleSource: AddSource for '{localeId}' failed: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
