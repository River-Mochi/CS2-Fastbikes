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

    [FileLocation("ModsSettings/FastBikes/FastBikes")]
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(
        ActionsSpeedGrp, ActionsStabilityGrp, ActionsResetGrp, ActionsPathSpeedGrp, ActionsStatusGrp,
        AboutInfoGrp, AboutLinksGrp, AboutDebugGrp)]
    [SettingsUIShowGroupName(
        ActionsSpeedGrp, ActionsStabilityGrp, ActionsResetGrp, ActionsPathSpeedGrp, ActionsStatusGrp,
        AboutInfoGrp, AboutLinksGrp, AboutDebugGrp)]
    public sealed class Setting : ModSetting
    {
        public const string ActionsTab = "Actions";
        public const string AboutTab = "About";

        public const string ActionsSpeedGrp = "Speed";
        public const string ActionsStabilityGrp = "Stability";
        public const string ActionsResetGrp = "Reset";
        public const string ActionsStatusGrp = "Status";
        public const string ActionsPathSpeedGrp = "PathSpeed";

        public const string AboutInfoGrp = "Mod info";
        public const string AboutLinksGrp = "Links";
        public const string AboutDebugGrp = "Debug";

        private const string UrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";

        private const float Vanilla = 1.0f;

        private const bool DefaultEnabled = true;
        private const float DefaultSpeed = 2.0f;
        private const float DefaultStiffness = 1.50f;
        private const float DefaultDamping = 1.50f;
        private const float DefaultPathSpeedScalar = 2.0f;

        private const float FloatEpsilon = 0.0001f;

        public Setting(IMod mod) : base(mod)
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;
            PathSpeedScalar = DefaultPathSpeedScalar;
        }

        // ----------------------------
        // ACTIONS: Main toggle
        // ----------------------------

        [SettingsUISection(ActionsTab, ActionsSpeedGrp)]
        [SettingsUISetter(typeof(Setting), nameof(SetEnableFastBikes))]
        public bool EnableFastBikes
        {
            get; set;
        }

        // ------------------------
        // Actions: Speed
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsSpeedGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 10.00f, step = 0.10f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetSpeedScalar))]
        public float SpeedScalar
        {
            get; set;
        }

        // ------------------------
        // Actions: Stability
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsStabilityGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 5.00f, step = 0.10f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetStiffnessScalar))]
        public float StiffnessScalar
        {
            get; set;
        }

        [SettingsUISection(ActionsTab, ActionsStabilityGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 5.00f, step = 0.10f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetDampingScalar))]
        public float DampingScalar
        {
            get; set;
        }

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

        // -----------------------------
        // Actions: Path speed
        // -----------------------------

        [SettingsUISection(ActionsTab, ActionsPathSpeedGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 1.00f, max = 5.00f, step = 0.25f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetPathSpeedScalar))]
        public float PathSpeedScalar
        {
            get; set;
        }


        // -----------------------------
        // Actions: Status (read-only)
        // -----------------------------

        [SettingsUISection(ActionsTab, ActionsStatusGrp)]
        public string StatusSummary1
        {
            get
            {
                RefreshStatusSafe();
                return FastBikeStatus.BikesRow ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, ActionsStatusGrp)]
        public string StatusSummary2
        {
            get
            {
                RefreshStatusSafe();
                return FastBikeStatus.CarsRow ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, ActionsStatusGrp)]
        public string StatusSummary3
        {
            get
            {
                RefreshStatusSafe();
                return FastBikeStatus.CarsRow3 ?? string.Empty;
            }
        }

        [SettingsUISection(ActionsTab, ActionsStatusGrp)]
        [SettingsUIButton]
        public bool LogBorderHiddenCars
        {
            set
            {
                if (!value)
                {
                    return;
                }

                World world = World.DefaultGameObjectInjectionWorld;
                if (world == null || !world.IsCreated)
                {
                    return;
                }

                world.GetOrCreateSystemManaged<FastBikeStatusSystem>()
                    .LogBorderParkedSamples(headCount: 10, tailCount: 10);
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

                GetSystem()?.ScheduleDump();
            }
        }

        // --------------------------------------------------------------------
        // Defaults
        // --------------------------------------------------------------------

        public override void SetDefaults( )
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;
            PathSpeedScalar = DefaultPathSpeedScalar;
        }

        // --------------------------------------------------------------------
        // Helpers
        // --------------------------------------------------------------------

        private static void RefreshStatusSafe( )
        {
            try
            {
                FastBikeStatus.RefreshIfNeeded();
            }
            catch
            {
            }
        }

        private static World? GetWorld( )
        {
            World world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated)
            {
                return null;
            }

            return world;
        }

        private static FastBikeSystem? GetSystem( )
        {
            World? world = GetWorld();
            if (world == null)
            {
                return null;
            }

            return world.GetOrCreateSystemManaged<FastBikeSystem>();
        }

        // ------------------------------------------------
        // UI setter handlers
        // ------------------------------------------------

        private void SetEnableFastBikes(bool value)
        {
            if (EnableFastBikes == value)
            {
                return;
            }

            if (value)
            {
                GetSystem()?.ScheduleApplyAll();
            }
            else
            {
                GetSystem()?.ScheduleResetVanillaAll();
            }
        }

        private void SetSpeedScalar(float value)
        {
            if (Math.Abs(SpeedScalar - value) < FloatEpsilon)
            {
                return;
            }

            if (EnableFastBikes)
            {
                GetSystem()?.ScheduleApplyBicyclesAndStability();
            }
        }

        private void SetStiffnessScalar(float value)
        {
            if (Math.Abs(StiffnessScalar - value) < FloatEpsilon)
            {
                return;
            }

            if (EnableFastBikes)
            {
                GetSystem()?.ScheduleApplyBicyclesAndStability();
            }
        }

        private void SetDampingScalar(float value)
        {
            if (Math.Abs(DampingScalar - value) < FloatEpsilon)
            {
                return;
            }

            if (EnableFastBikes)
            {
                GetSystem()?.ScheduleApplyBicyclesAndStability();
            }
        }

        private void SetPathSpeedScalar(float value)
        {
            if (Math.Abs(PathSpeedScalar - value) < FloatEpsilon)
            {
                return;
            }

            if (EnableFastBikes)
            {
                GetSystem()?.ScheduleApplyPaths();
            }
        }

        private void DoResetToVanilla( )
        {
            SpeedScalar = Vanilla;
            StiffnessScalar = Vanilla;
            DampingScalar = Vanilla;
            PathSpeedScalar = Vanilla;

            ApplyAndSave();
            GetSystem()?.ScheduleResetVanillaAll();
        }

        private void DoResetToModDefaults( )
        {
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;
            PathSpeedScalar = DefaultPathSpeedScalar;

            ApplyAndSave();
            GetSystem()?.ScheduleApplyAll();
        }
    }
}
