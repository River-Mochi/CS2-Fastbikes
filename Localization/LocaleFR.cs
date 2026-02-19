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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "Actions" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "Vitesse" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "Stabilité" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "Réinitialisation" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "Chemins" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Infos du mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "Débogage" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activer Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Active/désactive le mod **ON/OFF**.\n" +
                    "Quand OFF, le comportement des vélos et des trottinettes est restauré aux valeurs par défaut du jeu."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Vitesse vélos & trottinettes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Multiplie la vitesse max**\n" +
                    "Une formule d’accélération et de freinage plus douce est utilisée aux vitesses élevées.\n" +
                    "**0.30 = 30%** de la valeur par défaut du jeu\n" +
                    "**1.00 = valeur par défaut**\n" +
                    "Note : les limites de vitesse des routes et les conditions du jeu peuvent toujours s’appliquer."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidité" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Coefficient pour **l’amplitude du balancement**.\n" +
                    "**Plus élevé = moins d’inclinaison** (aspect plus stable).\n" +
                    "**Plus bas = plus de tangage.**\n" +
                    "Note : les trottinettes peuvent encore plus pencher car leurs valeurs par défaut sont différentes.\n" +
                    "- Pour plus de stabilité à grande vitesse : 1.25–1.75.\n" +
                    "- Pour plus de tangage : < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortissement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Plus élevé = se stabilise plus vite (l’oscillation s’éteint plus rapidement).\n" +
                    "**1.0 = valeurs par défaut du jeu**\n" +
                    "- Pour plus de stabilité à grande vitesse : 1.25–2.0+\n" +
                    "- Pour plus de tangage : < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valeurs du mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applique les valeurs de réglage par défaut du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valeurs du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Remet tous les curseurs à **100%** et restaure les valeurs par défaut du jeu (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite de vitesse des chemins" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Multiplie les limites de vitesse des **Chemins** (les chemins ne sont pas des routes).\n" +
                    "**1.00 = valeur par défaut du jeu**\n" +
                    "Affecte : pistes cyclables, chemins piétons+vélos, et chemins piétons.\n" +
                    "Pour désinstaller le mod, remettez ceci à 1.00 (et toutes les valeurs), chargez la ville, ce qui restaure les limites de vitesse des chemins.\n" +
                    "Ensuite, le mod peut être désinstallé en toute sécurité. Si vous avez ignoré cette étape,\n" +
                    "les chemins existants conservent les limites actuelles, et tous les nouveaux chemins utilisent les limites vanilla."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "Statut des véhicules personnels" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Groupe vélo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Vélos et trottinettes électriques.\n" +
                    "**Actif** = entités avec une voie actuelle (en mouvement).\n" +
                    "**Stationné** = entités avec **ParkedCar**.\n" +
                    "Le groupe vélo est filtré par **BicycleData** sur le prefab."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Groupe voiture" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Voitures personnelles uniquement (exclut le groupe vélo ci-dessus).\n" +
                    "**Actif** = entités avec une voie actuelle (en mouvement).\n" +
                    "**Stationné** = entités avec **ParkedCar** (inclut le stationnement en bord de route).\n" +
                    "Note : ne correspondra pas aux chiffres de stationnement du panneau du jeu car nous comptons tous les véhicules stationnés, pas seulement ceux dans un parking.\n" +
                    "Le scan s’exécute uniquement lorsque le menu Options est ouvert (pas à chaque frame en ville, pour de meilleures performances)."
                },

                { "FAST_STATUS_NOT_LOADED", "Statut non chargé." },
                { "FAST_STATS_NOT_AVAIL", "Pas de ville... ¯\\_(ツ)_/¯ ...Pas de stats" },
                { "FAST_STATS_CARS_NOT_AVAIL", "Laissez tourner la ville quelques minutes pour obtenir des données." },

                { "FAST_STATS_BIKES_ROW1", "{0} actifs | {1} vélos | {2} trottinette | {3} / {4} stationnés/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} actifs | {1} stationnés | {2} total | mis à jour {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "Nom affiché." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "Version actuelle." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "Ouvre la page Paradox Mods de l’auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Rapport debug vélos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Rapport de log ponctuel sur les valeurs pertinentes des vélos.\n" +
                    "Inutile pour le gameplay normal.\n\n" +
                    "Utile pour vérifier les prefabs après une mise à jour du jeu ou lors du débogage.\n" +
                    "Chargez une ville avant de cliquer ; données envoyées vers **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
