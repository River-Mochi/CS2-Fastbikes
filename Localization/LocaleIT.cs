// File: Localization/LocaleIT.cs
// Purpose: Italian entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Azioni" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "Informazioni" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Velocità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Ripristino" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Percorsi" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Info mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Abilita Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Attiva/disattiva il mod **ON/OFF**.\n" +
                    "Quando OFF, il comportamento di biciclette e monopattini torna ai valori predefiniti del gioco."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocità bici & monopattini" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scala la velocità massima**\n" +
                    "Per le alte velocità viene usata una formula di accelerazione e frenata più fluida.\n" +
                    "**0.30 = 30%** del valore predefinito del gioco\n" +
                    "**1.00 = valore predefinito**\n" +
                    "Nota: i limiti di velocità delle strade e le condizioni del gioco possono comunque applicarsi."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidità" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Fattore per **l’ampiezza dell’oscillazione**.\n" +
                    "**Più alto = meno inclinazione** (aspetto più stabile).\n" +
                    "**Più basso = più oscillazione.**\n" +
                    "Nota: i monopattini possono inclinarsi di più perché i loro valori predefiniti sono diversi.\n" +
                    "- Per più stabilità ad alta velocità: 1.25–1.75.\n" +
                    "- Per più oscillazione: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Smorzamento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Più alto = si stabilizza più velocemente (l’oscillazione si spegne prima).\n" +
                    "**1.0 = valori predefiniti del gioco**\n" +
                    "- Per più stabilità ad alta velocità: 1.25–2.0+\n" +
                    "- Per più oscillazione: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Predefiniti del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applica i valori di tuning predefiniti del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Predefiniti del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Riporta tutti i cursori a **100%** e ripristina i valori predefiniti del gioco (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite velocità percorsi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Scala i limiti di velocità dei **Percorsi** (i percorsi non sono strade).\n" +
                    "**1.00 = valore predefinito del gioco**\n" +
                    "Influisce su: piste ciclabili, percorsi pedonali+bici, e percorsi pedonali.\n" +
                    "Per disinstallare il mod, reimposta a 1.00 (e tutti i valori), carica la città, così da ripristinare i limiti dei percorsi.\n" +
                    "Poi il mod può essere disinstallato in sicurezza. Se hai saltato questo passaggio,\n" +
                    "i percorsi esistenti mantengono i limiti attuali e tutti i nuovi percorsi useranno i limiti vanilla."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Stato veicoli personali" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Gruppo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Biciclette e monopattini elettrici.\n" +
                    "**Attivo** = entità con corsia corrente (in movimento).\n" +
                    "**Parcheggiato** = entità con **ParkedCar**.\n" +
                    "Il gruppo bici è filtrato tramite **BicycleData** sul prefab."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Gruppo auto" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Solo auto personali (esclude il gruppo bici sopra).\n" +
                    "**Attivo** = entità con corsia corrente (in movimento).\n" +
                    "**Parcheggiato** = entità con **ParkedCar** (include a bordo strada).\n" +
                    "Nota: non corrisponderà al pannello del gioco perché contiamo tutte le auto parcheggiate, non solo quelle nei parcheggi.\n" +
                    "La scansione avviene solo quando il menu Opzioni è aperto (non per-frame in città, massime prestazioni)."
                },

                { "FAST_STATUS_NOT_LOADED", "Stato non caricato." },
                { "FAST_STATS_NOT_AVAIL", "Nessuna città... ¯\\_(ツ)_/¯ ...Nessuna statistica" },
                { "FAST_STATS_CARS_NOT_AVAIL", "Lascia girare la città per qualche minuto per ottenere dati." },

                { "FAST_STATS_BIKES_ROW1", "{0} attivi | {1} bici | {2} monopattino | {3} / {4} parcheggiati/totale" },
                { "FAST_STATS_CARS_ROW2",  "{0} attivi | {1} parcheggiati | {2} totale | aggiornato {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "Nome visualizzato." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "Versione corrente." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Apre la pagina Paradox Mods dell’autore." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Report debug bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Report di log una tantum dei valori rilevanti per le biciclette.\n" +
                    "Non necessario per il gameplay normale.\n\n" +
                    "Utile per verificare i prefab dopo aggiornamenti del gioco o durante il debug.\n" +
                    "Carica una città prima di cliccare; dati inviati a **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
