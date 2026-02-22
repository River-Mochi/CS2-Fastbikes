// File: Localization/LocaleIT.cs
// Purpose: Italian entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleIT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleIT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Azioni" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Velocità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Stabilità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Stato veicoli perso" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Percorsi" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Abilita Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Attiva/disattiva il mod **ON/OFF**.\n" +
                    "Quando OFF, bici e monopattini tornano ai valori del gioco.\n\n" +
                    "Lo stato sotto è disponibile anche se Abilita Fast Bikes è OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocità bici e monop." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scala la velocità max**.\n" +
                    "**0.30 = 30%** del gioco\n" +
                    "**1.00 = gioco**\n" +
                    "Nota: limiti stradali e condizioni del gioco si applicano comunque."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidità" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Scala per **ampiezza oscillazione**.\n" +
                    "**Più alto = meno inclinazione**.\n" +
                    "**Più basso = più wobble**.\n" +
                    "Consiglio: 1.25–1.75 per stabilità ad alta velocità."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Smorzamento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Più alto = si stabilizza prima (meno oscillazione).\n" +
                    "**1.00 = gioco**\n" +
                    "Consiglio: 1.25–2.00 per stabilità ad alta velocità."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Default mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applica i valori predefiniti del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Default gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Imposta tutti gli slider a **100%** e ripristina il gioco (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite percorsi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Scala i limiti dei **Percorsi** (i percorsi non sono strade).\n" +
                    "**1.00 = gioco**\n" +
                    "Influisce su: piste ciclabili, pedonale+bici, e percorsi pedonali.\n\n" +
                    "Disinstallazione: imposta a 1.00 o usa reset, salva la città, poi disinstalla.\n" +
                    "Se te ne dimentichi: i vecchi percorsi tengono la velocità mod e i nuovi sono vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Gruppo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bici e monopattini elettrici.\n" +
                    "**Attivo** = ha una lane corrente (in movimento).\n" +
                    "**Totale parcheggiati** = include tutti i flag Parked (es. bordo strada), non solo parcheggi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Gruppo auto" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Solo auto personali (esclude Gruppo bici).\n" +
                    "**Attivo** = ha una lane corrente (in movimento).\n" +
                    "**Parcheggiata** = ha **ParkedCar**.\n" +
                    "Nota: il pannello del gioco non include tutti i tipi di auto parcheggiate, quindi numeri più bassi.\n" +
                    "Lo scan gira solo mentre Opzioni è aperto, quindi ok per le prestazioni."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Auto nascoste" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Nascoste al confine** = auto parcheggiate appena fuori dal confine su collegamento Outside City (OC).\n" +
                    "Queste auto non sono visibili in gioco e sono PARTE del totale auto parcheggiate.\n" +
                    "Alcune città mostrano molte auto OC collegate a Owners in città.\n" +
                    "Da studiare: staging del gioco o altro?\n\n" +
                    "Se curioso: usa <Log auto nascoste> per scrivere ID di esempio nel log.\n" +
                    "Poi controlla gli ID con Scene Explorer e condividi i risultati. I cims possono usare queste auto?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Log auto nascoste" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Scrive un piccolo report una tantum in **Logs/FastBikes.log** con esempi testa+coda.\n" +
                    "Usa Scene Explorer: Jump To agli ID entità Vehicle elencati."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Stato non caricato." },
                { "FAST_STATS_NOT_AVAIL",       "Niente città... ¯\\_(ツ)_/¯ ...niente dati" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "fai girare la città qualche minuto." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} attivi | {1} bici | {2} monop. | {3} / {4} parch/tot" },
                { "FAST_STATS_CARS_ROW2",  "{0} attivi | {1} parch | {2} tot | {3} rimorchi" },
                { "FAST_STATS_CARS_ROW3",  "{0} nascosti confine OC | agg {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nome visualizzato." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Versione corrente." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Apre la pagina Paradox mods dell’autore." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Report debug bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Report una tantum per debug.\n" +
                    "Carica prima una città.\n" +
                    "Output: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
