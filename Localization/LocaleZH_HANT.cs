// File: Localization/LocaleZH_HANT.cs
// Purpose: Traditional Chinese entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleZH_HANT : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleZH_HANT(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "穩定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "重設" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "路徑" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "模組資訊" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "除錯" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "啟用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "開啟/關閉模組。\n" +
                    "關閉時，自行車與滑板車行為會恢復為遊戲預設值。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行車/滑板車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**縮放最高速度**\n" +
                    "在高速時使用更平滑的加速/煞車公式。\n" +
                    "**0.30 = 預設值的 30%**\n" +
                    "**1.00 = 遊戲預設值**\n" +
                    "注意：道路限速與遊戲條件仍可能生效。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**擺動幅度** 的倍率。\n" +
                    "**更高 = 更少傾斜**（更「緊」）。\n" +
                    "**更低 = 更搖晃。**\n" +
                    "注意：滑板車預設值不同，可能仍會更傾斜。\n" +
                    "高速更穩：1.25–1.75。\n" +
                    "更搖晃：< 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快穩定（振動更快消失）。\n" +
                    "**1.0 = 遊戲預設值**\n" +
                    "高速更穩：1.25–2.0+\n" +
                    "更搖晃：< 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模組預設值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "套用模組的預設調校數值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "遊戲預設值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "將所有滑桿設回 **100%** 並恢復遊戲預設值。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "路徑限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "縮放 **路徑(Path)** 的限速（路徑不是道路）。\n" +
                    "**1.00 = 遊戲預設值**\n" +
                    "影響：自行車道、行人+自行車分隔、以及純人行路徑。\n" +
                    "新 Beta 功能 — 歡迎在 GitHub 或論壇回饋。"
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "顯示名稱。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "目前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "開啟作者的 Paradox Mods 頁面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行車除錯報告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性輸出到日誌：自行車/滑板車的詳細Prefab數值。\n" +
                    "一般遊玩不需要。\n\n" +
                    "適合在遊戲更新後核對Prefab或進行除錯。\n" +
                    "請先載入城市再點擊；輸出到 **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
