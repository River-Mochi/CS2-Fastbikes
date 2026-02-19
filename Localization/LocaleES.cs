// File: Localization/LocaleES.cs
// Purpose: Spanish entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Acciones" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Velocidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Estabilidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Restablecer" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Caminos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Información del mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "Depuración" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Activa/desactiva el mod **ON/OFF**.\n" +
                    "Cuando está OFF, el comportamiento de bicicletas y patinetes vuelve a los valores por defecto del juego."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidad de bici y patinete" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala la velocidad máxima**\n" +
                    "Se usa una fórmula de aceleración y frenado más suave a altas velocidades.\n" +
                    "**0.30 = 30%** del valor por defecto del juego\n" +
                    "**1.00 = valor por defecto**\n" +
                    "Nota: los límites de velocidad de las carreteras y las condiciones del juego aún pueden aplicarse."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escalar para la **amplitud del balanceo**.\n" +
                    "**Más alto = menos inclinación** (se ve más firme).\n" +
                    "**Más bajo = más bamboleo.**\n" +
                    "Nota: los patinetes pueden inclinarse más porque sus valores por defecto son diferentes.\n" +
                    "- Para más estabilidad a alta velocidad: 1.25–1.75.\n" +
                    "- Para más bamboleo: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortiguación" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Más alto = se asienta más rápido (la oscilación desaparece antes).\n" +
                    "**1.0 = valores por defecto del juego**\n" +
                    "- Para más estabilidad a alta velocidad: 1.25–2.0+\n" +
                    "- Para más bamboleo: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valores del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica los valores recomendados por defecto del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valores del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Restablece todos los deslizadores a **100%** y restaura los valores por defecto del juego (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Límite de velocidad en caminos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala los límites de velocidad de los **Caminos** (los caminos no son carreteras).\n" +
                    "**1.00 = valor por defecto del juego**\n" +
                    "Afecta: carriles bici, caminos peatonal+bici, y caminos peatonales.\n" +
                    "Para desinstalar el mod, vuelve esto a 1.00 (y todos los valores), carga la ciudad, lo que restaura los límites de velocidad de los caminos.\n" +
                    "Luego el mod se puede desinstalar de forma segura. Si omitiste este paso,\n" +
                    "los caminos existentes mantienen los límites actuales, y todos los nuevos caminos usan límites vanilla."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Estado de vehículos personales" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Grupo bici" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Bicicletas y patinetes eléctricos.\n" +
                    "**Activo** = entidades con un carril actual (en movimiento).\n" +
                    "**Aparcado** = entidades con **ParkedCar**.\n" +
                    "El grupo bici se filtra por **BicycleData** en el prefab."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Grupo coche" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Solo coches personales (excluye el grupo bici de arriba).\n" +
                    "**Activo** = entidades con un carril actual (en movimiento).\n" +
                    "**Aparcado** = entidades con **ParkedCar** (incluye junto a la calzada).\n" +
                    "Nota: no coincidirá con el panel del juego porque contamos todos los aparcados, no solo los de aparcamientos.\n" +
                    "El escaneo se ejecuta solo mientras el menú de Opciones está abierto (no por frame en la ciudad, para mejor rendimiento)."
                },

                { "FAST_STATUS_NOT_LOADED", "Estado no cargado." },
                { "FAST_STATS_NOT_AVAIL", "Sin ciudad... ¯\\_(ツ)_/¯ ...Sin estadísticas" },
                { "FAST_STATS_CARS_NOT_AVAIL", "Deja la ciudad unos minutos para obtener datos." },

                { "FAST_STATS_BIKES_ROW1", "{0} activos | {1} bicis | {2} patinete | {3} / {4} aparcados/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} activos | {1} aparcados | {2} total | actualizado {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "Nombre mostrado." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "Versión actual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre la página de mods del autor en Paradox." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Informe debug de bicis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Informe único en el log con valores relevantes de bicicletas.\n" +
                    "No es necesario para el juego normal.\n\n" +
                    "Útil para verificar prefabs tras actualizaciones del juego o al depurar.\n" +
                    "Carga una ciudad antes de pulsar; datos enviados a **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
