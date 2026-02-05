// File: Settings/Setting.cs
// Purpose: Options UI + live apply triggers for FastBikes.

namespace FastBikes
{
    using Colossal.IO.AssetDatabase; // FileLocation
    using Game.Modding;              // IMod, ModSetting
    using Game.Settings;             // Settings UI attributes
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

        private const int Vanilla = 100;

        // Mod defaults
        private const bool DefaultEnabled = true;
        private const int DefaultSpeed = 150;
        private const int DefaultStiffness = 100;
        private const int DefaultSpring = 100;
        private const int DefaultDamping = 100;

        public Setting(IMod mod) : base(mod)
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            SpringScalar = DefaultSpring;
            DampingScalar = DefaultDamping;
        }

        // --------------------------------------------------------------------
        // ACTIONS: Master toggle
        // --------------------------------------------------------------------

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
        [SettingsUISlider(min = 50, max = 300, step = 5)]
        [SettingsUISetter(typeof(Setting), nameof(SetSpeedScalar))]
        public int SpeedScalar
        {
            get; set;
        }

        // ------------------------
        // Actions: Handling (placeholder until swaying component is confirmed)
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsHandlingGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 25, max = 400, step = 5)]
        [SettingsUISetter(typeof(Setting), nameof(SetStiffnessScalar))]
        public int StiffnessScalar
        {
            get; set;
        }

        [SettingsUISection(ActionsTab, ActionsHandlingGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 25, max = 400, step = 5)]
        [SettingsUISetter(typeof(Setting), nameof(SetSpringScalar))]
        public int SpringScalar
        {
            get; set;
        }

        [SettingsUISection(ActionsTab, ActionsHandlingGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUISlider(min = 25, max = 400, step = 5)]
        [SettingsUISetter(typeof(Setting), nameof(SetDampingScalar))]
        public int DampingScalar
        {
            get; set;
        }

        // ------------------------
        // Actions: Reset buttons
        // ------------------------

        [SettingsUISection(ActionsTab, ActionsResetGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetToVanilla
        {
            set => DoResetToVanilla();
        }

        [SettingsUISection(ActionsTab, ActionsResetGrp)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(EnableFastBikes), true)]
        [SettingsUIButton]
        public bool ResetToModDefaults
        {
            set => DoResetToModDefaults();
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

        [SettingsUISection(AboutTab, AboutLinksGrp)]
        [SettingsUIButton]
        [SettingsUIButtonGroup("LinksRow")]
        public bool OpenGitHub
        {
            set => OpenUrl("https://github.com/River-Mochi/CS2-Fastbikes");
        }

        // ------------------------
        // About: Debug
        // ------------------------

        [SettingsUISection(AboutTab, AboutDebugGrp)]
        public bool VerboseLogging
        {
            get; set;
        }

        [SettingsUISection(AboutTab, AboutDebugGrp)]
        [SettingsUIButton]
        public bool DumpBicyclePrefabs
        {
            set => RequestDump();
        }

        // --------------------------------------------------------------------
        // Defaults
        // --------------------------------------------------------------------

        public override void SetDefaults()
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            SpringScalar = DefaultSpring;
            DampingScalar = DefaultDamping;
            VerboseLogging = false;
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

        private static void OpenUrl(string url)
        {
            try { Application.OpenURL(url); }
            catch { }
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

        private void SetSpeedScalar(int value)
        {
            SpeedScalar = value;
            ScheduleApply();
        }

        private void SetStiffnessScalar(int value)
        {
            StiffnessScalar = value;
            ScheduleApply();
        }

        private void SetSpringScalar(int value)
        {
            SpringScalar = value;
            ScheduleApply();
        }

        private void SetDampingScalar(int value)
        {
            DampingScalar = value;
            ScheduleApply();
        }

        private void DoResetToVanilla()
        {
            SpeedScalar = Vanilla;
            StiffnessScalar = Vanilla;
            SpringScalar = Vanilla;
            DampingScalar = Vanilla;

            ScheduleResetVanilla();
        }

        private void DoResetToModDefaults()
        {
            EnableFastBikes = DefaultEnabled;
            SpeedScalar = DefaultSpeed;
            StiffnessScalar = DefaultStiffness;
            SpringScalar = DefaultSpring;
            DampingScalar = DefaultDamping;

            ScheduleApply();
        }
    }
}
