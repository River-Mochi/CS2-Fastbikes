// File: Localization/LocaleEN.cs
// English en-US locale for Fast Bikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            string title = Mod.ModName;

            if (!string.IsNullOrEmpty(Mod.ModVersion))
            {
                title = title + " (" + Mod.ModVersion + ")";
            }

            return new Dictionary<string, string>
            {
                // Options mod name
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.TuningGrp),     "Bikes" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Mod info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },

                // Actions / Bikes
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Enable bike tuning" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Scales bicycle max speed using a slider.\n" +
                    "Turn OFF to restore vanilla values."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.BikeSpeedScalar)), "Bicycle speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.BikeSpeedScalar)),
                    "**50%** = 0.5x speed (slower)\n" +
                    "**100%** = vanilla\n" +
                    "**2000%** = 20x speed (very fast)\n\n" +
                    "<Road/path speed limits and AI safe speed still apply>.\n" +
                    "Acceleration and braking are scaled gently to avoid extreme launch/stop behavior at high top speeds."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetSliders)), "Reset slider" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetSliders)),
                    "Sets the bicycle speed slider back to **100%** (vanilla default)." },

                // About
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Display name of this mod." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Current version." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),
                    "Opens the author’s Paradox mods page." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenGitHub)), "GitHub" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenGitHub)),
                    "Opens the source repository." },
            };
        }

        public void Unload()
        {
        }
    }
}
