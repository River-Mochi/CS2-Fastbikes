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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Reimposta" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Info mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Link" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Abilita Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Attiva/disattiva la mod (ON/OFF).\n" +
                    "Quando è OFF, il comportamento di biciclette e monopattini viene ripristinato."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocità bici e monopattini" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Scala la velocità massima**\n" +
                    "Accelerazione e frenata vengono anche regolate in base alla velocità selezionata.\n" +
                    "**0.30 = 30%** del valore predefinito del gioco\n" +
                    "**1.00 = valore predefinito del gioco**\n" +
                    "Nota: i limiti di velocità delle vie e le condizioni di gioco possono comunque applicarsi."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidità" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Moltiplicatore per l’**ampiezza dell’oscillazione**.\n" +
                    "Più alto = meno inclinazione (aspetto più stabile).\n" +
                    "Più basso = più ondeggiamento.\n" +
                    "Nota: i monopattini possono inclinarsi di più perché i valori predefiniti sono diversi.\n" +
                    "Più stabile ad alta velocità: 1.25–1.75.\n" +
                    "Più ondeggiamento: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Smorzamento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Più alto = si stabilizza più velocemente (l’oscillazione si spegne prima).\n" +
                    "**1.0 = valori predefiniti del gioco**\n" +
                    "Più stabile ad alta velocità: 1.25–2.0+\n" +
                    "Più ondeggiamento: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valori predefiniti del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applica i valori di tuning predefiniti della mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valori predefiniti del gioco" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Riporta tutti i cursori a **100%** e ripristina i valori predefiniti del gioco."
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Dump dei prefab bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Registra valori dettagliati di biciclette/monopattini.\n" +
                    "Non necessario per il gameplay normale.\n\n" +
                    "Utile dopo aggiornamenti del gioco o per il debug.\n" +
                    "Carica prima una città; i dati vengono scritti in **FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
