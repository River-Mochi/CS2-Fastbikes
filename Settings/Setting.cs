// File: Settings/Setting.cs
// Purpose: Options UI + live apply triggers for FastBikes.

namespace FastBikes
{
    using Colossal.IO.AssetDatabase; // FileLocation
    using Game.Modding;              // IMod, ModSetting
    using Game.Settings;             // Settings UI attributes
    using Game.UI;                   // Unit
    using System;                    // Exception, Math
    using Unity.Entities;            // World
    using UnityEngine;               // Application.OpenURL

    [FileLocation(nameof(Setting))]
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(
        ActionsSpeedGrp, ActionsHandlingGrp, ActionsResetGrp,
        AboutInfoGrp, AboutLinksGrp, AboutDebugGrp)]
    [SettingsUIShowGroupName(
        ActionsSpeedGrp, ActionsHandlingGrp, ActionsResetGrp,
        AboutInfoGrp, AboutLinksGrp, AboutDebugGrp)]
    public sealed class Setting : ModSetting
    {
        public const string ActionsTab = "Actions";
        public const string AboutTab = "About";

        public const string ActionsSpeedGrp = "Speed";
        public const string ActionsHandlingGrp = "Handling";
        public const string ActionsResetGrp = "Reset";

        public const string AboutInfoGrp = "Mod info";
        public const string AboutLinksGrp = "Links";
        public const string AboutDebugGrp = "Debug";

        private const string UrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        // Vanilla multipliers
        private const float Vanilla = 1.0f;

        // Mod defaults (first install)
        private const bool DefaultEnabled = true;
        private const float DefaultSpeed = 2.0f;
        private const float DefaultStiffness = 1.25f;
        private const float DefaultDamping = 1.25f;

        private const float FloatEpsilon = 0.0001f;

        public Setting(IMod mod) : base(mod)
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;
        }

        // --------------------------------------------------------------------
        // ACTIONS: Master toggle
        // --------------------------------------------------------------------

        [SettingsUISection(ActionsTab, ActionsSpeedGrp)]
        [SettingsUISetter(typeof(Setting), nameof(SetEnableFastBikes))]
        public bool EnableFastBikes { get; set; }

        // ------------------------
        // Actions: Speed
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsSpeedGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 10.00f, step = 0.10f, unit = Unit.kFloatTwoFractions)]
        [SettingsUISetter(typeof(Setting), nameof(SetSpeedScalar))]
        public float SpeedScalar { get; set; }

        // ------------------------
        // Actions: Handling
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsHandlingGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 5.00f, step = 0.25f, unit = Unit.kFloatTwoFractions)]
        [SettingsUISetter(typeof(Setting), nameof(SetStiffnessScalar))]
        public float StiffnessScalar { get; set; }

        [SettingsUISection(ActionsTab, ActionsHandlingGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 5.00f, step = 0.25f, unit = Unit.kFloatTwoFractions)]
        [SettingsUISetter(typeof(Setting), nameof(SetDampingScalar))]
        public float DampingScalar { get; set; }

        // ------------------------
        // Actions: Reset buttons
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsResetGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUIButton]
        [SettingsUIButtonGroup("ResetRow")]
        public bool ResetToModDefaults
        {
            set
            {
                if (!value)
                {
                    return;
                }

                DoResetToModDefaults();
            }
        }

        [SettingsUISection(ActionsTab, ActionsResetGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUIButton]
        [SettingsUIButtonGroup("ResetRow")]
        public bool ResetToVanilla
        {
            set
            {
                if (!value)
                {
                    return;
                }

                DoResetToVanilla();
            }
        }

        // ------------------------
        // About: Info
        // ------------------------

        [SettingsUISection(AboutTab, AboutInfoGrp)]
        public string AboutName => Mod.ModName;

        [SettingsUISection(AboutTab, AboutInfoGrp)]
        public string AboutVersion => Mod.ModVersion;

        // ------------------------
        // About: Links
        // ------------------------

        [SettingsUIButton]
        [SettingsUIButtonGroup("LinksRow")]
        [SettingsUISection(AboutTab, AboutLinksGrp)]
        public bool OpenParadoxMods
        {
            set
            {
                if (!value)
                {
                    return;
                }

                try
                {
                    Application.OpenURL(UrlParadox);
                }
                catch (Exception)
                {
                    // Silent catch; worst case the link does nothing.
                }
            }
        }

        // ------------------------
        // About: Debug
        // ------------------------

        [SettingsUISection(AboutTab, AboutDebugGrp)]
        [SettingsUIButton]
        public bool DumpBicyclePrefabs
        {
            set
            {
                if (!value)
                {
                    return;
                }

                RequestDump();
            }
        }

        // --------------------------------------------------------------------
        // Defaults
        // --------------------------------------------------------------------

        public override void SetDefaults()
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;
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

        private static void ScheduleApply()
        {
            World? world = GetWorld();
            if (world == null)
            {
                return;
            }

            world.GetOrCreateSystemManaged<FastBikeSystem>().ScheduleApply();
        }

        private static void ScheduleResetVanilla()
        {
            World? world = GetWorld();
            if (world == null)
            {
                return;
            }

            world.GetOrCreateSystemManaged<FastBikeSystem>().ScheduleResetVanilla();
        }

        private static void RequestDump()
        {
            World? world = GetWorld();
            if (world == null)
            {
                return;
            }

            world.GetOrCreateSystemManaged<FastBikeSystem>().ScheduleDump();
        }

        // --------------------------------------------------------------------
        // UI setter handlers
        // --------------------------------------------------------------------

        private void SetEnableFastBikes(bool value)
        {
            if (EnableFastBikes == value)
            {
                return;
            }

            EnableFastBikes = value;

            if (EnableFastBikes)
            {
                ScheduleApply();
            }
            else
            {
                ScheduleResetVanilla();
            }
        }

        private void SetSpeedScalar(float value)
        {
            if (Math.Abs(SpeedScalar - value) < FloatEpsilon)
            {
                return;
            }

            SpeedScalar = value;

            // Slider setters are invoked repeatedly while dragging; apply is scheduled per change.
            if (EnableFastBikes)
            {
                ScheduleApply();
            }
        }

        private void SetStiffnessScalar(float value)
        {
            if (Math.Abs(StiffnessScalar - value) < FloatEpsilon)
            {
                return;
            }

            StiffnessScalar = value;

            if (EnableFastBikes)
            {
                ScheduleApply();
            }
        }

        private void SetDampingScalar(float value)
        {
            if (Math.Abs(DampingScalar - value) < FloatEpsilon)
            {
                return;
            }

            DampingScalar = value;

            if (EnableFastBikes)
            {
                ScheduleApply();
            }
        }

        private void DoResetToVanilla()
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = Vanilla;
            StiffnessScalar = Vanilla;
            DampingScalar = Vanilla;

            ScheduleResetVanilla();
        }

        private void DoResetToModDefaults()
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;

            ScheduleApply();
        }
    }
}
