// File: Localization/LocalePL.cs
// Purpose: Polish entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using Colossal.IO.AssetDatabase.Internal;
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocalePL : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePL(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Akcje" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "O modzie" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Prędkość" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilność" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Ścieżki" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Info o modzie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Włącz Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Włącza/wyłącza mod (ON/OFF).\n" +
                    "Gdy OFF, rowery i hulajnogi wracają do wartości domyślnych gry."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Prędkość rowerów i hulajnóg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaluje prędkość maksymalną**\n" +
                    "Dla dużych prędkości używana jest łagodniejsza formuła przyspieszania/hamowania.\n" +
                    "**0.30 = 30%** wartości domyślnej\n" +
                    "**1.00 = domyślne w grze**\n" +
                    "Uwaga: limity prędkości i warunki gry nadal mogą obowiązywać."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Sztywność" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar dla **amplitudy kołysania**.\n" +
                    "**Wyżej = mniej przechyłu** (bardziej „sztywno”).\n" +
                    "**Niżej = więcej bujania.**\n" +
                    "Uwaga: hulajnogi mogą przechylać się bardziej (inne wartości bazowe).\n" +
                    "Stabilniej przy wysokiej prędkości: 1.25–1.75.\n" +
                    "Więcej bujania: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Tłumienie" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Wyżej = szybciej się uspokaja (oscylacje szybciej znikają).\n" +
                    "**1.0 = domyślne w grze**\n" +
                    "Stabilniej przy wysokiej prędkości: 1.25–2.0+\n" +
                    "Więcej bujania: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Domyślne moda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Ustawia domyślne wartości strojenia moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Domyślne gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Ustawia wszystkie suwaki na **100%** i przywraca wartości gry."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limit prędkości na ścieżkach" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaluje limity prędkości na **ścieżkach** (ścieżki to nie drogi).\n" +
                    "**1.00 = domyślne w grze**\n" +
                    "Dotyczy: ścieżek rowerowych, ścieżek pieszy+rower oraz pieszych.\n" +
                    "Nowa funkcja Beta — daj znać na GitHubie lub forum."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nazwa wyświetlana." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Aktualna wersja." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),
                    "Otwiera stronę autora w serwisie Paradox Mods." },
                
                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Raport debug rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Jednorazowy raport w logu z detalami prefabów rowerów/hulajnóg.\n" +
                    "Niepotrzebne do normalnej gry.\n\n" +
                    "Przydatne po aktualizacjach gry lub do debugowania.\n" +
                    "Najpierw wczytaj miasto; wynik w **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
