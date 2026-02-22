// File: Localization/LocaleZH_HANT.cs
// Purpose: Traditional Chinese entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "關於" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "穩定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "重置" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "私人車輛狀態" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "路徑" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "模組資訊" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "除錯" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "啟用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "將模組切換為 **開/關**。\n" +
                    "關閉時，自行車與電動滑板車行為會回到遊戲預設。\n\n" +
                    "即使 啟用 Fast Bikes 為關閉，下方狀態仍可使用。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行車/滑板車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "縮放 **最高速度**。\n" +
                    "**0.30 = 30%** 遊戲預設\n" +
                    "**1.00 = 遊戲預設**\n" +
                    "注意：仍受道路限速與遊戲狀況影響。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**擺動幅度** 倍率。\n" +
                    "**更高 = 更少傾斜**。\n" +
                    "**更低 = 更晃**。\n" +
                    "建議：高速穩定 1.25–1.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快穩定（更少振盪）。\n" +
                    "**1.00 = 遊戲預設**\n" +
                    "建議：高速穩定 1.25–2.00。"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模組預設" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "套用模組的預設調校值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "遊戲預設" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "把所有滑桿設為 **100%** 並恢復遊戲預設（vanilla）。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "路徑限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "縮放 **路徑** 的限速（路徑不是道路）。\n" +
                    "**1.00 = 遊戲預設**\n" +
                    "影響：自行車道、步行+自行車、步行路徑。\n\n" +
                    "解除安裝：設為 1.00 或用重置按鈕，存檔後再解除安裝。\n" +
                    "忘了也沒事：舊路徑保留改過速度，新路徑用遊戲預設。"
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自行車組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自行車與電動滑板車。\n" +
                    "**活動** = 有目前車道（移動中）。\n" +
                    "**總停放** = 包含所有停放標記（如路邊），不只停車場。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "汽車組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "僅私人汽車（不含自行車組）。\n" +
                    "**活動** = 有目前車道（移動中）。\n" +
                    "**停放** = 有 **ParkedCar**。\n" +
                    "注意：遊戲資訊面板不包含所有停放類型，所以數字較低。\n" +
                    "掃描只在開啟選項時執行，不影響城內 fps。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "邊界隱藏車輛" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**邊界隱藏** = 停在邊界外、城外連線（OC）的車輛。\n" +
                    "這些車在遊戲內看不到，但計入總停放數。\n" +
                    "有些城市會有大量 OC 車輛，並綁定城內車主。\n" +
                    "仍需研究：這是遊戲暫存還是別的？\n\n" +
                    "想查：按 <記錄隱藏車輛> 把樣本 ID 寫入日誌。\n" +
                    "再用 Scene Explorer 查看車輛 ID 並分享結果。市民能用這些綁定的車嗎？"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "記錄隱藏車輛" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "一次性寫入 **Logs/FastBikes.log**（開頭+結尾樣本）。\n" +
                    "用 Scene Explorer 跳轉到列出的車輛實體 ID。"
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "狀態未載入。" },
                { "FAST_STATS_NOT_AVAIL",       "沒有城市... ¯\\_(ツ)_/¯ ...沒資料" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "讓城市跑幾分鐘再看。" },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 活動 | {1} 自行車 | {2} 滑板車 | {3} / {4} 停/總" },
                { "FAST_STATS_CARS_ROW2",  "{0} 活動 | {1} 停放 | {2} 總計 | {3} 拖車" },
                { "FAST_STATS_CARS_ROW3",  "{0} 邊界隱藏OC | 更新 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "顯示名稱。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "目前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "開啟作者的 Paradox mods 頁面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行車除錯報告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性除錯日誌。\n" +
                    "先載入城市。\n" +
                    "輸出：**Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
