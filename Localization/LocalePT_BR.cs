// File: Localization/LocalePT_BR.cs
// Purpose: Portuguese (Brazil) entries for FastBikes.

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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Ações" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Velocidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Estabilidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Redefinir" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Caminhos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Informações do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "Depuração" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Ativar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Liga/desliga o mod **ON/OFF**.\n" +
                    "Quando OFF, o comportamento de bicicletas e patinetes volta ao padrão do jogo."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidade da bicicleta e patinete" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala a velocidade máxima**\n" +
                    "Uma fórmula de aceleração e frenagem mais suave é usada para altas velocidades.\n" +
                    "**0.30 = 30%** do padrão do jogo\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Obs.: limites de velocidade das vias e condições do jogo ainda podem se aplicar."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escalar para a **amplitude de balanço**.\n" +
                    "**Maior = menos inclinação** (visual mais firme).\n" +
                    "**Menor = mais oscilação.**\n" +
                    "Obs.: patinetes ainda podem inclinar mais porque os padrões são diferentes.\n" +
                    "- Para mais estabilidade em alta velocidade: 1.25–1.75.\n" +
                    "- Para mais oscilação: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortecimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Maior = estabiliza mais rápido (a oscilação morre mais rápido).\n" +
                    "**1.0 = padrão do jogo**\n" +
                    "- Para mais estabilidade em alta velocidade: 1.25–2.0+\n" +
                    "- Para mais oscilação: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Padrões do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica os valores de ajuste padrão do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Padrões do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Define todos os sliders de volta para **100%** e restaura os padrões do jogo (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite de velocidade de caminhos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala os limites de velocidade de **Caminhos** (caminhos não são vias).\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Afeta: ciclovias, caminhos divididos pedestre+bicicleta e caminhos de pedestres.\n" +
                    "Para desinstalar o mod, redefina isto para 1.00 (e todos os valores), carregue a cidade e isso restaura os limites de velocidade dos caminhos.\n" +
                    "Depois, o mod pode ser desinstalado com segurança. Se você pular essa etapa,\n" +
                    "os caminhos existentes mantêm os limites atuais, e todos os novos caminhos usam os limites padrão do vanilla."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Status de veículos pessoais" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupo de bicicletas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bicicletas e patinetes elétricos.\n" +
                    "**Ativo** = entidades com uma faixa atual (em movimento).\n" +
                    "**Estacionado** = entidades com **ParkedCar**.\n" +
                    "O grupo de bicicletas é filtrado por **BicycleData** no prefab."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupo de carros" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Somente carros pessoais (exclui o Grupo de bicicletas acima).\n" +
                    "**Ativo** = entidades com uma faixa atual (em movimento).\n" +
                    "**Estacionado** = entidades com **ParkedCar** (inclui beira de rua/meio-fio).\n" +
                    "Obs.: não vai bater com o painel do jogo de carros estacionados porque contamos todos os estacionados, não só os em estacionamentos.\n" +
                    "A varredura roda apenas com o menu de Opções aberto (não roda a cada frame na cidade, para melhor desempenho)."
                },

                { "FAST_STATUS_NOT_LOADED", "Status não carregado." },
                { "FAST_STATS_NOT_AVAIL", "Sem cidade... ¯\\_(ツ)_/¯ ...sem estatísticas" },
                { "FAST_STATS_CARS_NOT_AVAIL", "Deixe a cidade rodar alguns minutos para obter dados." },

                { "FAST_STATS_BIKES_ROW1", "{0} ativo | {1} bicicletas | {2} patinete | {3} / {4} estac./total" },
                { "FAST_STATS_CARS_ROW2",  "{0} ativo | {1} estacionado | {2} total | atualizado {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "Nome exibido." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "Versão atual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Relatório de depuração de bicicletas" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Relatório único no log com valores relevantes de bicicletas.\n" +
                    "Não é necessário para jogabilidade normal.\n\n" +
                    "Útil para verificar prefabs após atualizações do jogo ou durante depuração.\n" +
                    "Carregue uma cidade antes de clicar; dados em **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
