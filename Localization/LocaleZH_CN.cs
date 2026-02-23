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
                    "将模组设为 **开 / 关**。\n" +
                    "关闭时，自行车与电动滑板车行为恢复为游戏默认。\n\n" +
                    "即使启用 Fast Bikes 为关闭，下方状态信息仍可查看。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行车与滑板车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**按比例调整最高速度**。\n" +
                    "**0.30 = 游戏默认的 30%**\n" +
                    "**1.00 = 游戏默认**\n" +
                    "注意：道路限速和游戏条件仍会生效。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "刚性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**摇摆幅度** 的比例。\n" +
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
                    "将所有滑块设为 **100%** 并恢复游戏默认（原版）。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "路径限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "按比例调整 **路径(Path)** 限速（路径不是道路）。\n" +
                    "**1.00 = 游戏默认**\n" +
                    "影响：自行车道、步行+自行车共用道、步行道。\n\n" +
                    "卸载提示：设为 1.00 或用重置按钮，保存城市后再卸载。\n" +
                    "若忘了：旧路径保留已修改限速，新建路径使用原版默认。"
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自行车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自行车与电动滑板车。\n" +
                    "**活跃** = 有当前车道（在移动）。\n" +
                    "**总停放** = 包含所有停放标记（如路边），不只停车场。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "汽车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "仅个人汽车（不含自行车组）。\n" +
                    "**活跃** = 有当前车道（在移动）。\n" +
                    "**停放** = 具有 **ParkedCar**。\n" +
                    "扫描只在打开 Options 时运行，不会影响城内 fps。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "隐藏停放车辆" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**OC 边界总数** = 汽车组中在 城市外(OC) 连接处停放（ParkedCar）的车辆。\n" +
                    "有些城市会出现大量车辆卡在 城市外 连接处停放。\n" +
                    "使用 <记录隐藏车辆> 查看隐藏车辆的样本分解。\n"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "记录隐藏车辆" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "写入一次性报告到 **Logs/FastBikes.log**。\n" +
                    "包含 总数 + A/B/C 分组明细 与示例 ID。\n" +
                    "用 Scene Explorer mod 跳转到列出的车辆实体 ID 进行排查。"
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "状态未加载。" },
                { "FAST_STATS_NOT_AVAIL",       "没有城市... ¯\\_(ツ)_/¯ ...没有统计" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "让城市运行几分钟以生成数据。" },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 活跃 | {1} 自行车 | {2} 滑板车 | {3} / {4} 停放/总计" },
                { "FAST_STATS_CARS_ROW2",  "{0} 活跃 | {1} 停放 | {2} 总计 | {3} 拖车" },

                // Row3 shows TOTAL OC hidden
                { "FAST_STATS_CARS_ROW3",  "OC 边界总数 {0} | 更新 {1}" },

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
                    "一次性详细日志，用于调试或补丁日验证。\n" +
                    "正常游玩不需要。\n" +
                    "请先加载城市。\n" +
                    "输出位置：**Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
