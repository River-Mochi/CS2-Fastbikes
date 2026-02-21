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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "About" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Speed" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stability" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Paths" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Mod info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Enable Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Turns the mod **ON/OFF**.\n" +
                    "When OFF, bicycle and scooter behavior is restored to game defaults.\n\n" +
                    "QoL: Status info below is available even if Enable Fast Bikes is off."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Bike & scooter speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scales max speed**\n" +
                    "Smooth acceleration and braking formula is used for high speeds.\n" +
                    "**0.30 = 30%** of game default\n" +
                    "**1.00 = game default**\n" +
                    "Note: road speed limits and game conditions may still apply."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Stiffness" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Scalar for **sway amplitude**.\n" +
                    "**Higher = less leaning** (tighter look).\n" +
                    "**Lower = more wobble.**\n" +
                    "Note: scooters can still lean more because their defaults are different.\n" +
                    "- For more stable at high speed: 1.25–1.75.\n" +
                    "- For more wobble: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Damping" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Higher = settles faster (oscillation dies faster).\n" +
                    "**1.0 = game defaults**\n" +
                    "- For more stable at high speed: 1.25–2.0+\n" +
                    "- For more wobble: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applies the mod’s default tuning values."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Game defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Sets all sliders back to **100%** and restores game defaults (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Path speed limit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Scales **Path** speed limits (paths are not roads).\n" +
                    "**1.00 = game default**\n" +
                    "Affects: bike paths, divided pedestrian+bike, and pedestrian paths.\n" +
                    "To uninstall the mod, reset this to 1.00 (and all values), load city, which restores path speed limits.\n" +
                    "Then the mod can be safely uninstalled. If you skipped this step, \n" +
                    "existing paths keep the current speed limits, and all new paths use vanilla default speed limits."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Status personal vehicles" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Bike group" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bikes and Electric scooters.\n" +
                    "**Active** = in a current lane (moving).\n" +
                    "**Parked** = has a **ParkedCar** flag.\n" +
                    "Tech notes: Bike group uses **BicycleData** filter."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Car group" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Personal cars only (excludes the Bike group above).\n" +
                    "**Active** = entities with a current lane (moving).\n" +
                    "**Parked** = entities with **ParkedCar** (includes roadside/curbside).\n" +
                    "Note: will not match in-game info panel parked numbers because we count all parked not just ones in a parking lot. \n" +
                    "Scan runs only when Options menu is open (not per-frame in city so it won't affect city performance)."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "Hidden parked vehicles:\n" +
                    "**Parked in buildings** = normal inside buildings/garages (it exists and has Unspawned flag because it's not currently visible).\n" +
                    "**Hidden at border** = parked at Outside Connection (OC),\n" +
                    "with an Owner who is most likely inside the city.\n" +
                    "  - Not sure of cause; info is for those who want to invstigate\n." +
                    "  - This could be a dev short cut to stage them there or something else.\n" +
                    "  - Use the <Log OC Cars> button to get a sample list"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Log OC cars" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Writes a small report to **Logs/FastBikes.log** with head+tail samples.\n" +
                    "Then use Scene Explorer mod to Jump To the listed Vehicle entity IDs and investigate."
                },

                { "FAST_STATUS_NOT_LOADED", "Status not loaded." },
                { "FAST_STATS_NOT_AVAIL", "No city... ¯\\_(ツ)_/¯ ...No stats" },
                { "FAST_STATS_CARS_NOT_AVAIL", "run the city a few minutes for data change." },

                { "FAST_STATS_BIKES_ROW1", "{0} active | {1} bikes | {2} scooter | {3} / {4} parked/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} active | {1} parked | {2} total | {3} trailers" },
                { "FAST_STATS_CARS_ROW3",  "{0} parked in buildings | {1} hidden at border OC | updated {2}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),     "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),      "Display name." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),  "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),   "Current version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Opens the author’s Paradox mods page." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Bike debug report" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "One-time log report of bicycle relevant values.\n" +
                    "Not needed for normal gameplay.\n\n" +
                    "Useful to verify prefabs after game updates or when debugging.\n" +
                    "Load city first before clicking; data sent to **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
