// File: Localization/LocalePT_BR.cs
// Purpose: Brazilian Portuguese entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "Sobre" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Velocidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Estabilidade" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status de veículos pessoais" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Caminhos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Ativar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Liga/desliga o mod **LIGADO / DESLIGADO**.\n" +
                    "Quando DESLIGADO, o comportamento de bikes e scooters volta ao padrão do jogo.\n\n" +
                    "O status abaixo aparece mesmo com Enable Fast Bikes DESLIGADO."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidade de bike e scooter" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala a velocidade máxima**.\n" +
                    "**0.30 = 30%** do padrão do jogo\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Nota: limites das vias e condições do jogo ainda se aplicam."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escala da **amplitude de balanço**.\n" +
                    "**Maior = menos inclinação**.\n" +
                    "**Menor = mais tremedeira**.\n" +
                    "Sugestão: 1.25–1.75 para estabilidade em alta velocidade."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortecimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Maior = estabiliza mais rápido (menos oscilação).\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Sugestão: 1.25–2.00 para estabilidade em alta velocidade."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Padrão do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica os valores padrão de ajuste do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Padrão do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Coloca todos os sliders em **100%** e restaura o padrão do jogo (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite de velocidade dos caminhos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala limites de velocidade de **Caminhos** (caminhos não são estradas).\n" +
                    "**1.00 = padrão do jogo**\n" +
                    "Afeta: ciclovias, pedestre+bike, e caminhos de pedestres.\n\n" +
                    "Desinstalar: coloque 1.00 ou use reset, salve a cidade, depois desinstale.\n" +
                    "Se esquecer: caminhos antigos ficam alterados, caminhos novos ficam vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupo de bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bikes e scooters elétricos.\n" +
                    "**Ativo** = tem faixa atual (em movimento).\n" +
                    "**Total estacionado** = inclui todas as flags de estacionado (ex: na rua), não só estacionamentos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupo de carros" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Apenas carros pessoais (exclui o Grupo de bikes).\n" +
                    "**Ativo** = tem faixa atual (em movimento).\n" +
                    "**Estacionado** = tem **ParkedCar**.\n" +
                    "A varredura roda só com Options aberto, então sem preocupação de desempenho."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Carros estacionados ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Total na borda OC** = veículos do grupo de carros com ParkedCar na conexão Fora da Cidade (OC).\n" +
                    "Algumas cidades mostram muitos carros presos estacionados na conexão Fora da Cidade.\n" +
                    "Use <Registrar carros ocultos> para um exemplo do detalhamento de carros ocultos.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Registrar carros ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Escreve um relatório único em **Logs/FastBikes.log**.\n" +
                    "Inclui Total + detalhamento Grupo A/B/C e alguns números de ID de exemplo.\n" +
                    "Use o mod Scene Explorer para ir aos IDs de entidade dos veículos e investigar."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status não carregado." },
                { "FAST_STATS_NOT_AVAIL",       "Sem cidade... ¯\\_(ツ)_/¯ ...Sem stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Rode a cidade por alguns minutos para gerar dados." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} ativo | {1} bikes | {2} scooters | {3} / {4} estac./total" },
                { "FAST_STATS_CARS_ROW2",  "{0} ativo | {1} estac. | {2} total | {3} reboques" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "{0} total na borda OC | atualizado {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nome exibido." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Versão atual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Abre a página do autor no Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Relatório debug de bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Relatório detalhado único para debug ou checagem no dia da atualização.\n" +
                    "Não é necessário para o jogo normal.\n" +
                    "Carregue uma cidade primeiro.\n" +
                    "Local de saída: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
