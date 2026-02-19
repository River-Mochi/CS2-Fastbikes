// File: Localization/LocaleDE.cs
// Purpose: German entries for FastBikes.

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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Aktionen" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "Über" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Geschwindigkeit" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilität" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Zurücksetzen" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Wege" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Mod-Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Schaltet den Mod **ON/OFF**.\n" +
                    "Wenn OFF, werden Fahrrad- und Rollerwerte auf die Standardwerte des Spiels zurückgesetzt."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Fahrrad- & Roller-Geschwindigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaliert die Maximalgeschwindigkeit**\n" +
                    "Für hohe Geschwindigkeiten wird eine weichere Beschleunigungs-/Bremsformel verwendet.\n" +
                    "**0.30 = 30%** der Spiel-Standardwerte\n" +
                    "**1.00 = Spiel-Standard**\n" +
                    "Hinweis: Tempolimits der Straßen und Spielbedingungen können weiterhin gelten."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Steifigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar für die **Schwank-Amplitude**.\n" +
                    "**Höher = weniger Neigung** (wirkt straffer).\n" +
                    "**Niedriger = mehr Wackeln.**\n" +
                    "Hinweis: Roller können stärker kippen, da ihre Standardwerte anders sind.\n" +
                    "- Für mehr Stabilität bei hoher Geschwindigkeit: 1.25–1.75.\n" +
                    "- Für mehr Wackeln: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Dämpfung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Höher = beruhigt sich schneller (Oszillation klingt schneller ab).\n" +
                    "**1.0 = Spiel-Standardwerte**\n" +
                    "- Für mehr Stabilität bei hoher Geschwindigkeit: 1.25–2.0+\n" +
                    "- Für mehr Wackeln: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Wendet die empfohlenen Standardwerte des Mods an."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Spiel-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Setzt alle Regler auf **100%** zurück und stellt die Spiel-Standardwerte (vanilla) wieder her."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Weg-Geschwindigkeitslimit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaliert die Geschwindigkeitslimits von **Wegen** (Wege sind keine Straßen).\n" +
                    "**1.00 = Spiel-Standard**\n" +
                    "Betrifft: Radwege, getrennte Fuß+Radwege und Fußwege.\n" +
                    "Zum Deinstallieren: auf 1.00 zurücksetzen (und alle Werte), Stadt laden (setzt Weg-Limits zurück).\n" +
                    "Danach kann der Mod sicher deinstalliert werden. Wenn dieser Schritt übersprungen wurde,\n" +
                    "behalten bestehende Wege die aktuellen Limits, und neue Wege nutzen die Vanilla-Standardlimits."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Status persönliche Fahrzeuge" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Fahrradgruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Fahrräder und E-Roller.\n" +
                    "**Aktiv** = Entitäten mit aktueller Spur (in Bewegung).\n" +
                    "**Geparkt** = Entitäten mit **ParkedCar**.\n" +
                    "Die Fahrradgruppe wird über **BicycleData** am Prefab gefiltert."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Autogruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Nur private Autos (schließt die Fahrradgruppe oben aus).\n" +
                    "**Aktiv** = Entitäten mit aktueller Spur (in Bewegung).\n" +
                    "**Geparkt** = Entitäten mit **ParkedCar** (inkl. Straßenrand).\n" +
                    "Hinweis: stimmt nicht mit dem Info-Panel überein, da dort nur Parkplatz-Fahrzeuge gezählt werden.\n" +
                    "Scan läuft nur, solange das Optionsmenü offen ist (nicht pro Frame in der Stadt, beste Performance)."
                },

                { "FAST_STATUS_NOT_LOADED", "Status nicht geladen." },
                { "FAST_STATS_NOT_AVAIL", "Keine Stadt... ¯\\_(ツ)_/¯ ...Keine Stats" },
                { "FAST_STATS_CARS_NOT_AVAIL", "Lass die Stadt ein paar Minuten laufen, um Daten zu erhalten." },

                { "FAST_STATS_BIKES_ROW1", "{0} aktiv | {1} Fahrräder | {2} Scooter | {3} / {4} geparkt/gesamt" },
                { "FAST_STATS_CARS_ROW2",  "{0} aktiv | {1} geparkt | {2} gesamt | aktualisiert {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "Anzeigename." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "Aktuelle Version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Bike-Debugbericht" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Einmaliger Log-Bericht mit relevanten Fahrradwerten.\n" +
                    "Nicht nötig für normales Gameplay.\n\n" +
                    "Nützlich, um Prefabs nach Spiel-Updates zu prüfen oder beim Debugging.\n" +
                    "Stadt zuerst laden; Daten werden in **Logs/FastBikes.log** geschrieben"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
