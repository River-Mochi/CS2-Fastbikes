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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "安定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "リセット" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "個人車両ステータス" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "歩道/パス" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "Mod情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "デバッグ" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikesを有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Modを **オン / オフ** します。\n" +
                    "オフ時は、自転車とスクーターの挙動がゲーム標準に戻ります。\n\n" +
                    "下のステータスは Enable Fast Bikes がオフでも表示されます。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自転車&スクーター速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**最高速度を倍率調整**。\n" +
                    "**0.30 = 30%**（ゲーム標準）\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "注意：道路制限やゲーム状況は適用されます。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**揺れ幅** の倍率。\n" +
                    "**高い = 傾き少なめ**。\n" +
                    "**低い = ふらつき多め**。\n" +
                    "推奨：高速安定は 1.25–1.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "減衰" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "高いほど早く落ち着く（振動が減る）。\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "推奨：高速安定は 1.25–2.00。"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Modデフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Modのデフォルト調整値を適用します。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "ゲームデフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "全スライダーを **100%** にしてゲーム標準（バニラ）へ戻します。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "パス速度制限" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**パス** の速度制限を倍率調整（パスは道路ではない）。\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "対象：自転車道、歩行者+自転車、歩行者道。\n\n" +
                    "アンインストール：1.00に戻すかリセット→都市保存→アンインストール。\n" +
                    "忘れた場合：既存パスは変更のまま、新規パスはバニラ。"
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "バイクグループ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自転車と電動スクーター。\n" +
                    "**稼働** = 現在のレーンあり（移動中）。\n" +
                    "**総駐車** = すべての駐車フラグ（例：路肩）を含む。駐車場だけではない。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "車グループ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "個人用の車のみ（バイクグループ除外）。\n" +
                    "**稼働** = 現在のレーンあり（移動中）。\n" +
                    "**駐車** = **ParkedCar** を持つ。\n" +
                    "スキャンはOptions表示中のみ。街のfpsには影響しません。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "隠し駐車車両" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**OC境界の合計** = 都市外（OC）接続でParkedCarの車グループ。\n" +
                    "都市によっては都市外接続に駐車で詰まる車が大量に出ることがあります。\n" +
                    "<隠し車両をログ> で隠し車両のサンプル内訳をログに出せます。\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "隠し車両をログ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "**Logs/FastBikes.log** に1回だけレポートを書きます。\n" +
                    "合計 + 区分A/B/Cの内訳とサンプルID番号を含みます。\n" +
                    "Scene Explorer modで一覧の車両エンティティIDへ移動して調査できます。"
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "ステータス未読込。" },
                { "FAST_STATS_NOT_AVAIL",       "都市なし... ¯\\_(ツ)_/¯ ...統計なし" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "数分動かしてデータ取得。" },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 稼働 | {1} 自転車 | {2} スクーター | {3} / {4} 駐車/合計" },
                { "FAST_STATS_CARS_ROW2",  "{0} 稼働 | {1} 駐車 | {2} 合計 | {3} トレーラー" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "OC境界 合計{0} | 更新 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "表示名。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "現在のバージョン。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "作者のParadox Modsページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "バイク詳細ログ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "デバッグやパッチ確認用の詳細ログ（1回だけ）。\n" +
                    "通常プレイでは不要です。\n" +
                    "先に都市をロードしてください。\n" +
                    "出力先: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
