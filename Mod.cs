// File: Mod.cs
// Purpose: Entry point for FastBikes. Registers settings, localization, and ECS systems.

namespace FastBikes
{
    using Colossal;                  // IDictionarySource
    using Colossal.IO.AssetDatabase; // AssetDatabase.LoadSettings
    using Colossal.Localization;     // LocalizationManager
    using Colossal.Logging;          // ILog, LogManager
    using CS2HonuShared;             // LogUtils
    using Game;                      // UpdateSystem, SystemUpdatePhase
    using Game.Modding;              // IMod
    using Game.SceneFlow;            // GameManager
    using System;                    // Exception, Func<T>
    using System.Reflection;         // Assembly

    public sealed class Mod : IMod
    {
        public const string ModId = "FastBikes";
        public const string ModName = "Fast Bikes";
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

        public static Setting? Settings
        {
            get; private set;
        }

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

            // Locales should be best-effort; never crash mod load.
            AddLocaleSource("en-US", new LocaleEN(setting));

            // Phase 2 (commented out for now)
            // AddLocaleSource("fr-FR", new LocaleFR(setting));
            // AddLocaleSource("es-ES", new LocaleES(setting));
            // AddLocaleSource("de-DE", new LocaleDE(setting));
            // AddLocaleSource("it-IT", new LocaleIT(setting));
            // AddLocaleSource("ja-JP", new LocaleJA(setting));
            // AddLocaleSource("ko-KR", new LocaleKO(setting));
            // AddLocaleSource("zh-HANS", new LocaleZH_CN(setting));
            // AddLocaleSource("pl-PL", new LocalePL(setting));
            // AddLocaleSource("pt-BR", new LocalePT_BR(setting));
            // AddLocaleSource("zh-HANT", new LocaleZH_HANT(setting));

            // Settings + Options UI
            try
            {
                // CS2 wiki + template pattern:
                // - LoadSettings(sectionName, instance, defaultInstance)
                // - Saving is automatic on changes (no manual SaveSettings(name, instance) call)
                AssetDatabase.global.LoadSettings(ModId, setting, new Setting(this));
                setting.RegisterInOptionsUI();
            }
            catch (Exception ex)
            {
                WarnSafe(() => $"Settings/UI init failed: {ex.GetType().Name}: {ex.Message}");
            }

            // System scheduling/init.
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
                try { Settings.UnregisterInOptionsUI(); }
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

            try { lm.AddSource(localeId, source); }
            catch (Exception ex)
            {
                WarnSafe(() => $"AddLocaleSource: AddSource for '{localeId}' failed: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}
