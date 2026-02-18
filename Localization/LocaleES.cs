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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Acciones" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "Acerca de" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Velocidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Estabilidad" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Caminos" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Info del mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Enciende/apaga el mod (ON/OFF).\n" +
                    "Cuando está OFF, se restauran los valores por defecto del juego."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidad de bici y scooter" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala la velocidad punta**\n" +
                    "Se usa una fórmula de aceleración/frenado más suave a altas velocidades.\n" +
                    "**0.30 = 30%** del valor por defecto\n" +
                    "**1.00 = por defecto del juego**\n" +
                    "Nota: límites de velocidad y condiciones del juego todavía aplican."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escala la **amplitud del balanceo**.\n" +
                    "**Más alto = menos inclinación** (se ve más “firme”).\n" +
                    "**Más bajo = más bamboleo.**\n" +
                    "Nota: los scooters pueden inclinar más por sus valores base.\n" +
                    "Más estable a alta velocidad: 1.25–1.75.\n" +
                    "Más bamboleo: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortiguación" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Más alto = se estabiliza más rápido (la oscilación muere antes).\n" +
                    "**1.0 = por defecto del juego**\n" +
                    "Más estable a alta velocidad: 1.25–2.0+\n" +
                    "Más bamboleo: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valores del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica los valores por defecto del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valores del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Pone todos los sliders en **100%** y restaura los valores del juego."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Límite de velocidad de caminos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Escala los límites de velocidad de **caminos** (los caminos no son carreteras).\n" +
                    "**1.00 = por defecto del juego**\n" +
                    "Afecta: ciclovías, caminos divididos peatón+bici y caminos solo peatón.\n" +
                    "Nueva función Beta: deja feedback en GitHub o en el foro."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nombre mostrado." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versión actual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre la página del autor en Paradox Mods." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Informe de depuración (bicis)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Informe único en el log con valores detallados de bicis/patinetes.\n" +
                    "No es necesario para jugar normalmente.\n\n" +
                    "Útil para comprobar prefabs tras actualizaciones o al depurar.\n" +
                    "Carga una ciudad antes de pulsar; salida en **Logs/FastBikes.log**"
                },

            };
        }

        public void Unload( )
        {
        }
    }
}
