// File: Settings/Setting.cs
// Purpose: Options UI + settings for Fast Bikes (Actions/About tabs).

namespace FastBikes
{
    using System;                    // Exception
    using Colossal.IO.AssetDatabase; // FileLocation
    using Game.Modding;              // IMod, ModSetting
    using Game.Settings;             // Settings UI attributes
    using Game.UI;                   // Unit
    using Unity.Entities;            // World
    using UnityEngine;               // Application.OpenURL

    [FileLocation("ModsSettings/FastBikes")]
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(TuningGrp, AboutInfoGrp, AboutLinksGrp)]
    [SettingsUIShowGroupName(TuningGrp, AboutLinksGrp)]
    public sealed class Setting : ModSetting
    {
        // ---- TABS ----
        public const string ActionsTab = "Actions";
        public const string AboutTab = "About";

        // ---- GROUPS ----
        public const string TuningGrp = "Tuning";
        public const string AboutInfoGrp = "AboutInfo";
        public const string AboutLinksGrp = "AboutLinks";

        // ---- TUNABLE CONSTANTS ----
        private const int kDefaultPercent = 100;

        private const int kBikeSpeedMin = 50;     // 0.5x
        private const int kBikeSpeedMax = 2000;   // 20x
        private const int kBikeSpeedStep = 10;

        private const string kUrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        private const string kUrlGitHub =
            "https://github.com/River-Mochi/CS2-Fastbikes";

        // ---- BACKING FIELDS ----
        private bool m_EnableFastBikes = true;

        public Setting(IMod mod)
            : base(mod)
        {
        }

        // --------------------------------------------------------------------
        // ACTIONS – TUNING
        // --------------------------------------------------------------------

        [SettingsUISection(ActionsTab, TuningGrp)]
        [SettingsUISetter(typeof(Setting), nameof(SetEnableFastBikes))]
        public bool EnableFastBikes
        {
            get => m_EnableFastBikes;
            set => m_EnableFastBikes = value;
        }

        [SettingsUISlider(min = kBikeSpeedMin, max = kBikeSpeedMax, step = kBikeSpeedStep, scalarMultiplier = 1, unit = Unit.kPercentage)]
        [SettingsUISection(ActionsTab, TuningGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISetter(typeof(Setting), nameof(SetBikeSpeedScalar))]
        public int BikeSpeedScalar { get; set; } = kDefaultPercent;

        [SettingsUIButton]
        [SettingsUISection(ActionsTab, TuningGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        public bool ResetSliders
        {
            set
            {
                if (!value)
                {
                    return;
                }

                BikeSpeedScalar = kDefaultPercent;
                RequestApply();
            }
        }

        // --------------------------------------------------------------------
        // ABOUT – INFO
        // --------------------------------------------------------------------

        [SettingsUISection(AboutTab, AboutInfoGrp)]
        public string AboutName => Mod.ModName;

        [SettingsUISection(AboutTab, AboutInfoGrp)]
        public string AboutVersion => Mod.ModVersion;

        // --------------------------------------------------------------------
        // ABOUT – LINKS
        // --------------------------------------------------------------------

        [SettingsUIButton]
        [SettingsUIButtonGroup(AboutLinksGrp)]
        [SettingsUISection(AboutTab, AboutLinksGrp)]
        public bool OpenParadoxMods
        {
            set
            {
                if (!value)
                {
                    return;
                }

                TryOpenUrl(kUrlParadox);
            }
        }

        [SettingsUIButton]
        [SettingsUIButtonGroup(AboutLinksGrp)]
        [SettingsUISection(AboutTab, AboutLinksGrp)]
        public bool OpenGitHub
        {
            set
            {
                if (!value)
                {
                    return;
                }

                TryOpenUrl(kUrlGitHub);
            }
        }

        // --------------------------------------------------------------------
        // DEFAULTS
        // --------------------------------------------------------------------

        public override void SetDefaults()
        {
            m_EnableFastBikes = true;
            BikeSpeedScalar = kDefaultPercent;
        }

        // --------------------------------------------------------------------
        // UI Setter Handlers
        // --------------------------------------------------------------------

        private void SetEnableFastBikes(bool value)
        {
            m_EnableFastBikes = value;
            RequestApply();
        }

        private void SetBikeSpeedScalar(int value)
        {
            BikeSpeedScalar = value;
            RequestApply();
        }

        // --------------------------------------------------------------------
        // Helpers
        // --------------------------------------------------------------------

        private static World? GetWorld()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return null;
            }

            return world;
        }

        private void RequestApply()
        {
            World? world = GetWorld();
            if (world == null)
            {
                return;
            }

            world.GetOrCreateSystemManaged<FastBikeSystem>()
                .ScheduleReapply();
        }

        private static void TryOpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            try
            {
                Application.OpenURL(url);
            }
            catch (Exception)
            {
            }
        }
    }
}
