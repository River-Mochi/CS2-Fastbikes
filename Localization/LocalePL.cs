// File: Localization/LocalePL.cs
// Purpose: Polish entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "O modzie" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Prędkość" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Stabilność" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status pojazdów osobistych" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Ścieżki" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info o modzie" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Włącz Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Przełącza mod **WŁ. / WYŁ.**\n" +
                    "Gdy WYŁ., zachowanie rowerów i skuterów wraca do ustawień gry.\n\n" +
                    "Status poniżej jest dostępny nawet gdy Enable Fast Bikes jest WYŁ."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Prędkość rowerów i skuterów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaluje prędkość maks.**\n" +
                    "**0.30 = 30%** ustawień gry\n" +
                    "**1.00 = ustawienia gry**\n" +
                    "Uwaga: limity dróg i warunki gry nadal działają."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Sztywność" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skala dla **amplitudy kołysania**.\n" +
                    "**Wyżej = mniej przechyłu**.\n" +
                    "**Niżej = więcej chwiania**.\n" +
                    "Sugerowane: 1.25–1.75 dla stabilności przy dużej prędkości."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Tłumienie" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Wyżej = szybciej się uspokaja (mniej oscylacji).\n" +
                    "**1.00 = ustawienia gry**\n" +
                    "Sugerowane: 1.25–2.00 dla stabilności przy dużej prędkości."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Domyślne moda" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Ustawia domyślne wartości tuningu moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Domyślne gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Ustawia wszystkie suwaki na **100%** i przywraca ustawienia gry (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limit prędkości ścieżek" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaluje limity prędkości **Ścieżek** (ścieżki to nie drogi).\n" +
                    "**1.00 = ustawienia gry**\n" +
                    "Dotyczy: ścieżek rowerowych, pieszo+rower, i ścieżek pieszych.\n\n" +
                    "Odinstalowanie: ustaw 1.00 lub użyj resetu, zapisz miasto, potem odinstaluj.\n" +
                    "Jeśli zapomnisz: stare ścieżki zostaną zmienione, nowe będą vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupa rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Rowery i elektryczne skutery.\n" +
                    "**Aktywne** = ma bieżący pas (jedzie).\n" +
                    "**Łącznie zaparkowane** = wszystkie flagi parkowania (np. pobocze), nie tylko parkingi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupa aut" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Tylko auta osobiste (bez Grupy rowerów).\n" +
                    "**Aktywne** = ma bieżący pas (jedzie).\n" +
                    "**Zaparkowane** = ma **ParkedCar**.\n" +
                    "Skan działa tylko gdy Options jest otwarte, więc bez obaw o wydajność."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Ukryte zaparkowane auta" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Suma na granicy OC** = auta z grupy aut z ParkedCar na połączeniu Poza Miastem (OC).\n" +
                    "W niektórych miastach dużo aut utknie zaparkowanych na połączeniu Poza Miastem.\n" +
                    "Użyj <Zaloguj ukryte auta> aby dostać przykładowy podział ukrytych aut.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Zaloguj ukryte auta" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Zapisuje jednorazowy raport do **Logs/FastBikes.log**.\n" +
                    "Zawiera Sumę + podział Grupa A/B/C oraz przykładowe numery ID.\n" +
                    "Użyj Scene Explorer mod, aby przejść do ID encji pojazdów i sprawdzić."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status niezaładowany." },
                { "FAST_STATS_NOT_AVAIL",       "Brak miasta... ¯\\_(ツ)_/¯ ...Brak statystyk" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Uruchom miasto na kilka minut, aby zebrać dane." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} aktywne | {1} rowery | {2} skutery | {3} / {4} zapark./suma" },
                { "FAST_STATS_CARS_ROW2",  "{0} aktywne | {1} zapark. | {2} suma | {3} przyczepy" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "{0} suma na granicy OC | aktualizacja {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nazwa wyświetlana." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Aktualna wersja." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Otwiera stronę autora na Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Debug rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Jednorazowy szczegółowy raport do debugowania lub weryfikacji po patchu.\n" +
                    "Niepotrzebne do normalnej gry.\n" +
                    "Najpierw wczytaj miasto.\n" +
                    "Lokalizacja: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
