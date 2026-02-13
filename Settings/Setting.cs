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


    [FileLocation("ModsSettings/FastBikes/FastBikes")]    // Settings file path
    [SettingsUITabOrder(ActionsTab, AboutTab)]
    [SettingsUIGroupOrder(
        ActionsSpeedGrp, ActionsStabilityGrp, ActionsResetGrp, ActionsPathSpeedGrp,
        AboutInfoGrp, AboutLinksGrp, AboutDebugGrp)]
    [SettingsUIShowGroupName(
        ActionsSpeedGrp, ActionsStabilityGrp, ActionsResetGrp, ActionsPathSpeedGrp,
        AboutInfoGrp, AboutLinksGrp, AboutDebugGrp)]
    public sealed class Setting : ModSetting
    {
        public const string ActionsTab = "Actions";
        public const string AboutTab = "About";

        public const string ActionsSpeedGrp = "Speed";
        public const string ActionsStabilityGrp = "Stability";
        public const string ActionsResetGrp = "Reset";

        // PathSpeed group at bottom of Actions.
        public const string ActionsPathSpeedGrp = "PathSpeed";

        public const string AboutInfoGrp = "Mod info";
        public const string AboutLinksGrp = "Links";
        public const string AboutDebugGrp = "Debug";

        private const string UrlParadox =
            "https://mods.paradoxplaza.com/authors/River-mochi/cities_skylines_2?games=cities_skylines_2&orderBy=desc&sortBy=best&time=alltime";


        private const float Vanilla = 1.0f;   // game default multipliers.

        // Mod defaults (first install), 1.0 = off (vanilla game).
        private const bool DefaultEnabled = true;
        private const float DefaultSpeed = 2.0f;
        private const float DefaultStiffness = 1.50f;
        private const float DefaultDamping = 1.50f;

        private const float DefaultPathSpeedScalar = 2.0f;

        // Float compare guard to avoid scheduling extra applies on near identical values.
        private const float FloatEpsilon = 0.0001f;

        public Setting(IMod mod) : base(mod)
        {
            EnableFastBikes     = DefaultEnabled;
            SpeedScalar         = DefaultSpeed;
            StiffnessScalar     = DefaultStiffness;
            DampingScalar       = DefaultDamping;

            PathSpeedScalar     = DefaultPathSpeedScalar;
        }

        // -------------------------------------------------
        // ACTIONS: Main toggle
        // -------------------------------------------------

        [SettingsUISection(ActionsTab, ActionsSpeedGrp)]
        [SettingsUISetter(typeof(Setting), nameof(SetEnableFastBikes))]
        public bool EnableFastBikes { get; set; }

        // ------------------------
        // Actions: Speed
        // ------------------------

        // SettingsUIHideByCondition(..., invert: true) hides controls when the condition is false.
        [SettingsUISection(ActionsTab, ActionsSpeedGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 10.00f, step = 0.10f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetSpeedScalar))]
        public float SpeedScalar { get; set; }

        // ------------------------
        // Actions: Stability
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsStabilityGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 5.00f, step = 0.10f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetStiffnessScalar))]
        public float StiffnessScalar { get; set; }

        [SettingsUISection(ActionsTab, ActionsStabilityGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 0.30f, max = 5.00f, step = 0.10f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
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

        // -----------------------------
        // Actions: Path speed
        // -----------------------------

        [SettingsUISection(ActionsTab, ActionsPathSpeedGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 1.00f, max = 5.00f, step = 0.25f, unit = Unit.kFloatTwoFractions, updateOnDragEnd = true)]
        [SettingsUISetter(typeof(Setting), nameof(SetPathSpeedScalar))]
        public float PathSpeedScalar { get; set; }

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

            PathSpeedScalar = DefaultPathSpeedScalar;
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

        private static FastBikeSystem? GetSystem()
        {
            World? world = GetWorld();
            if (world == null)
            {
                return null;
            }

            return world.GetOrCreateSystemManaged<FastBikeSystem>();
        }

        private static void ScheduleApplyAll()
        {
            GetSystem()?.ScheduleApplyAll();
        }

        private static void ScheduleApplyBicyclesAndStability()
        {
            GetSystem()?.ScheduleApplyBicyclesAndStability();
        }

        private static void ScheduleApplyPaths()
        {
            GetSystem()?.ScheduleApplyPaths();
        }

        private static void ScheduleResetVanillaAll()
        {
            GetSystem()?.ScheduleResetVanillaAll();
        }

        private static void RequestDump()
        {
            GetSystem()?.ScheduleDump();
        }

        // ------------------------------------------------
        // UI setter handlers
        // ------------------------------------------------
        // - Called by Options UI binding system.
        // - Slider setters are invoked when the value is committed (updateOnDragEnd=true), not every drag tick.
        // - AutomaticSettings applies the property value after this callback, so setters only schedule work.
        // - Epsilon guard avoids scheduling extra applies for tiny/no-op float changes.

        private void SetEnableFastBikes(bool value)
        {
            if (EnableFastBikes == value)
            {
                return;
            }

            // AutomaticSettings sets the property after this callback.
            if (value)
            {
                ScheduleApplyAll();
            }
            else
            {
                ScheduleResetVanillaAll();
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
                ScheduleApplyBicyclesAndStability();
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
                ScheduleApplyBicyclesAndStability();
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
                ScheduleApplyBicyclesAndStability();
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
                ScheduleApplyPaths();
            }
        }

        private void DoResetToVanilla()
        {
            SpeedScalar = Vanilla;
            StiffnessScalar = Vanilla;
            DampingScalar = Vanilla;

            PathSpeedScalar = Vanilla;

         
            ApplyAndSave();             // Bool buttons do not auto-save; persist explicitly.
            ScheduleResetVanillaAll();  // Explicit vanilla restore is used to revert exactly to cached baselines.
        }

        private void DoResetToModDefaults()
        {
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            DampingScalar = DefaultDamping;

            PathSpeedScalar = DefaultPathSpeedScalar;

            ApplyAndSave();             // Bool buttons do not auto-save; persist explicitly.

            ScheduleApplyAll();
        }
    }
}
