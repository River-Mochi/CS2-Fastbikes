// File: Localization/LocaleFR.cs
// Purpose: French entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "À propos" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "Vitesse" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "Stabilité" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Reset" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Statut véhicules perso" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Chemins" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Infos du mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Debug" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activer Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Active/désactive le mod **ON/OFF**.\n" +
                    "Quand OFF, les vélos et trottinettes reviennent aux valeurs du jeu.\n\n" +
                    "Le statut ci-dessous est dispo même si Activer Fast Bikes est OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Vitesse vélo & trott." },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Multiplie la vitesse max**.\n" +
                    "**0.30 = 30%** du jeu\n" +
                    "**1.00 = jeu**\n" +
                    "Note : limites de route et conditions du jeu s’appliquent toujours."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidité" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Scalar pour **l’amplitude de sway**.\n" +
                    "**Plus haut = moins d’inclinaison**.\n" +
                    "**Plus bas = plus de wobble**.\n" +
                    "Conseil : 1.25–1.75 pour la stabilité à haute vitesse."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortissement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Plus haut = se stabilise plus vite (moins d’oscillation).\n" +
                    "**1.00 = jeu**\n" +
                    "Conseil : 1.25–2.00 pour la stabilité à haute vitesse."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Défauts du mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applique les valeurs par défaut du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Défauts du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Remet tous les curseurs à **100%** et restaure le jeu (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Vitesse sur chemins" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Multiplie les limites des **Chemins** (les chemins ne sont pas des routes).\n" +
                    "**1.00 = jeu**\n" +
                    "Affecte : pistes vélo, piéton+vélo, et chemins piétons.\n\n" +
                    "Désinstallation : remettre à 1.00 ou utiliser reset, sauvegarder la ville, puis désinstaller.\n" +
                    "Si oublié : les anciens chemins gardent la vitesse modifiée et les nouveaux sont en vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Groupe vélo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Vélos et trottinettes électriques.\n" +
                    "**Actif** = a une voie actuelle (bouge).\n" +
                    "**Total garés** = inclut tous les flags Parked (ex : bord de route), pas seulement les parkings."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Groupe voiture" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Voitures perso uniquement (exclut le groupe vélo).\n" +
                    "**Actif** = a une voie actuelle (bouge).\n" +
                    "**Garé** = a **ParkedCar**.\n" +
                    "Note : le panneau du jeu ne compte pas tout, donc chiffres plus bas.\n" +
                    "Scan uniquement quand Options est ouvert, donc impact perf OK."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Voitures cachées" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Cachées à la frontière** = voitures garées juste hors limite sur connexion Outside City (OC).\n" +
                    "Invisibles en jeu et INCLUSES dans le total de voitures garées.\n" +
                    "Certaines villes ont beaucoup de voitures OC liées à des Owners en ville.\n" +
                    "À étudier : staging du jeu ou autre ?\n\n" +
                    "Si curieux : bouton <Journaliser voitures cachées> pour écrire des IDs exemples dans le log.\n" +
                    "Puis vérifier les IDs avec Scene Explorer et partager. Les cims peuvent-ils utiliser ces voitures ?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Journaliser voitures cachées" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Écrit un petit rapport unique dans **Logs/FastBikes.log** (exemples début+fin).\n" +
                    "Utiliser Scene Explorer : Jump To les IDs d’entités Vehicle."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Statut non chargé." },
                { "FAST_STATS_NOT_AVAIL",       "Pas de ville... ¯\\_(ツ)_/¯ ...pas de stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "laisse tourner la ville qq minutes." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} actifs | {1} vélos | {2} trott. | {3} / {4} garés/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} actifs | {1} garés | {2} total | {3} remorques" },
                { "FAST_STATS_CARS_ROW3",  "{0} cachés à la frontière OC | maj {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nom affiché." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Version actuelle." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Ouvre la page Paradox Mods de l’auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Rapport debug vélo" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Rapport unique pour debug.\n" +
                    "Charger une ville d’abord.\n" +
                    "Sortie : **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
