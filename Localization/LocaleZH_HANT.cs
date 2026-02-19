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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "穩定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "重設" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "步道" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "模組資訊" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "除錯" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "啟用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "將模組 **開啟/關閉（開/關）**。\n" +
                    "關閉後，自行車與電動滑板車會恢復為遊戲預設行為。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行車與電動滑板車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**縮放最大速度**\n" +
                    "在高速時使用更平滑的加速與制動公式。\n" +
                    "**0.30 = 預設值的 30%**\n" +
                    "**1.00 = 遊戲預設值**\n" +
                    "注意：道路限速與遊戲條件仍可能套用。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "用於 **擺動幅度** 的倍率。\n" +
                    "**更高 = 更少傾斜**（更緊實的外觀）。\n" +
                    "**更低 = 更搖晃。**\n" +
                    "注意：電動滑板車的預設值不同，可能仍會更傾斜。\n" +
                    "- 高速更穩定：1.25–1.75。\n" +
                    "- 更搖晃：< 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快穩定（振盪更快消失）。\n" +
                    "**1.0 = 遊戲預設值**\n" +
                    "- 高速更穩定：1.25–2.0+\n" +
                    "- 更搖晃：< 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模組預設值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "套用模組建議的預設調校數值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "遊戲預設值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "將所有滑桿恢復為 **100%**，並恢復遊戲預設值（vanilla）。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "步道限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "按倍率縮放 **步道（Path）** 的限速（步道不是道路）。\n" +
                    "**1.00 = 遊戲預設值**\n" +
                    "影響：自行車道、分離的步行+自行車道、步行道。\n" +
                    "卸載模組前，請將此項（以及所有數值）重設為 1.00，然後載入一次城市以恢復步道限速。\n" +
                    "之後即可安全卸載。若跳過此步驟，\n" +
                    "現有步道會保留目前限速，而所有新建步道將使用 vanilla 預設限速。"
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "個人車輛狀態" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自行車組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自行車與電動滑板車。\n" +
                    "**行駛中** = 具有目前車道的實體（正在移動）。\n" +
                    "**停放** = 具有 **ParkedCar** 的實體。\n" +
                    "自行車組由 prefab 上的 **BicycleData** 進行判定。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "汽車組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "僅個人汽車（不包含上方自行車組）。\n" +
                    "**行駛中** = 具有目前車道的實體（正在移動）。\n" +
                    "**停放** = 具有 **ParkedCar** 的實體（包含路邊/路沿）。\n" +
                    "注意：可能與遊戲資訊面板的停放數量不一致，因為此處統計所有停放，而不只停車場內的。\n" +
                    "掃描僅在開啟選項選單時執行（不會在城市中每幀執行，以獲得最佳效能）。"
                },

                { "FAST_STATUS_NOT_LOADED", "狀態尚未載入。" },
                { "FAST_STATS_NOT_AVAIL", "沒有城市... ¯\\_(ツ)_/¯ ...沒有統計" },
                { "FAST_STATS_CARS_NOT_AVAIL", "讓城市運行幾分鐘以取得資料。" },

                { "FAST_STATS_BIKES_ROW1", "{0} 行駛中 | {1} 自行車 | {2} 電動滑板車 | {3} / {4} 停放/總計" },
                { "FAST_STATS_CARS_ROW2",  "{0} 行駛中 | {1} 停放 | {2} 總計 | 更新 {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "顯示名稱。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "目前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "開啟作者的 Paradox Mods 頁面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行車除錯報告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性輸出自行車相關數值到日誌。\n" +
                    "一般遊玩不需要。\n\n" +
                    "用於遊戲更新後驗證 prefab 或進行除錯。\n" +
                    "點擊前請先載入城市；輸出到 **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
