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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Stato veicoli personali" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Percorsi" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Abilita Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Attiva/disattiva il mod **ON / OFF**.\n" +
                    "Quando OFF, bici e scooter tornano ai valori predefiniti del gioco.\n\n" +
                    "Le info di stato sotto sono disponibili anche se Enable Fast Bikes è OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocità bici e scooter" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scala la velocità massima**.\n" +
                    "**0.30 = 30%** del valore del gioco\n" +
                    "**1.00 = valore del gioco**\n" +
                    "Nota: limiti stradali e condizioni del gioco si applicano comunque."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidità" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Fattore per **ampiezza oscillazione**.\n" +
                    "**Più alto = meno inclinazione**.\n" +
                    "**Più basso = più wobble**.\n" +
                    "Suggerito: 1.25–1.75 per stabilità ad alta velocità."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Smorzamento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Più alto = si stabilizza prima (meno oscillazioni).\n" +
                    "**1.00 = valore del gioco**\n" +
                    "Suggerito: 1.25–2.00 per stabilità ad alta velocità."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Default mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applica i valori di tuning predefiniti del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Default gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Imposta tutti gli slider a **100%** e ripristina i default del gioco (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite velocità percorsi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Scala i limiti di velocità dei **Percorsi** (i percorsi non sono strade).\n" +
                    "**1.00 = valore del gioco**\n" +
                    "Influisce: piste ciclabili, pedoni+bici, e percorsi pedonali.\n\n" +
                    "Nota disinstallazione: metti 1.00 o usa reset, salva la città, poi disinstalla.\n" +
                    "Se lo dimentichi: i vecchi percorsi restano moddati, i nuovi usano i default vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Gruppo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bici e scooter elettrici.\n" +
                    "**Attivo** = ha una corsia corrente (in movimento).\n" +
                    "**Totale parcheggiati** = include tutti i flag di parcheggio (es: bordo strada), non solo i parcheggi."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Gruppo auto" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Solo auto personali (esclude il Gruppo bici).\n" +
                    "**Attivo** = ha una corsia corrente (in movimento).\n" +
                    "**Parcheggiato** = ha **ParkedCar**.\n" +
                    "La scansione gira solo quando Options è aperto, quindi niente pensieri sulle prestazioni."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Auto parcheggiate nascoste" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Totale al confine OC** = veicoli del gruppo auto con ParkedCar alla connessione Fuori Città (OC).\n" +
                    "In alcune città ci sono molte auto bloccate e parcheggiate alla connessione Fuori Città.\n" +
                    "Usa <Registra auto nascoste> per un esempio di auto nascoste.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Registra auto nascoste" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Scrive un report una tantum in **Logs/FastBikes.log**.\n" +
                    "Include Totale + ripartizione Gruppo A/B/C e numeri ID di esempio.\n" +
                    "Usa il mod Scene Explorer per andare agli ID entità dei veicoli e investigare."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Stato non caricato." },
                { "FAST_STATS_NOT_AVAIL",       "Nessuna città... ¯\\_(ツ)_/¯ ...Niente stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Fai girare la città qualche minuto per i dati." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} attivi | {1} bici | {2} scooter | {3} / {4} parcheggiati/totale" },
                { "FAST_STATS_CARS_ROW2",  "{0} attivi | {1} parcheggiati | {2} totale | {3} rimorchi" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "{0} totale al confine OC | aggiornato {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nome visualizzato." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Versione attuale." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Apre la pagina Paradox Mods dell’autore." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Report debug bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Report dettagliato una tantum per debug o verifica giorno patch.\n" +
                    "Non serve per il gameplay normale.\n" +
                    "Prima carica una città.\n" +
                    "Posizione output: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
