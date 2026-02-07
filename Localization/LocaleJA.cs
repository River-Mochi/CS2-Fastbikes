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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "情報" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "安定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "リセット" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "Mod情報" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "リンク" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "デバッグ" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes を有効化" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "Mod を ON/OFF します。\n" +
                    "OFF のとき、自転車と電動キックボードの挙動が元に戻ります。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自転車・キックボードの速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**最高速度を倍率で変更**\n" +
                    "選択した速度に合わせて加速とブレーキも調整されます。\n" +
                    "**0.30 = ゲーム標準の30%**\n" +
                    "**1.00 = ゲーム標準**\n" +
                    "注意: 道路/経路の速度制限やゲーム状況の影響は引き続き適用される場合があります。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**揺れ（傾き）の大きさ**の倍率です。\n" +
                    "高い = 傾きが小さい（見た目が引き締まる）。\n" +
                    "低い = ふらつきが大きい。\n" +
                    "注意: キックボードは標準値が異なるため、より傾くことがあります。\n" +
                    "高速で安定: 1.25–1.75。\n" +
                    "ふらつきを増やす: 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "減衰" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "高い = 収まりが早い（振動が早く消える）。\n" +
                    "**1.0 = ゲーム標準**\n" +
                    "高速で安定: 1.25–2.0+\n" +
                    "ふらつきを増やす: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "Modのデフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "Mod のデフォルト調整値を適用します。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "ゲームのデフォルト" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "すべてのスライダーを **100%** に戻し、ゲーム標準に復元します。"
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "Mod" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "表示名。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "バージョン" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "現在のバージョン。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "作者の Paradox Mods ページを開きます。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自転車Prefabダンプ" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "自転車/キックボードの詳細値をログに出力します。\n" +
                    "通常プレイには不要です。\n\n" +
                    "ゲーム更新後や問題のデバッグ時に役立ちます。\n" +
                    "先に都市を読み込み、データは **FastBikes.log** に出力されます。"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
