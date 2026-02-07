// File: Localization/LocaleFR.cs
// Purpose: French entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleFR : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleFR(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "Vitesse" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilité" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "Réinitialiser" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Infos du mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Débogage" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activer Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Active/désactive le mod (ON/OFF).\n" +
                    "Lorsque c’est OFF, le comportement des vélos et des trottinettes est restauré."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Vitesse vélos & trottinettes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Multiplie la vitesse de pointe**\n" +
                    "L’accélération et le freinage sont aussi ajustés selon la vitesse choisie.\n" +
                    "**0.30 = 30%** de la valeur par défaut du jeu\n" +
                    "**1.00 = valeur par défaut du jeu**\n" +
                    "Note : les limites de vitesse des voies et les conditions du jeu peuvent encore s’appliquer."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidité" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Multiplicateur de l’**amplitude de balancement**.\n" +
                    "Plus élevé = moins d’inclinaison (aspect plus “serré”).\n" +
                    "Plus faible = plus d’oscillation.\n" +
                    "Note : les trottinettes peuvent encore plus s’incliner car leurs valeurs par défaut diffèrent.\n" +
                    "Plus stable à grande vitesse : 1.25–1.75.\n" +
                    "Plus d’oscillation : 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortissement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Plus élevé = se stabilise plus vite (l’oscillation disparaît plus vite).\n" +
                    "**1.0 = valeurs par défaut du jeu**\n" +
                    "Plus stable à grande vitesse : 1.25–2.0+\n" +
                    "Plus d’oscillation : < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valeurs par défaut du mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applique les valeurs de réglage par défaut du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valeurs par défaut du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Remet tous les curseurs à **100%** et restaure les valeurs par défaut du jeu."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "Nom affiché." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "Version actuelle." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Ouvre la page Paradox Mods de l’auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Dump des prefabs de vélos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Journalise des valeurs détaillées des vélos/trottinettes.\n" +
                    "Inutile pour une partie normale.\n\n" +
                    "Utile après des mises à jour du jeu ou pour diagnostiquer des problèmes.\n" +
                    "Charger une ville d’abord, données écrites dans **FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
