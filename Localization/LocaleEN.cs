// File: Localization/LocaleEN.cs
// Purpose: English entries for FastBikes.

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
                { m_Setting.GetSettingsLocaleID(), title },

                // Tabs
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Speed" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stability" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Reset" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Mod info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Enable Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Turns the mod ON/OFF.\n" +
                    "When OFF, bicycle and scooter behavior is restored."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Bike & scooter speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scales top speed**\n" +
                    "Smooth acceleration and braking formula use for high speeds.\n" +
                    "**0.30 = 30%** of game default\n" +
                    "**1.00 = game default**\n" +
                    "Note: road speed limits and game conditions may still apply."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Stiffness" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Scalar for **sway amplitude**.\n" +
                    "Higher = less leaning (tighter look).\n" +
                    "Lower = more wobble.\n" +
                    "Note: scooters can still lean more because their defaults are different.\n" +
                    "More stable at high speed: 1.25–1.75.\n" +
                    "More wobble: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Damping" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Higher = settles faster (oscillation dies faster).\n" +
                    "**1.0 = game defaults**\n" +
                    "More stable at high speed: 1.25–2.0+\n" +
                    "More wobble: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applies the mod’s default tuning values."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Game defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Sets all sliders back to **100%** and restores game defaults."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Display name." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Current version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Opens the author’s Paradox mods page." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Bicycle Prefab Dump" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Logs bicycle/scooter detailed values.\n" +
                    "Not needed for normal gameplay.\n\n" +
                    "Useful after game updates or when debugging issues.\n" +
                    "Load city first, data sent to **FastBikes.log**"
                },

                // Alpha Path Speed
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsAlphaGrp), "Path Speed (alpha)" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DoublePathSpeedAlpha)), "Double speed of bike paths (alpha)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DoublePathSpeedAlpha)),
                    "Doubles the Pathway speed limit.\n" +
                    "Affects bike paths and divided pedestrian/bike paths.\n" +
                    "May also affect pedestrian-only paths.\n" +
                    "Alpha: experimental feature."
                },


            };
        }

        public void Unload()
        {
        }
    }
}
