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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "Réinitialiser" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "Statut véhicules perso" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "Chemins" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Infos du mod" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "Liens" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "Débogage" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Activer Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Active le mod **ON/OFF**.\n" +
                    "Quand OFF, le comportement vélos et trottinettes revient aux valeurs du jeu.\n\n" +
                    "Le statut ci-dessous reste disponible même si Enable Fast Bikes est OFF."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "Vitesse vélo & scooter" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**Ajuste la vitesse max**.\n" +
                    "**0.30 = 30%** du jeu\n" +
                    "**1.00 = jeu par défaut**\n" +
                    "Note : limites de route et conditions du jeu s’appliquent."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "Rigidité" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "Facteur pour **l’amplitude de balancement**.\n" +
                    "**Plus haut = moins d’inclinaison**.\n" +
                    "**Plus bas = plus de wobble**.\n" +
                    "Suggéré : 1.25–1.75 pour la stabilité à grande vitesse."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "Amortissement" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "Plus haut = se stabilise plus vite (moins d’oscillation).\n" +
                    "**1.00 = jeu par défaut**\n" +
                    "Suggéré : 1.25–2.00 pour la stabilité à grande vitesse."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Valeurs du mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Applique les valeurs par défaut du mod."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "Valeurs du jeu" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "Met tous les curseurs à **100%** et restaure le jeu (vanilla)."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "Limite vitesse chemins" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "Ajuste les limites de vitesse des **Chemins** (les chemins ne sont pas des routes).\n" +
                    "**1.00 = jeu par défaut**\n" +
                    "Affecte : pistes cyclables, piéton+vélo, et chemins piétons.\n\n" +
                    "Désinstallation : remettre à 1.00 ou utiliser reset, sauvegarder, puis désinstaller.\n" +
                    "Sinon : anciens chemins gardent la vitesse modifiée, nouveaux chemins = vanilla."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "Groupe vélos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "Vélos et trottinettes électriques.\n" +
                    "**Actif** = a une voie actuelle (en mouvement).\n" +
                    "**Total garés** = inclut tous les indicateurs Parked (ex : bord de route), pas seulement les parkings."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "Groupe voitures" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "Voitures personnelles uniquement (exclut le groupe vélos).\n" +
                    "**Actif** = a une voie actuelle (en mouvement).\n" +
                    "**Garé** = a **ParkedCar**.\n" +
                    "Note : le panneau du jeu ne compte pas tous les types, donc chiffre plus bas.\n" +
                    "Scan uniquement quand Options est ouvert, donc aucun souci perf."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "Voitures cachées" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**Total à la frontière OC** = véhicules du groupe voiture avec **ParkedCar + Unspawned** à la connexion Outside City (OC).\n" +
                    "La ligne de statut affiche le total.\n" +
                    "Utiliser <Log hidden cars> pour Buckets A/B/C et IDs.\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "Log voitures cachées" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "Écrit un rapport unique dans **Logs/FastBikes.log**.\n" +
                    "Inclut Total + Buckets A/B/C et échantillons head+tail.\n" +
                    "Utiliser le mod Scene Explorer pour aller aux IDs de véhicules."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "Statut non chargé." },
                { "FAST_STATS_NOT_AVAIL",       "Pas de ville... ¯\\_(ツ)_/¯ ...Pas de stats" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "Laisser tourner la ville quelques minutes." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} actif | {1} vélos | {2} scooters | {3} / {4} garés/total" },
                { "FAST_STATS_CARS_ROW2",  "{0} actif | {1} garés | {2} total | {3} remorques" },
                { "FAST_STATS_CARS_ROW3",  "Voitures cachées {0} total à la frontière OC | maj {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "Nom affiché." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "Version" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "Version actuelle." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "Ouvre la page Paradox Mods de l’auteur." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "Rapport debug vélos" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "Rapport unique pour débogage.\n" +
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
