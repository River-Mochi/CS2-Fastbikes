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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "アクション" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "概要" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "安定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "リセット" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "パス" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "Mod 情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "デバッグ" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes を有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Mod を **ON/OFF（オン/オフ）** します。\n" +
                    "OFF の場合、自転車とスクーターの挙動はゲームのデフォルトに戻ります。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自転車 & スクーター速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**最高速度を倍率で調整**\n" +
                    "高速時は、加速と減速が滑らかになる計算式を使用します。\n" +
                    "**0.30 = デフォルトの 30%**\n" +
                    "**1.00 = ゲームのデフォルト**\n" +
                    "注: 道路の制限速度やゲーム内条件が適用される場合があります。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**揺れ幅（スウェイ）** の倍率です。\n" +
                    "**高い = 傾きが少ない**（締まった見た目）。\n" +
                    "**低い = ふらつきが増える。**\n" +
                    "注: スクーターはデフォルト値が異なるため、より傾く場合があります。\n" +
                    "- 高速で安定させたい場合: 1.25–1.75。\n" +
                    "- ふらつきを増やしたい場合: < 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "減衰" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "高い = 収束が速い（振動が早く消える）。\n" +
                    "**1.0 = ゲームのデフォルト**\n" +
                    "- 高速で安定させたい場合: 1.25–2.0+\n" +
                    "- ふらつきを増やしたい場合: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Mod デフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Mod の推奨デフォルト値を適用します。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "ゲーム デフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "すべてのスライダーを **100%** に戻し、ゲームのデフォルト（vanilla）を復元します。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "パス速度制限" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**パス** の速度制限を倍率で調整します（パスは道路ではありません）。\n" +
                    "**1.00 = ゲームのデフォルト**\n" +
                    "対象: 自転車道、歩行者+自転車の分離パス、歩行者パス。\n" +
                    "アンインストールするには、これを 1.00（および全値）に戻して都市を読み込み、パス速度制限を復元してください。\n" +
                    "その後、安全に Mod を削除できます。この手順を省略した場合、\n" +
                    "既存のパスは現在の速度制限のままになり、新しいパスは vanilla のデフォルトになります。"
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "個人車両ステータス" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自転車グループ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自転車と電動スクーター。\n" +
                    "**稼働中** = 現在のレーンを持つエンティティ（移動中）。\n" +
                    "**駐車中** = **ParkedCar** を持つエンティティ。\n" +
                    "自転車グループは prefab の **BicycleData** で判定されます。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "車グループ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "個人用自動車のみ（上の自転車グループは除外）。\n" +
                    "**稼働中** = 現在のレーンを持つエンティティ（移動中）。\n" +
                    "**駐車中** = **ParkedCar** を持つエンティティ（路肩/縁石も含む）。\n" +
                    "注: 駐車場だけを数えるゲーム内パネルとは一致しません（こちらはすべての駐車をカウント）。\n" +
                    "スキャンはオプションメニュー表示中のみ実行されます（都市内で毎フレームは動かさず、性能優先）。"
                },

                { "FAST_STATUS_NOT_LOADED", "ステータスが読み込まれていません。" },
                { "FAST_STATS_NOT_AVAIL", "都市がありません... ¯\\_(ツ)_/¯ ...統計なし" },
                { "FAST_STATS_CARS_NOT_AVAIL", "データ取得のため、都市を数分動かしてください。" },

                { "FAST_STATS_BIKES_ROW1", "{0} 稼働中 | {1} 自転車 | {2} スクーター | {3} / {4} 駐車/合計" },
                { "FAST_STATS_CARS_ROW2",  "{0} 稼働中 | {1} 駐車中 | {2} 合計 | 更新 {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "表示名。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "現在のバージョン。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自転車デバッグレポート" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "自転車関連の値を 1 回だけログに出力します。\n" +
                    "通常のプレイには不要です。\n\n" +
                    "ゲーム更新後の prefab 確認やデバッグに役立ちます。\n" +
                    "クリック前に都市を読み込んでください。出力先: **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
