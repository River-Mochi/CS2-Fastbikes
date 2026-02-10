// File: Localization/LocalePL.cs
// Purpose: Polish entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
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

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Informacje o modzie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Włącz Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Włącza/wyłącza mod.\n" +
                    "Gdy jest WYŁ., zachowanie rowerów i hulajnóg elektrycznych zostaje przywrócone."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Prędkość rowerów i hulajnóg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaluje prędkość maksymalną**\n" +
                    "Przyspieszenie i hamowanie są też dostosowywane do wybranej prędkości.\n" +
                    "**0.30 = 30%** domyślnych wartości gry\n" +
                    "**1.00 = wartości domyślne gry**\n" +
                    "Uwaga: limity prędkości na drogach/ścieżkach i warunki gry nadal mogą mieć zastosowanie."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Sztywność" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Mnożnik **amplitudy kołysania**.\n" +
                    "Wyżej = mniej przechyłu (bardziej „sztywno”).\n" +
                    "Niżej = więcej chwiania.\n" +
                    "Uwaga: hulajnogi mogą nadal bardziej się przechylać, bo ich wartości domyślne są inne.\n" +
                    "Większa stabilność przy dużej prędkości: 1.25–1.75.\n" +
                    "Więcej chwiania: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Tłumienie" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Wyżej = szybciej się uspokaja (drgania szybciej zanikają).\n" +
                    "**1.0 = wartości domyślne gry**\n" +
                    "Większa stabilność przy dużej prędkości: 1.25–2.0+\n" +
                    "Więcej chwiania: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Domyślne moda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Stosuje domyślne wartości strojenia moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Domyślne gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Ustawia wszystkie suwaki na **100%** i przywraca wartości domyślne gry."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nazwa wyświetlana." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Aktualna wersja." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Otwiera stronę autora w serwisie Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Zrzut prefabów rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Zapisuje szczegółowe wartości rowerów/hulajnóg w logu.\n" +
                    "Nie jest potrzebne do normalnej rozgrywki.\n\n" +
                    "Przydatne po aktualizacjach gry lub podczas debugowania.\n" +
                    "Najpierw wczytaj miasto; dane trafią do **FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
