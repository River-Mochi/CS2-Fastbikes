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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status pojazdów" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Ścieżki" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info moda" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Linki" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Włącz Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Włącza/wyłącza mod **ON/OFF**.\n" +
                    "Gdy OFF, rowery i hulajnogi wracają do ustawień gry.\n\n" +
                    "Status poniżej działa nawet gdy Włącz Fast Bikes jest OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Prędkość rowerów i hulajnóg" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Skaluje max prędkość**.\n" +
                    "**0.30 = 30%** ustawień gry\n" +
                    "**1.00 = ustawienia gry**\n" +
                    "Uwaga: limity dróg i warunki gry nadal obowiązują."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Sztywność" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Skalar dla **amplitudy bujania**.\n" +
                    "**Wyżej = mniejsze przechyły**.\n" +
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
                    "Ustawia domyślne wartości moda."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Domyślne gry" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Ustawia wszystkie suwaki na **100%** i przywraca grę (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limit na ścieżkach" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Skaluje limity prędkości **Ścieżek** (ścieżki to nie drogi).\n" +
                    "**1.00 = ustawienia gry**\n" +
                    "Dotyczy: ścieżek rowerowych, pieszo+rower, i pieszych.\n\n" +
                    "Odinstalowanie: ustaw 1.00 lub użyj resetu, zapisz miasto, potem usuń mod.\n" +
                    "Jeśli zapomnisz: stare ścieżki zostaną z prędkością moda, nowe będą vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupa rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Rowery i hulajnogi elektryczne.\n" +
                    "**Aktywne** = ma aktualny pas (ruch).\n" +
                    "**Razem zaparkowane** = obejmuje wszystkie flagi Parked (np. przy drodze), nie tylko parkingi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupa aut" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Tylko auta osobiste (bez Grupy rowerów).\n" +
                    "**Aktywne** = ma aktualny pas (ruch).\n" +
                    "**Zaparkowane** = ma **ParkedCar**.\n" +
                    "Uwaga: panel gry nie liczy wszystkich typów zaparkowanych aut, więc pokazuje mniej.\n" +
                    "Skan działa tylko gdy Opcje są otwarte, więc bez obaw o wydajność."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Ukryte auta" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Ukryte na granicy** = auta zaparkowane tuż za granicą na połączeniu Outside City (OC).\n" +
                    "Nie widać ich w grze i są CZĘŚCIĄ całkowitej liczby zaparkowanych aut.\n" +
                    "Niektóre miasta mają dużo aut OC powiązanych z Ownerami w mieście.\n" +
                    "Do zbadania: staging gry czy coś innego?\n\n" +
                    "Jeśli ciekawość: użyj <Zapisz ukryte auta> aby zapisać przykładowe ID do logu.\n" +
                    "Potem sprawdź ID w Scene Explorer i podziel się wynikami. Czy cims mogą używać tych aut?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Zapisz ukryte auta" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Zapisuje mały jednorazowy raport do **Logs/FastBikes.log** (próbki początek+koniec).\n" +
                    "Użyj Scene Explorer: Jump To do ID encji Vehicle."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status niezaładowany." },
                { "FAST_STATS_NOT_AVAIL",       "Brak miasta... ¯\\_(ツ)_/¯ ...brak danych" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "uruchom miasto parę minut." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} aktywne | {1} rowery | {2} hulajnogi | {3} / {4} zapark./razem" },
                { "FAST_STATS_CARS_ROW2",  "{0} aktywne | {1} zapark. | {2} razem | {3} przyczepy" },
                { "FAST_STATS_CARS_ROW3",  "{0} ukryte na granicy OC | akt {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nazwa wyświetlana." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Wersja" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Aktualna wersja." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Otwiera stronę Paradox mods autora." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Raport debug rowerów" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Jednorazowy raport do debugowania.\n" +
                    "Najpierw wczytaj miasto.\n" +
                    "Wyjście: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
