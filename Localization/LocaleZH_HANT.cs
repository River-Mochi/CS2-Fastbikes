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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "重設" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "個人車輛狀態" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "路徑" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "模組資訊" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "連結" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "除錯" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "啟用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "將模組設為 **開 / 關**。\n" +
                    "關閉時，自行車與電動滑板車行為會恢復為遊戲預設。\n\n" +
                    "即使啟用 Fast Bikes 為關閉，下方狀態資訊仍可查看。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行車與滑板車速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**按比例調整最高速度**。\n" +
                    "**0.30 = 遊戲預設的 30%**\n" +
                    "**1.00 = 遊戲預設**\n" +
                    "注意：道路限速與遊戲條件仍會生效。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "剛性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**搖擺幅度** 的比例。\n" +
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
                    "將所有滑桿設為 **100%** 並恢復遊戲預設（原版）。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "路徑限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "按比例調整 **路徑(Path)** 限速（路徑不是道路）。\n" +
                    "**1.00 = 遊戲預設**\n" +
                    "影響：自行車道、行人+自行車共用道、行人道。\n\n" +
                    "解除安裝：設為 1.00 或用重設按鈕，儲存城市後再解除安裝。\n" +
                    "若忘了：舊路徑保留已修改限速，新建路徑使用原版預設。"
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自行車組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自行車與電動滑板車。\n" +
                    "**活躍** = 有目前車道（在移動）。\n" +
                    "**總停放** = 包含所有停放標記（如路邊），不只停車場。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "汽車組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "僅個人汽車（不含自行車組）。\n" +
                    "**活躍** = 有目前車道（在移動）。\n" +
                    "**停放** = 具有 **ParkedCar**。\n" +
                    "掃描只在開啟 Options 時執行，不用擔心城內 fps。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "隱藏停放車輛" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**OC 邊界總數** = 汽車組中在 城市外(OC) 連線處停放（ParkedCar）的車輛。\n" +
                    "有些城市會出現大量車輛卡在 城市外 連線處停放。\n" +
                    "使用 <記錄隱藏車輛> 查看隱藏車輛的樣本分解。\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "記錄隱藏車輛" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "寫入一次性報告到 **Logs/FastBikes.log**。\n" +
                    "包含 總數 + A/B/C 分組明細 與示例 ID。\n" +
                    "用 Scene Explorer mod 跳到列出的車輛實體 ID 進行調查。"
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "狀態未載入。" },
                { "FAST_STATS_NOT_AVAIL",       "沒有城市... ¯\\_(ツ)_/¯ ...沒有統計" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "讓城市運行幾分鐘以生成資料。" },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 活躍 | {1} 自行車 | {2} 滑板車 | {3} / {4} 停放/總計" },
                { "FAST_STATS_CARS_ROW2",  "{0} 活躍 | {1} 停放 | {2} 總計 | {3} 拖車" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "OC 邊界總數 {0} | 更新 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "模組" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "顯示名稱。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "目前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "開啟作者的 Paradox Mods 頁面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行車除錯報告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性詳細日誌，用於除錯或更新日驗證。\n" +
                    "一般遊玩不需要。\n" +
                    "請先載入城市。\n" +
                    "輸出位置：**Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
