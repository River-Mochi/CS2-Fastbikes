// File: Localization/LocalePT_BR.cs
// Purpose: Brazilian Portuguese entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocalePT_BR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocalePT_BR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Velocidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Estabilidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Caminhos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Info do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Ativar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Liga/desliga o mod (ON/OFF).\n" +
                    "Quando OFF, bicicletas e patinetes voltam ao padrão do jogo."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidade de bike e patinete" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala a velocidade máxima**\n" +
                    "Em velocidades altas, usa uma fórmula mais suave para acelerar/frear.\n" +
                    "**0.30 = 30%** do padrão do jogo\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Obs.: limites de velocidade e condições do jogo ainda podem valer."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escala a **amplitude do balanço**.\n" +
                    "**Maior = menos inclinação** (visual mais “firme”).\n" +
                    "**Menor = mais bamboleio.**\n" +
                    "Obs.: patinetes podem inclinar mais (valores base diferentes).\n" +
                    "Mais estável em alta velocidade: 1.25–1.75.\n" +
                    "Mais bamboleio: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortecimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Maior = estabiliza mais rápido (a oscilação some antes).\n" +
                    "**1.0 = padrão do jogo**\n" +
                    "Mais estável em alta velocidade: 1.25–2.0+\n" +
                    "Mais bamboleio: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Padrão do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica os valores padrão do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Padrão do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Volta todos os sliders para **100%** e restaura o padrão do jogo."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite de velocidade dos caminhos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala o limite de velocidade de **caminhos** (caminhos não são ruas).\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Afeta: ciclovias, caminhos pedestre+bike separados e caminhos só de pedestres.\n" +
                    "Novo recurso Beta — manda feedback no GitHub ou no fórum."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nome exibido." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versão atual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Relatório de debug de bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Relatório único no log com valores detalhados de bikes/patinetes.\n" +
                    "Não é necessário para jogar normalmente.\n\n" +
                    "Útil após atualizações do jogo ou para depurar.\n" +
                    "Carregue uma cidade antes de clicar; saída em **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
