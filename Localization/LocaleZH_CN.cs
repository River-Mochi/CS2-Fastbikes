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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "操作" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "关于" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "速度" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "稳定性" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "重置" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "路径" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "模组信息" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "链接" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "调试" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "启用 Fast Bikes" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "开启/关闭模组。\n" +
                    "关闭时，自行车和滑板车行为恢复为游戏默认值。"
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "自行车/滑板车速度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**缩放最高速度**\n" +
                    "在高速时使用更平滑的加速/刹车公式。\n" +
                    "**0.30 = 默认值的 30%**\n" +
                    "**1.00 = 游戏默认值**\n" +
                    "注意：道路限速与游戏条件仍可能生效。"
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "刚度" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**摆动幅度** 的倍率。\n" +
                    "**更高 = 更少倾斜**（更“紧”）。\n" +
                    "**更低 = 更摇晃。**\n" +
                    "注意：滑板车默认值不同，可能仍会更倾斜。\n" +
                    "高速更稳：1.25–1.75。\n" +
                    "更摇晃：< 0.75。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "阻尼" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "更高 = 更快稳定（振动更快消失）。\n" +
                    "**1.0 = 游戏默认值**\n" +
                    "高速更稳：1.25–2.0+\n" +
                    "更摇晃：< 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "模组默认值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "应用模组的默认调校数值。"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "游戏默认值" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "将所有滑条设回 **100%** 并恢复游戏默认值。"
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "路径限速" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "缩放 **路径(Path)** 的限速（路径不是道路）。\n" +
                    "**1.00 = 游戏默认值**\n" +
                    "影响：自行车道、行人+自行车分隔、以及纯人行路径。\n" +
                    "新 Beta 功能 — 欢迎在 GitHub 或论坛反馈。"
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "模组" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "显示名称。" },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "版本" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "当前版本。" },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "打开作者的 Paradox Mods 页面。" },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "自行车调试报告" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "一次性输出到日志：自行车/滑板车的详细Prefab数值。\n" +
                    "正常游戏不需要。\n\n" +
                    "适合在游戏更新后核对Prefab或进行调试。\n" +
                    "请先加载城市再点击；输出到 **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
