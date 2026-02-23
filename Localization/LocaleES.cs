// File: Localization/LocaleES.cs
// Purpose: Spanish entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleES : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleES(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Velocidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Estabilidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Restablecer" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Estado vehículos personales" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Caminos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info del mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Depuración" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Activa el mod **ON/OFF**.\n" +
                    "En OFF, el comportamiento de bicis y scooters vuelve al juego.\n\n" +
                    "El estado de abajo sigue disponible aunque Enable Fast Bikes esté en OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidad bici y scooter" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala la velocidad máx**.\n" +
                    "**0.30 = 30%** del juego\n" +
                    "**1.00 = juego por defecto**\n" +
                    "Nota: límites de carretera y condiciones del juego aplican."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Factor para **amplitud de balanceo**.\n" +
                    "**Más alto = menos inclinación**.\n" +
                    "**Más bajo = más wobble**.\n" +
                    "Sugerido: 1.25–1.75 para estabilidad a alta velocidad."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortiguación" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Más alto = se estabiliza antes (menos oscilación).\n" +
                    "**1.00 = juego por defecto**\n" +
                    "Sugerido: 1.25–2.00 para estabilidad a alta velocidad."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valores del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica los valores por defecto del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valores del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Pone todos los sliders a **100%** y restaura el juego (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Límite velocidad caminos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala los límites de velocidad de **Caminos** (no son carreteras).\n" +
                    "**1.00 = juego por defecto**\n" +
                    "Afecta: carriles bici, peatón+bici, y caminos peatonales.\n\n" +
                    "Desinstalar: poner 1.00 o usar reset, guardar ciudad y desinstalar.\n" +
                    "Si se olvida: caminos viejos mantienen la velocidad, nuevos = vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupo bicis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bicis y scooters eléctricos.\n" +
                    "**Activo** = tiene carril actual (en movimiento).\n" +
                    "**Total aparcados** = incluye todas las flags Parked (ej: borde de calle), no solo parkings."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupo coches" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Solo coches personales (excluye el grupo bicis).\n" +
                    "**Activo** = tiene carril actual (en movimiento).\n" +
                    "**Aparcado** = tiene **ParkedCar**.\n" +
                    "Nota: el panel del juego no cuenta todos los tipos, por eso sale menos.\n" +
                    "El scan solo corre con Options abierto, sin impacto en fps."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Coches ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Total en borde OC** = vehículos del grupo coche con **ParkedCar + Unspawned** en la conexión Outside City (OC).\n" +
                    "La línea de estado muestra el total.\n" +
                    "Usar <Log hidden cars> para Buckets A/B/C e IDs.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Log coches ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Escribe un informe único en **Logs/FastBikes.log**.\n" +
                    "Incluye Total + Buckets A/B/C y muestras head+tail.\n" +
                    "Usar el mod Scene Explorer para ir a los IDs de vehículos."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Estado no cargado." },
                { "FAST_STATS_NOT_AVAIL",       "Sin ciudad... ¯\\_(ツ)_/¯ ...Sin stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Deja correr la ciudad unos minutos." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} activo | {1} bicis | {2} scooters | {3} / {4} aparc./total" },
                { "FAST_STATS_CARS_ROW2",  "{0} activo | {1} aparc. | {2} total | {3} remolques" },
                { "FAST_STATS_CARS_ROW3",  "Coches ocultos {0} total en borde OC | act {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nombre mostrado." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Versión actual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Abre la página de mods del autor en Paradox." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Informe debug bicis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Informe único para depuración.\n" +
                    "Primero cargar una ciudad.\n" +
                    "Salida: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
