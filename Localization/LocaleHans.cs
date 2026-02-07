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

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Mod-Info" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes aktivieren" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Schaltet den Mod EIN/AUS.\n" +
                    "Wenn AUS, wird das Verhalten von Fahrrädern und E-Scootern wiederhergestellt."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Fahrrad- & E-Scooter-Geschwindigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaliert die Höchstgeschwindigkeit**\n" +
                    "Beschleunigung und Bremsen werden ebenfalls für die gewählte Geschwindigkeit angepasst.\n" +
                    "**0.30 = 30%** des Spiel-Standards\n" +
                    "**1.00 = Spiel-Standard**\n" +
                    "Hinweis: Tempolimits der Wege und Spielbedingungen können weiterhin gelten."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Steifigkeit" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Faktor für die **Schwank-/Neigamplitude**.\n" +
                    "Höher = weniger Neigung (strafferes Aussehen).\n" +
                    "Niedriger = mehr Wackeln.\n" +
                    "Hinweis: E-Scooter können sich weiterhin stärker neigen, weil ihre Standardwerte anders sind.\n" +
                    "Stabiler bei hoher Geschwindigkeit: 1.25–1.75.\n" +
                    "Mehr Wackeln: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Dämpfung" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Höher = beruhigt sich schneller (Oszillation endet schneller).\n" +
                    "**1.0 = Spiel-Standardwerte**\n" +
                    "Stabiler bei hoher Geschwindigkeit: 1.25–2.0+\n" +
                    "Mehr Wackeln: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Wendet die Standard-Tuningwerte des Mods an."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Spiel-Standardwerte" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Setzt alle Regler auf **100%** zurück und stellt die Spiel-Standardwerte wieder her."
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Fahrrad-Prefab-Dump" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Protokolliert detaillierte Werte für Fahrräder/E-Scooter.\n" +
                    "Nicht für normales Gameplay nötig.\n\n" +
                    "Nützlich nach Spielupdates oder beim Debuggen.\n" +
                    "Zuerst eine Stadt laden; Daten werden in **FastBikes.log** geschrieben."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
