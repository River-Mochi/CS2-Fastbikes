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
                    "Schaltet das Mod ON/OFF.\n" +
                    "Wenn OFF, werden Fahrrad- und Scooter-Werte auf Spielstandard zurückgesetzt."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Fahrrad- & Scooter-Speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaliert die Höchstgeschwindigkeit**\n" +
                    "Für hohe Geschwindigkeiten wird eine weichere Beschleunigungs-/Bremsformel genutzt.\n" +
                    "**0.30 = 30%** vom Spielstandard\n" +
                    "**1.00 = Spielstandard**\n" +
                    "Hinweis: Tempolimits und Spielbedingungen können weiter gelten."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Steifigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar für die **Schwank-Amplitude**.\n" +
                    "**Höher = weniger Neigung** (wirkt „straffer“).\n" +
                    "**Niedriger = mehr Wackeln.**\n" +
                    "Hinweis: Scooter können stärker neigen (andere Defaults).\n" +
                    "Stabiler bei Highspeed: 1.25–1.75.\n" +
                    "Mehr Wackeln: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Dämpfung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Höher = beruhigt sich schneller (Schwingen endet schneller).\n" +
                    "**1.0 = Spielstandard**\n" +
                    "Stabiler bei Highspeed: 1.25–2.0+\n" +
                    "Mehr Wackeln: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod-Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Setzt auf die Standardwerte des Mods."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Spiel-Defaults" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Setzt alle Slider auf **100%** und stellt den Spielstandard wieder her."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Weg-Tempolimit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaliert Tempolimits von **Wegen** (Wege sind keine Straßen).\n" +
                    "**1.00 = Spielstandard**\n" +
                    "Betrifft: Radwege, getrennte Fuß+Rad, und reine Fußwege.\n" +
                    "Neue Beta-Funktion — Feedback auf GitHub oder im Forum willkommen."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Anzeigename." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Aktuelle Version." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Öffnet die Paradox-Mods-Seite des Autors." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Fahrrad-Debug-Report" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Einmaliger Log-Report mit detaillierten Fahrrad/Roller-Prefabwerten.\n" +
                    "Für normales Spielen nicht nötig.\n\n" +
                    "Hilfreich nach Updates oder beim Debuggen.\n" +
                    "Erst eine Stadt laden, dann klicken; Ausgabe in **Logs/FastBikes.log**"
                },

            };
        }

        public void Unload()
        {
        }
    }
}
