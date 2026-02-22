// File: Localization/LocaleDE.cs
// Purpose: German entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Tempo" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Stabilität" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status Privatfahrz." },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Wege" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Mod-Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Schaltet den Mod **ON/OFF**.\n" +
                    "Bei OFF: Fahrrad- und Scooter-Verhalten zurück auf Spiel-Standard.\n\n" +
                    "Status unten ist auch verfügbar, wenn Fast Bikes aktivieren OFF ist."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Bike & E-Scooter Tempo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaliert max. Tempo**.\n" +
                    "**0.30 = 30%** vom Spiel\n" +
                    "**1.00 = Spiel**\n" +
                    "Hinweis: Straßenlimits und Spielbedingungen gelten weiterhin."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Steifigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar für **Sway-Amplitude**.\n" +
                    "**Höher = weniger Lean**.\n" +
                    "**Niedriger = mehr Wobble**.\n" +
                    "Tipp: 1.25–1.75 für High-Speed-Stabilität."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Dämpfung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Höher = beruhigt sich schneller (weniger Oszillation).\n" +
                    "**1.00 = Spiel**\n" +
                    "Tipp: 1.25–2.00 für High-Speed-Stabilität."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod-Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Setzt auf die Standardwerte des Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Spiel-Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Setzt alle Slider auf **100%** und stellt das Spiel (vanilla) wieder her."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Wege-Tempolimit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaliert Tempolimits von **Wegen** (Wege sind keine Straßen).\n" +
                    "**1.00 = Spiel**\n" +
                    "Wirkt auf: Radwege, geteilte Fuß+Rad, und Fußwege.\n\n" +
                    "Deinstallieren: auf 1.00 setzen oder Reset nutzen, Stadt speichern, dann deinstallieren.\n" +
                    "Wenn vergessen: alte Wege behalten Mod-Tempo, neue Wege sind Spiel-Default."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Bike-Gruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Fahrräder und E-Scooter.\n" +
                    "**Aktiv** = hat aktuelle Lane (fährt).\n" +
                    "**Insgesamt geparkt** = umfasst alle Parked-Flags (z.B. am Straßenrand), nicht nur Parkplätze."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Auto-Gruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Nur Privatwagen (ohne Bike-Gruppe).\n" +
                    "**Aktiv** = hat aktuelle Lane (fährt).\n" +
                    "**Geparkt** = hat **ParkedCar**.\n" +
                    "Hinweis: Ingame-Panel zählt nicht alles, daher niedriger.\n" +
                    "Scan läuft nur, solange Optionen offen sind — perf OK."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Versteckte Autos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Versteckt an Grenze** = Autos knapp außerhalb der Grenze an Outside City (OC)-Connection.\n" +
                    "Ingame unsichtbar und TEIL der Gesamtzahl geparkter Autos.\n" +
                    "Manche Städte zeigen viele OC-Autos mit Owners in der Stadt.\n" +
                    "Mehr Analyse nötig: Spiel-Staging oder was anderes?\n\n" +
                    "Wenn neugierig: Button <Versteckte Autos loggen> schreibt Beispiel-IDs ins Log.\n" +
                    "Dann IDs in Scene Explorer prüfen und teilen. Können Cims diese Autos nutzen?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Versteckte Autos loggen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Schreibt einen kleinen Einmal-Report in **Logs/FastBikes.log** (Kopf+Ende Beispiele).\n" +
                    "Scene Explorer nutzen: Jump To der Vehicle-Entity-IDs."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status nicht geladen." },
                { "FAST_STATS_NOT_AVAIL",       "Keine Stadt... ¯\\_(ツ)_/¯ ...keine Daten" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Stadt ein paar Minuten laufen lassen." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} aktiv | {1} Bikes | {2} Scooter | {3} / {4} geparkt/gesamt" },
                { "FAST_STATS_CARS_ROW2",  "{0} aktiv | {1} geparkt | {2} gesamt | {3} Anhänger" },
                { "FAST_STATS_CARS_ROW3",  "{0} versteckt an Grenze OC | akt {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Anzeigename." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Aktuelle Version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Bike-Debug-Report" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Einmaliger Log-Report für Debug.\n" +
                    "Erst eine Stadt laden.\n" +
                    "Ausgabe: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
