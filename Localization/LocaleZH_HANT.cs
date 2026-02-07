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

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "模組資訊" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "除錯" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "啟用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "開啟/關閉模組。\n" +
                    "關閉後，自行車與電動滑板車的行為會恢復為遊戲預設。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行車與滑板車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**縮放最高速度**\n" +
                    "加速度與煞車也會依所選速度自動調整。\n" +
                    "**0.30 = 遊戲預設的30%**\n" +
                    "**1.00 = 遊戲預設**\n" +
                    "注意：道路/路徑限速與遊戲條件仍可能適用。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**擺動幅度**的倍率。\n" +
                    "更高 = 更少傾斜（更緊實的觀感）。\n" +
                    "更低 = 更搖晃。\n" +
                    "注意：滑板車的預設值不同，仍可能更容易傾斜。\n" +
                    "高速更穩定：1.25–1.75。\n" +
                    "更搖晃：0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快穩定（振盪更快消失）。\n" +
                    "**1.0 = 遊戲預設值**\n" +
                    "高速更穩定：1.25–2.0+\n" +
                    "更搖晃：< 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模組預設" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "套用模組的預設調校數值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "遊戲預設" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "將所有滑桿重設為 **100%** 並還原為遊戲預設。"
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行車預製體Dump" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "記錄自行車/電動滑板車的詳細數值。\n" +
                    "一般遊玩不需要。\n\n" +
                    "在遊戲更新後或除錯問題時很有用。\n" +
                    "請先載入城市；資料會寫入 **FastBikes.log**"
                },
            };
        }

        public void Unload()
        {
        }
    }
}
