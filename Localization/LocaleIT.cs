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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Azioni" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Info" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Velocità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilità" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Percorsi" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Info mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Abilita Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Attiva/disattiva la mod.\n" +
                    "Quando è OFF, bici e monopattini tornano ai valori predefiniti del gioco."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocità bici e monopattini" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scala la velocità massima**\n" +
                    "Formula di accelerazione e frenata più fluida alle alte velocità.\n" +
                    "**0.30 = 30%** del valore predefinito\n" +
                    "**1.00 = valore predefinito del gioco**\n" +
                    "Nota: limiti di velocità e condizioni del gioco possono comunque applicarsi."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidità" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Moltiplicatore per **ampiezza dell’oscillazione**.\n" +
                    "**Più alto = meno inclinazione** (aspetto più “stabile”).\n" +
                    "**Più basso = più wobble.**\n" +
                    "Nota: i monopattini possono inclinarsi di più perché i valori base sono diversi.\n" +
                    "Più stabile ad alta velocità: 1.25–1.75.\n" +
                    "Più wobble: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Smorzamento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Più alto = si stabilizza più in fretta (l’oscillazione si spegne prima).\n" +
                    "**1.0 = valori predefiniti del gioco**\n" +
                    "Più stabile ad alta velocità: 1.25–2.0+\n" +
                    "Più wobble: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valori mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applica i valori predefiniti della mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valori gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Riporta tutti i cursori al **100%** e ripristina i valori predefiniti del gioco."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite velocità percorsi" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Scala i limiti di velocità dei **percorsi** (i percorsi non sono strade).\n" +
                    "**1.00 = valore predefinito del gioco**\n" +
                    "Influisce: piste ciclabili, percorsi pedoni+bici separati, e percorsi solo pedonali.\n" +
                    "Nuova funzione Beta — feedback su GitHub o Forum."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nome visualizzato." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versione" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versione attuale." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Apre la pagina Paradox Mods dell’autore." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Report debug bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Report unico nel log con valori dettagliati di bici/monopattini.\n" +
                    "Non serve per il gameplay normale.\n\n" +
                    "Utile per verificare i prefab dopo aggiornamenti o per il debug.\n" +
                    "Carica prima una città; output in **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
