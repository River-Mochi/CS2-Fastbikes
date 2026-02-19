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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Akcje" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "Informacje" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Prędkość" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilność" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Ścieżki" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Informacje o modzie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Włącz Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Włącza/wyłącza mod **ON/OFF**.\n" +
                    "Gdy OFF, zachowanie rowerów i hulajnóg wraca do wartości domyślnych gry."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Prędkość roweru i hulajnogi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaluje maksymalną prędkość**\n" +
                    "Dla dużych prędkości używana jest łagodniejsza formuła przyspieszania i hamowania.\n" +
                    "**0.30 = 30%** wartości domyślnych gry\n" +
                    "**1.00 = wartości domyślne gry**\n" +
                    "Uwaga: limity prędkości dróg i warunki gry nadal mogą mieć zastosowanie."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Sztywność" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar dla **amplitudy kołysania**.\n" +
                    "**Wyżej = mniej przechyłu** (bardziej „sztywno”).\n" +
                    "**Niżej = więcej chwiania.**\n" +
                    "Uwaga: hulajnogi mogą się bardziej przechylać, bo ich wartości domyślne są inne.\n" +
                    "- Dla większej stabilności przy dużej prędkości: 1.25–1.75.\n" +
                    "- Dla większego chwiania: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Tłumienie" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Wyżej = szybciej się uspokaja (oscylacja szybciej zanika).\n" +
                    "**1.0 = wartości domyślne gry**\n" +
                    "- Dla większej stabilności przy dużej prędkości: 1.25–2.0+\n" +
                    "- Dla większego chwiania: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Domyślne moda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Stosuje domyślne wartości strojenia moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Domyślne gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Ustawia wszystkie suwaki na **100%** i przywraca wartości domyślne gry (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limit prędkości ścieżek" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaluje limity prędkości **Ścieżek** (ścieżki nie są drogami).\n" +
                    "**1.00 = wartości domyślne gry**\n" +
                    "Dotyczy: ścieżek rowerowych, ścieżek pieszo+rowerowych oraz ścieżek pieszych.\n" +
                    "Aby odinstalować mod, ustaw to na 1.00 (i wszystkie wartości), wczytaj miasto, co przywróci limity prędkości ścieżek.\n" +
                    "Następnie mod można bezpiecznie odinstalować. Jeśli pominiesz ten krok,\n" +
                    "istniejące ścieżki zachowają obecne limity prędkości, a wszystkie nowe ścieżki użyją domyślnych limitów vanilla."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Status pojazdów osobistych" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupa rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Rowery i hulajnogi elektryczne.\n" +
                    "**Aktywne** = encje z aktualnym pasem (w ruchu).\n" +
                    "**Zaparkowane** = encje z **ParkedCar**.\n" +
                    "Grupa rowerów jest filtrowana przez **BicycleData** na prefabie."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupa aut" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Tylko auta osobiste (wyklucza grupę rowerów powyżej).\n" +
                    "**Aktywne** = encje z aktualnym pasem (w ruchu).\n" +
                    "**Zaparkowane** = encje z **ParkedCar** (w tym przy krawężniku).\n" +
                    "Uwaga: może nie zgadzać się z panelem gry, bo liczymy wszystkie zaparkowane, nie tylko te na parkingach.\n" +
                    "Skan działa tylko, gdy menu Opcji jest otwarte (nie co klatkę w mieście, dla najlepszej wydajności)."
                },

                { "FAST_STATUS_NOT_LOADED", "Status nie został załadowany." },
                { "FAST_STATS_NOT_AVAIL", "Brak miasta... ¯\\_(ツ)_/¯ ...Brak statystyk" },
                { "FAST_STATS_CARS_NOT_AVAIL", "Uruchom miasto przez kilka minut, aby zebrać dane." },

                { "FAST_STATS_BIKES_ROW1", "{0} aktywne | {1} rowery | {2} hulajnoga | {3} / {4} zapark./razem" },
                { "FAST_STATS_CARS_ROW2",  "{0} aktywne | {1} zapark. | {2} razem | zaktualizowano {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "Nazwa wyświetlana." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "Aktualna wersja." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Otwiera stronę autora na Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Raport debug rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Jednorazowy raport w logu z wartościami dotyczącymi rowerów.\n" +
                    "Nie jest potrzebny do normalnej rozgrywki.\n\n" +
                    "Przydatne do weryfikacji prefabów po aktualizacjach gry lub podczas debugowania.\n" +
                    "Najpierw wczytaj miasto; dane trafią do **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
