// File: Localization/LocaleDE.cs
// Purpose: English entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleDE : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleDE(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Über" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Geschwindigkeit" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilität" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Zurücksetzen" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Wege" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Mod-Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Schaltet den Mod **EIN/AUS**.\n" +
                    "Wenn AUS, werden Fahrrad- und Scooter-Verhalten auf Spielstandard zurückgesetzt."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Fahrrad- & Scooter-Tempo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaliert die Höchstgeschwindigkeit**\n" +
                    "Für hohe Geschwindigkeiten wird eine sanfte Beschleunigungs- und Bremsformel verwendet.\n" +
                    "**0,30 = 30%** des Spielstandards\n" +
                    "**1,00 = Spielstandard**\n" +
                    "Hinweis: Tempolimits und Spielbedingungen können weiterhin gelten."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Steifigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar für **Schwankamplitude**.\n" +
                    "**Höher = weniger Neigung** (strafferer Look).\n" +
                    "**Niedriger = mehr Wackeln.**\n" +
                    "Hinweis: Scooter können weiterhin stärker neigen, weil ihre Standardwerte anders sind.\n" +
                    "- Für stabiler bei hohem Tempo: 1,25–1,75.\n" +
                    "- Für mehr Wackeln: < 0,75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Dämpfung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Höher = beruhigt sich schneller (Schwingung klingt schneller ab).\n" +
                    "**1,0 = Spielstandard**\n" +
                    "- Für stabiler bei hohem Tempo: 1,25–2,0+\n" +
                    "- Für mehr Wackeln: < 0,75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Wendet die Standard-Tuningwerte des Mods an."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Spiel-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Setzt alle Regler wieder auf **100%** und stellt die Spiel-Standardwerte wieder her."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Weg-Tempolimit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaliert **Weg**-Tempolimits (Wege sind keine Straßen).\n" +
                    "**1,00 = Spielstandard**\n" +
                    "Betrifft: Radwege, geteilte Fußgänger+Rad-Wege und reine Fußwege.\n" +
                    "Neue Beta-Funktion - bitte Feedback über Github- oder Forum-Links.\n" +
                    "Zum Deinstallieren: auf 1,00 zurücksetzen (und alle Werte), Stadt laden, wodurch Weg-Tempolimits wiederhergestellt werden.\n" +
                    "Dann kann der Mod sicher deinstalliert werden. Falls vergessen,\n" +
                    "bleiben vorhandene Wege bei ihrem aktuellen Tempolimit, aber neue Wege haben wieder die Standard-Tempolimits."

                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Status: Privatfahrzeuge" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Fahrrad-Gruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Prozent = Anteil an ALLEN Privatfahrzeugen.\n" +
                    "Fahrräder+Scooter werden über **BicycleData** am Prefab gefiltert."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Auto-Gruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Prozent = Anteil an ALLEN Privatfahrzeugen.\n" +
                    "Scan läuft nur, solange Optionen offen sind."
                },

                { "FAST_STATUS_NOT_LOADED", "Status nicht geladen." },
                { "FAST_STATS_NOT_AVAIL", "Keine Stadt... ¯\\_(ツ)_/¯ ...Keine Stats" },

                { "FAST_STATS_BIKES_ROW1", "{0}% ({1}) Bikes | {2}% ({3}) Scooter |  {4} / {5} geparkt/gesamt" },
                { "FAST_STATS_CARS_ROW2",  "{0}% ({1}) fährt | {2} / {3} geparkt/gesamt | aktualisiert {4}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Anzeigename." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Aktuelle Version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Bike-Debugbericht" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Einmaliger Log-Bericht zu fahrradrelevanten Werten.\n" +
                    "Für normales Spielen nicht nötig.\n\n" +
                    "Hilfreich, um Prefabs nach Spiel-Updates zu prüfen oder beim Debugging.\n" +
                    "Stadt zuerst laden, dann klicken; Daten gehen an **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
