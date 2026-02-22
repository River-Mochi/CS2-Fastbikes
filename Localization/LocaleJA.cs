// File: Localization/LocaleJA.cs
// Purpose: Japanese entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "安定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "リセット" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "個人車両状況" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "パス" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "モッド情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "デバッグ" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes 有効" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "モッドを **ON/OFF** します。\n" +
                    "OFF のとき、自転車と電動スクーターはゲーム標準に戻ります。\n\n" +
                    "下のステータスは Fast Bikes 有効が OFF でも表示されます。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自転車/スクーター速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**最大速度をスケール**。\n" +
                    "**0.30 = 30%**（ゲーム標準）\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "注：道路制限やゲーム状況は適用されます。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**揺れ幅** のスカラー。\n" +
                    "**高い = 傾き少なめ**。\n" +
                    "**低い = wobble多め**。\n" +
                    "おすすめ：高速安定は 1.25–1.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "減衰" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "高い = 収まりが早い（振動が少ない）。\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "おすすめ：高速安定は 1.25–2.00。"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "モッド既定" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "モッドの既定チューニングを適用します。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "ゲーム既定" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "全スライダーを **100%** にしてゲーム標準（vanilla）へ戻します。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "パス速度制限" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**パス** の速度制限をスケール（パスは道路ではありません）。\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "対象：自転車道、歩行+自転車、歩行者道。\n\n" +
                    "アンインストール：1.00に戻すかリセット→都市を保存→削除。\n" +
                    "忘れた場合：既存パスは改造値のまま、新規パスはゲーム標準。"
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自転車グループ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自転車と電動スクーター。\n" +
                    "**稼働** = 現在のレーンあり（移動中）。\n" +
                    "**合計駐車** = 路肩などを含む駐車フラグ全て。駐車場だけではない。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "車グループ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "個人車のみ（自転車グループ除外）。\n" +
                    "**稼働** = 現在のレーンあり（移動中）。\n" +
                    "**駐車** = **ParkedCar** あり。\n" +
                    "注：ゲーム内パネルは全種類の駐車車両を数えないため数が少なめ。\n" +
                    "スキャンはオプション画面を開いている間だけなので負荷は気にしなくてOK。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "隠し駐車車両" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**国境に隠し** = Outside City (OC) 接続の境界外に駐車している車。\n" +
                    "ゲーム内で見えず、総駐車台数に含まれます。\n" +
                    "都市によっては、都市内Ownerに紐づいたOC車が大量にあります。\n" +
                    "要調査：ゲームのステージング？それとも別物？\n\n" +
                    "興味があれば：<隠し車両をログ> ボタンでサンプルIDをログへ。\n" +
                    "Scene Explorer でIDを確認して結果を共有。紐づいた車をcimは使える？"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "隠し車両をログ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "**Logs/FastBikes.log** に小さな一回限りのレポート（先頭+末尾サンプル）を書きます。\n" +
                    "Scene Explorer の Jump To で Vehicle エンティティIDへ。"
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "ステータス未読み込み。" },
                { "FAST_STATS_NOT_AVAIL",       "都市なし... ¯\\_(ツ)_/¯ ...統計なし" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "都市を数分動かしてデータ取得。" },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 稼働 | {1} 自転車 | {2} スクーター | {3} / {4} 駐車/合計" },
                { "FAST_STATS_CARS_ROW2",  "{0} 稼働 | {1} 駐車 | {2} 合計 | {3} トレーラー" },
                { "FAST_STATS_CARS_ROW3",  "{0} 国境OCに隠し駐車 | 更新 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "モッド" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "表示名。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "現在のバージョン。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自転車デバッグ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "デバッグ用の一回ログ。\n" +
                    "先に都市をロード。\n" +
                    "出力: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
