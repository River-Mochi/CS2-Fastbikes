// File: Localization/LocaleEN.cs
// Purpose: English entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Speed" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Stability" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status personal vehicles" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Paths" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Mod info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Enable Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Turns the mod **ON/OFF**.\n" +
                    "When OFF, bicycle and scooter behavior is restored to game defaults.\n\n" +
                    "Status info below is available even if Enable Fast Bikes is OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Bike & scooter speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scales max speed**.\n" +
                    "**0.30 = 30%** of game default\n" +
                    "**1.00 = game default**\n" +
                    "Note: road limits and game conditions still apply."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Stiffness" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Scalar for **sway amplitude**.\n" +
                    "**Higher = less leaning**.\n" +
                    "**Lower = more wobble**.\n" +
                    "Suggested: 1.25–1.75 for high-speed stability."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Damping" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Higher = settles faster (less oscillation).\n" +
                    "**1.00 = game default**\n" +
                    "Suggested: 1.25–2.00 for high-speed stability."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applies the mod’s default tuning values."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Game defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Sets all sliders to **100%** and restores game defaults (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Path speed limit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Scales **Path** speed limits (paths are not roads).\n" +
                    "**1.00 = game default**\n" +
                    "Affects: bike paths, divided pedestrian+bike, and pedestrian paths.\n\n" +
                    "Uninstall note: set to 1.00 or use reset button, save city, then uninstall.\n" +
                    "If you forget, then old paths simply keep the modded speed and new paths are vanilla game defaults."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Bike group" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bikes and electric scooters.\n" +
                    "**Active** = has a current lane (moving).\n" +
                    "**Total Parked** = includes all Parked flags (e.g., roadside), not just parking lots."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Car group" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Personal cars only (excludes the Bike group).\n" +
                    "**Active** = has a current lane (moving).\n" +
                    "**Parked** = has **ParkedCar**.\n" +
                    "Note: in-game info panel does not include all types of parked car so it has lower numbers.\n" +
                    "Scan runs only while Options is open and not in-city fps, so no worries on performance."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Hidden parked cars" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Hidden at border** = cars parked just outside the border on Outside City (OC) connection.\n" +
                    "Some cities show a large number of OC cars linked to in-city Owners.\n" +
                    "More study needed: is this game staging or something else?\n\n" +
                    "If curious: use <Log hidden cars> button to record sample IDs to the log.\n" +
                    "Then inspect ID's with Scene Explorer mod and share results. Can cims use these cars they are tied to?\n" +
                    "These cars are part of the total parked car count but seem to be hidden/underground at OC."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Log hidden cars" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Writes a small one-time report to **Logs/FastBikes.log** with head+tail samples.\n" +
                    "Use Scene Explorer mod to Jump To the listed Vehicle entity IDs."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status not loaded." },
                { "FAST_STATS_NOT_AVAIL",       "No city... ¯\\_(ツ)_/¯ ...No stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "run the city a few minutes for data." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} active | {1} bikes | {2} scooters | {3} / {4} parked/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} active | {1} parked | {2} total | {3} trailers" },
                { "FAST_STATS_CARS_ROW3",  "{0} hidden at border OC | updated {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Display name." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Current version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Opens the author’s Paradox mods page." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Bike debug report" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "One-time log report for debugging.\n" +
                    "Load a city first.\n" +
                    "Output: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
