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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Redefinir" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Informações do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Ativar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Liga/desliga o mod.\n" +
                    "Quando estiver DESL., o comportamento de bicicletas e patinetes elétricos é restaurado."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidade de bicicleta e patinete" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala a velocidade máxima**\n" +
                    "A aceleração e a frenagem também são ajustadas para a velocidade selecionada.\n" +
                    "**0.30 = 30%** do padrão do jogo\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Observação: limites de velocidade de vias/caminhos e condições do jogo ainda podem se aplicar."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Multiplicador da **amplitude do balanço**.\n" +
                    "Maior = menos inclinação (visual mais firme).\n" +
                    "Menor = mais oscilação.\n" +
                    "Observação: patinetes ainda podem inclinar mais, pois os padrões deles são diferentes.\n" +
                    "Mais estável em alta velocidade: 1.25–1.75.\n" +
                    "Mais oscilação: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortecimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Maior = estabiliza mais rápido (a oscilação some mais rápido).\n" +
                    "**1.0 = padrões do jogo**\n" +
                    "Mais estável em alta velocidade: 1.25–2.0+\n" +
                    "Mais oscilação: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Padrões do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica os valores padrão de ajuste do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Padrões do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Define todos os controles deslizantes de volta para **100%** e restaura os padrões do jogo."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nome de exibição." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versão atual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Dump de prefabs de bicicleta" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Registra valores detalhados de bicicletas/patinetes no log.\n" +
                    "Não é necessário para o jogo normal.\n\n" +
                    "Útil após atualizações do jogo ou ao depurar problemas.\n" +
                    "Carregue a cidade primeiro; os dados vão para **FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
