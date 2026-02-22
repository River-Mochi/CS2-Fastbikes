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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Estado vehículos" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Caminos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Info del mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Enciende/apaga el mod **ON/OFF**.\n" +
                    "Si OFF, bicis y patinetes vuelven a lo normal del juego.\n\n" +
                    "El estado abajo funciona incluso con Activar Fast Bikes en OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidad bici y patín" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala la velocidad máx**.\n" +
                    "**0.30 = 30%** del juego\n" +
                    "**1.00 = juego**\n" +
                    "Nota: límites de carretera y condiciones del juego aún aplican."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escala de **amplitud de sway**.\n" +
                    "**Más alto = menos inclinación**.\n" +
                    "**Más bajo = más wobble**.\n" +
                    "Sugerido: 1.25–1.75 para estabilidad a alta velocidad."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortiguación" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Más alto = se asienta más rápido (menos oscilación).\n" +
                    "**1.00 = juego**\n" +
                    "Sugerido: 1.25–2.00 para estabilidad a alta velocidad."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valores del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica los valores por defecto del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valores del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Pone todos los sliders en **100%** y restaura el juego (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Límite en caminos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala límites de velocidad de **Caminos** (caminos no son carreteras).\n" +
                    "**1.00 = juego**\n" +
                    "Afecta: carriles bici, peatonal+bici, y caminos peatonales.\n\n" +
                    "Desinstalar: poner en 1.00 o usar reset, guardar ciudad, luego desinstalar.\n" +
                    "Si lo olvidas: caminos viejos quedan con velocidad mod y los nuevos son vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bicis y patinetes eléctricos.\n" +
                    "**Activo** = tiene carril actual (moviéndose).\n" +
                    "**Total aparcados** = incluye todos los flags Parked (p.ej., en la calle), no solo parkings."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupo coche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Coches personales (excluye Grupo bici).\n" +
                    "**Activo** = tiene carril actual (moviéndose).\n" +
                    "**Aparcado** = tiene **ParkedCar**.\n" +
                    "Nota: el panel del juego no cuenta todo, por eso da menos.\n" +
                    "El scan corre solo con Opciones abierto, sin impacto serio."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Coches ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Ocultos en borde** = coches aparcados justo fuera del borde en conexión Outside City (OC).\n" +
                    "No se ven en juego y CUENTAN dentro del total de aparcados.\n" +
                    "Algunas ciudades muestran muchos OC ligados a Owners dentro de la ciudad.\n" +
                    "Falta investigar: ¿staging del juego u otra cosa?\n\n" +
                    "Si tienes curiosidad: botón <Registrar coches ocultos> para escribir IDs de ejemplo en el log.\n" +
                    "Luego mira esos IDs con Scene Explorer y comparte. ¿Pueden los cims usar esos coches?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Registrar coches ocultos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Escribe un reporte único en **Logs/FastBikes.log** con muestras (inicio+fin).\n" +
                    "Usa Scene Explorer: Jump To a los IDs de Vehicle."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Estado no cargado." },
                { "FAST_STATS_NOT_AVAIL",       "Sin ciudad... ¯\\_(ツ)_/¯ ...sin datos" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "deja correr la ciudad unos minutos." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} activos | {1} bicis | {2} patin. | {3} / {4} aparc/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} activos | {1} aparc | {2} total | {3} remolques" },
                { "FAST_STATS_CARS_ROW3",  "{0} ocultos en borde OC | act {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nombre mostrado." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Versión actual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Abre la página de mods del autor en Paradox." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Reporte debug bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Reporte único para debug.\n" +
                    "Carga una ciudad primero.\n" +
                    "Salida: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
