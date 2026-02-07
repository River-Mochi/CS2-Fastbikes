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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Restablecer" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Info del mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Enlaces" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Depuración" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activar Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Activa/desactiva el mod (ON/OFF).\n" +
                    "Cuando está en OFF, se restaura el comportamiento de bicicletas y patinetes."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Velocidad de bici y patinete" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Escala la velocidad máxima**\n" +
                    "La aceleración y el frenado también se ajustan según la velocidad seleccionada.\n" +
                    "**0.30 = 30%** del valor predeterminado del juego\n" +
                    "**1.00 = valor predeterminado del juego**\n" +
                    "Nota: los límites de velocidad de la vía y las condiciones del juego pueden seguir aplicándose."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidez" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Escalar de la **amplitud de balanceo**.\n" +
                    "Más alto = menos inclinación (aspecto más firme).\n" +
                    "Más bajo = más bamboleo.\n" +
                    "Nota: los patinetes aún pueden inclinarse más porque sus valores por defecto son diferentes.\n" +
                    "Más estable a alta velocidad: 1.25–1.75.\n" +
                    "Más bamboleo: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortiguación" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Más alto = se estabiliza más rápido (la oscilación desaparece más rápido).\n" +
                    "**1.0 = valores predeterminados del juego**\n" +
                    "Más estable a alta velocidad: 1.25–2.0+\n" +
                    "Más bamboleo: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valores por defecto del mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Aplica los valores de ajuste predeterminados del mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valores por defecto del juego" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Vuelve a poner todos los deslizadores en **100%** y restaura los valores predeterminados del juego."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nombre mostrado." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Versión" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Versión actual." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Abre la página de mods del autor en Paradox." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Volcado de prefabs de bicicleta" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Registra valores detallados de bicicletas/patinetes.\n" +
                    "No es necesario para el juego normal.\n\n" +
                    "Útil después de actualizaciones del juego o al depurar problemas.\n" +
                    "Carga una ciudad primero; los datos se envían a **FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
