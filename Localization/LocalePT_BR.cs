// File: Localization/LocalePT_BR.cs
// Purpose: Portuguese (Brazil) entries for FastBikes.

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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Status veículos" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Caminhos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info do mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Links" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Ativar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Liga/desliga o mod **ON/OFF**.\n" +
                    "Em OFF, bikes e scooters voltam ao padrão do jogo.\n\n" +
                    "O status abaixo funciona mesmo se Ativar Fast Bikes estiver OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Veloc. bike e scooter" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala a velocidade máx**.\n" +
                    "**0.30 = 30%** do jogo\n" +
                    "**1.00 = jogo**\n" +
                    "Nota: limites de via e condições do jogo ainda valem."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escala para **amplitude de balanço**.\n" +
                    "**Maior = menos inclinação**.\n" +
                    "**Menor = mais wobble**.\n" +
                    "Sugestão: 1.25–1.75 para estabilidade em alta velocidade."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortecimento" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Maior = estabiliza mais rápido (menos oscilação).\n" +
                    "**1.00 = jogo**\n" +
                    "Sugestão: 1.25–2.00 para estabilidade em alta velocidade."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Padrões do mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica os valores padrão do mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Padrões do jogo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Define todos os sliders em **100%** e restaura o jogo (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite nos caminhos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala limites de velocidade de **Caminhos** (caminhos não são ruas).\n" +
                    "**1.00 = jogo**\n" +
                    "Afeta: ciclovias, pedestre+bike, e caminhos de pedestres.\n\n" +
                    "Desinstalar: volte para 1.00 ou use reset, salve a cidade, depois desinstale.\n" +
                    "Se esquecer: caminhos antigos ficam com valor do mod e novos ficam vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupo bike" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bikes e scooters elétricas.\n" +
                    "**Ativo** = tem lane atual (em movimento).\n" +
                    "**Total estacionados** = inclui todas as flags Parked (ex.: na rua), não só estacionamentos."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupo carro" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Carros pessoais (exclui Grupo bike).\n" +
                    "**Ativo** = tem lane atual (em movimento).\n" +
                    "**Estacionado** = tem **ParkedCar**.\n" +
                    "Nota: o painel do jogo não conta todos os tipos de estacionado, então dá números menores.\n" +
                    "O scan roda só com Opções aberto, então sem drama de desempenho."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Carros ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Ocultos na borda** = carros estacionados fora da borda na conexão Outside City (OC).\n" +
                    "Esses carros não aparecem no jogo e fazem PARTE do total de estacionados.\n" +
                    "Algumas cidades mostram muitos carros OC ligados a Owners dentro da cidade.\n" +
                    "Mais estudo: staging do jogo ou outra coisa?\n\n" +
                    "Se quiser investigar: use <Log carros ocultos> para gravar IDs de exemplo no log.\n" +
                    "Depois confira os IDs no Scene Explorer e compartilhe. Cims podem usar esses carros?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Log carros ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Escreve um pequeno relatório único em **Logs/FastBikes.log** com amostras (início+fim).\n" +
                    "Use Scene Explorer: Jump To nos IDs de entidade Vehicle listados."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Status não carregado." },
                { "FAST_STATS_NOT_AVAIL",       "Sem cidade... ¯\\_(ツ)_/¯ ...sem dados" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "roda a cidade uns minutos." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} ativos | {1} bikes | {2} scooters | {3} / {4} estac./total" },
                { "FAST_STATS_CARS_ROW2",  "{0} ativos | {1} estac. | {2} total | {3} reboques" },
                { "FAST_STATS_CARS_ROW3",  "{0} ocultos na borda OC | atual {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nome exibido." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versão" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Versão atual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Abre a página Paradox Mods do autor." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Relatório debug bike" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Relatório único para debug.\n" +
                    "Carregue uma cidade primeiro.\n" +
                    "Saída: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
