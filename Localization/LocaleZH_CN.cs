// File: Localization/LocaleZH_CN.cs
// Purpose: Simplified Chinese entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleZH_CN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleZH_CN(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "稳定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "重置" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "个人车辆状态" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "路径" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "模组信息" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "链接" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "调试" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "启用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "开关模组 **ON/OFF**。\n" +
                    "OFF 时，自行车和电动滑板车恢复游戏默认。\n\n" +
                    "即使 启用 Fast Bikes 为 OFF，下方状态也可用。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行车/滑板车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "缩放 **最高速度**。\n" +
                    "**0.30 = 30%** 游戏默认\n" +
                    "**1.00 = 游戏默认**\n" +
                    "注意：仍受道路限速与游戏状态影响。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "刚度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**摆动幅度** 倍率。\n" +
                    "**更高 = 更少倾斜**。\n" +
                    "**更低 = 更晃**。\n" +
                    "建议：高速稳定 1.25–1.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快稳定（更少振荡）。\n" +
                    "**1.00 = 游戏默认**\n" +
                    "建议：高速稳定 1.25–2.00。"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模组默认" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "应用模组的默认调校值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "游戏默认" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "把所有滑条设为 **100%** 并恢复游戏默认（vanilla）。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "路径限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "缩放 **路径(Path)** 限速（路径不是道路）。\n" +
                    "**1.00 = 游戏默认**\n" +
                    "影响：自行车道、步行+自行车道、步行道。\n\n" +
                    "卸载提示：设为 1.00 或用重置按钮，保存城市后再卸载。\n" +
                    "忘了也没事：旧路径保留改过的速度，新路径用游戏默认。"
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自行车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自行车和电动滑板车。\n" +
                    "**活动** = 有当前车道（在移动）。\n" +
                    "**总停放** = 包含所有停放标记（如路边），不只是停车场。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "汽车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "仅个人汽车（不含自行车组）。\n" +
                    "**活动** = 有当前车道（在移动）。\n" +
                    "**停放** = 有 **ParkedCar**。\n" +
                    "注意：游戏信息面板不统计所有停放类型，所以数字更低。\n" +
                    "扫描仅在打开选项时运行，不影响城内 fps。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "边境隐藏车辆" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**边境隐藏** = 停在边界外的 Outside City (OC) 连接车辆。\n" +
                    "这些车游戏里看不见，但计入总停放数。\n" +
                    "有些城市 OC 车很多，还绑定城内 Owner。\n" +
                    "还需要研究：这是游戏暂存还是别的？\n\n" +
                    "想看细节：点 <记录隐藏车辆> 把样本 ID 写入日志。\n" +
                    "用 Scene Explorer 查看 Vehicle ID 并分享结果。cim 能用这些车吗？"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "记录隐藏车辆" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "一次性写入 **Logs/FastBikes.log**（头+尾样本）。\n" +
                    "用 Scene Explorer Jump To 列出的 Vehicle 实体 ID。"
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "状态未加载。" },
                { "FAST_STATS_NOT_AVAIL",       "没有城市... ¯\\_(ツ)_/¯ ...没数据" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "让城市跑几分钟再看。" },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 活动 | {1} 自行车 | {2} 滑板车 | {3} / {4} 停/总" },
                { "FAST_STATS_CARS_ROW2",  "{0} 活动 | {1} 停放 | {2} 总计 | {3} 拖车" },
                { "FAST_STATS_CARS_ROW3",  "{0} 边境隐藏OC | 更新 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "显示名称。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "当前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "打开作者的 Paradox Mods 页面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行车调试报告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性调试日志。\n" +
                    "先加载城市。\n" +
                    "输出：**Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
