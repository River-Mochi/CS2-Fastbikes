// File: Localization/LocaleZH_CN.cs
// Purpose: Simplified Chinese entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "稳定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "重置" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "步道" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "模组信息" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "链接" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "调试" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "启用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "将模组 **开启/关闭（开/关）**。\n" +
                    "关闭后，自行车和电动滑板车将恢复为游戏默认行为。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行车与电动滑板车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**缩放最大速度**\n" +
                    "在高速度下使用更平滑的加速与制动公式。\n" +
                    "**0.30 = 默认值的 30%**\n" +
                    "**1.00 = 游戏默认值**\n" +
                    "注意：道路限速和游戏条件仍可能生效。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "刚性" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "用于 **摆动幅度** 的倍率。\n" +
                    "**更高 = 更少倾斜**（更紧致的外观）。\n" +
                    "**更低 = 更摇晃。**\n" +
                    "注意：电动滑板车的默认值不同，可能仍会更倾斜。\n" +
                    "- 高速更稳定：1.25–1.75。\n" +
                    "- 更摇晃：< 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快稳定（振荡更快消失）。\n" +
                    "**1.0 = 游戏默认值**\n" +
                    "- 高速更稳定：1.25–2.0+\n" +
                    "- 更摇晃：< 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模组默认值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "应用模组推荐的默认调校数值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "游戏默认值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "将所有滑条恢复为 **100%**，并恢复游戏默认值（vanilla）。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "步道限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "按倍率缩放 **步道（Path）** 的限速（步道不是道路）。\n" +
                    "**1.00 = 游戏默认值**\n" +
                    "影响：自行车道、分离的步行+自行车道、步行道。\n" +
                    "卸载模组前，请将此项（以及所有数值）重置为 1.00，然后加载一次城市以恢复步道限速。\n" +
                    "之后即可安全卸载。若跳过此步骤，\n" +
                    "现有步道会保留当前限速，而所有新建步道将使用 vanilla 默认限速。"
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "个人车辆状态" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "自行车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "自行车与电动滑板车。\n" +
                    "**行驶中** = 具有当前车道的实体（正在移动）。\n" +
                    "**停放** = 具有 **ParkedCar** 的实体。\n" +
                    "自行车组由 prefab 上的 **BicycleData** 进行判定。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "汽车组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "仅个人汽车（不包含上面的自行车组）。\n" +
                    "**行驶中** = 具有当前车道的实体（正在移动）。\n" +
                    "**停放** = 具有 **ParkedCar** 的实体（包含路边/路沿）。\n" +
                    "注意：可能与游戏信息面板的停放数量不一致，因为这里统计所有停放，而不仅是停车场内的。\n" +
                    "扫描仅在打开选项菜单时运行（不会在城市中每帧运行，以获得更好性能）。"
                },

                { "FAST_STATUS_NOT_LOADED", "状态未加载。" },
                { "FAST_STATS_NOT_AVAIL", "没有城市... ¯\\_(ツ)_/¯ ...没有统计" },
                { "FAST_STATS_CARS_NOT_AVAIL", "让城市运行几分钟以获取数据。" },

                { "FAST_STATS_BIKES_ROW1", "{0} 行驶中 | {1} 自行车 | {2} 电动滑板车 | {3} / {4} 停放/总计" },
                { "FAST_STATS_CARS_ROW2",  "{0} 行驶中 | {1} 停放 | {2} 总计 | 更新 {3}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "显示名称。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "当前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "打开作者的 Paradox Mods 页面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行车调试报告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性输出与自行车相关的数值日志。\n" +
                    "正常游玩不需要。\n\n" +
                    "用于在游戏更新后验证 prefab 或进行调试。\n" +
                    "点击前请先加载城市；输出到 **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
