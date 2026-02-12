// File: Localization/LocaleJA.cs
// Purpose: Japanese entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleJA : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleJA(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "アクション" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "について" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "安定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "リセット" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "パス" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Mod情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "デバッグ" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes を有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "ModをON/OFF。\n" +
                    "OFF のとき、自転車とスクーターはゲーム標準に戻ります。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自転車＆スクーター速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**最高速度を倍率で変更**\n" +
                    "高速時は加速/減速がなめらかになる式を使います。\n" +
                    "**0.30 = 標準の30%**\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "注意：制限速度やゲーム条件の影響は残ります。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**揺れ幅**の倍率。\n" +
                    "**高い = 傾きが少ない**（キュッとした見た目）。\n" +
                    "**低い = ふらつき多め。**\n" +
                    "注意：スクーターは標準値が違うので傾きが大きめです。\n" +
                    "高速で安定：1.25–1.75。\n" +
                    "ふらつき増：< 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "減衰" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "高い = 早く落ち着く（揺れが早く消える）。\n" +
                    "**1.0 = ゲーム標準**\n" +
                    "高速で安定：1.25–2.0+\n" +
                    "ふらつき増：< 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Modデフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Modのデフォルト設定を適用します。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "ゲームデフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "全スライダーを **100%** に戻し、ゲーム標準に復元します。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "パスの制限速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**パス**の制限速度を倍率で変更（パスは道路ではありません）。\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "対象：自転車道、歩行者+自転車分離、歩行者専用パス。\n" +
                    "新Beta機能：GitHub/フォーラムでフィードバック歓迎。"
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "表示名。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "現在のバージョン。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "作者のParadox Modsページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自転車デバッグレポート" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "自転車/スクーターの詳細Prefab値を1回だけログに出力します。\n" +
                    "普段のプレイでは不要です。\n\n" +
                    "ゲーム更新後の確認やデバッグに便利です。\n" +
                    "先に都市をロードしてからクリック；出力先 **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
