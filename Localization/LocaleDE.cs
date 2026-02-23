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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Geschwindigkeit" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Stabilität" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status Privatfahrzeuge" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Wege" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Mod-Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Schaltet den Mod **ON / OFF**.\n" +
                    "Bei OFF werden Fahrrad- und Scooter-Verhalten auf Spielstandard zurückgesetzt.\n\n" +
                    "Status unten ist auch verfügbar, wenn Enable Fast Bikes OFF ist."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Bike- & Scooter-Tempo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaliert die Maximalgeschwindigkeit**.\n" +
                    "**0.30 = 30%** vom Spielstandard\n" +
                    "**1.00 = Spielstandard**\n" +
                    "Hinweis: Straßenlimits und Spielbedingungen gelten weiter."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Steifigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar für **Schwank-Amplitude**.\n" +
                    "**Höher = weniger Neigung**.\n" +
                    "**Niedriger = mehr Wackeln**.\n" +
                    "Empfohlen: 1.25–1.75 für Stabilität bei hohem Tempo."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Dämpfung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Höher = beruhigt schneller (weniger Schwingung).\n" +
                    "**1.00 = Spielstandard**\n" +
                    "Empfohlen: 1.25–2.00 für Stabilität bei hohem Tempo."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod-Standard" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Wendet die Standard-Tuningwerte des Mods an."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Spiel-Standard" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Setzt alle Regler auf **100%** und stellt Spielstandard (vanilla) wieder her."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Tempo-Limit Wege" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaliert Tempolimits von **Wegen** (Wege sind keine Straßen).\n" +
                    "**1.00 = Spielstandard**\n" +
                    "Betrifft: Radwege, geteilte Fuß+Rad, und Fußwege.\n\n" +
                    "Deinstall-Hinweis: auf 1.00 setzen oder Reset nutzen, Stadt speichern, dann deinstallieren.\n" +
                    "Falls vergessen: alte Wege behalten die Mod-Geschwindigkeit, neue Wege sind Vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Bike-Gruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Fahrräder und E-Scooter.\n" +
                    "**Aktiv** = hat eine aktuelle Spur (in Bewegung).\n" +
                    "**Gesamt geparkt** = alle Park-Flags (z.B. Straßenrand), nicht nur Parkplätze."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Auto-Gruppe" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Nur Privatwagen (ohne Bike-Gruppe).\n" +
                    "**Aktiv** = hat eine aktuelle Spur (in Bewegung).\n" +
                    "**Geparkt** = hat **ParkedCar**.\n" +
                    "Scan läuft nur bei geöffneten Options und nicht in-city fps, also keine Sorge um Performance."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Versteckt geparkte Autos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Total an OC-Grenze** = Auto-Gruppen-Fahrzeuge mit ParkedCar an der Außerhalb-der-Stadt (OC) Verbindung.\n" +
                    "In manchen Städten gibt es viele Autos, die an der Außerhalb-der-Stadt Verbindung feststecken.\n" +
                    "Nutze <Versteckte Autos loggen> für eine Beispiel-Aufschlüsselung.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Versteckte Autos loggen" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Schreibt einen Einmal-Report nach **Logs/FastBikes.log**.\n" +
                    "Enthält Total + Aufteilung Bereich A/B/C und Beispiel-ID-Nummern.\n" +
                    "Nutze den Scene Explorer Mod, um zu den gelisteten Fahrzeug-Entity-IDs zu springen und zu prüfen."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status nicht geladen." },
                { "FAST_STATS_NOT_AVAIL",       "Keine Stadt... ¯\\_(ツ)_/¯ ...Keine Stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Stadt ein paar Minuten laufen lassen, um Daten zu sammeln." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} aktiv | {1} Bikes | {2} Scooter | {3} / {4} geparkt/gesamt" },
                { "FAST_STATS_CARS_ROW2",  "{0} aktiv | {1} geparkt | {2} gesamt | {3} Anhänger" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "{0} total an OC-Grenze | aktualisiert {1}" },

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
                    "Einmaliger, detaillierter Log-Report für Debug oder Patch-Day-Check.\n" +
                    "Für normales Gameplay nicht nötig.\n" +
                    "Zuerst eine Stadt laden.\n" +
                    "Ausgabeort: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
