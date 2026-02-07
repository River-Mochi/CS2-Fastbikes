// File: Localization/LocaleKO.cs
// Purpose: Korean entries for FastBikes.

namespace FastBikes
{
    using Colossal; // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic; // IEnumerable, Dictionary, KeyValuePair

    public sealed class LocaleKO : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleKO(Setting setting)
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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab), "작업" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab), "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp), "속도" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "안정성" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "초기화" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "모드 정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "디버그" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes 사용" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "모드를 ON/OFF 합니다.\n" +
                    "OFF이면 자전거와 전동 킥보드 동작이 원래대로 복원됩니다."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "자전거 & 킥보드 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**최고 속도 배율**\n" +
                    "선택한 속도에 맞춰 가속과 제동도 함께 조정됩니다.\n" +
                    "**0.30 = 게임 기본의 30%**\n" +
                    "**1.00 = 게임 기본**\n" +
                    "참고: 도로/경로 속도 제한과 게임 상황이 여전히 적용될 수 있습니다."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "강성(뻣뻣함)" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**흔들림(기울기) 크기** 배율입니다.\n" +
                    "높을수록 = 덜 기울어짐(더 타이트한 느낌).\n" +
                    "낮을수록 = 더 흔들림.\n" +
                    "참고: 킥보드는 기본값이 달라 더 많이 기울 수 있습니다.\n" +
                    "고속 안정 추천: 1.25–1.75.\n" +
                    "더 흔들리게: 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "감쇠" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "높을수록 = 더 빨리 안정됨(진동이 더 빨리 사라짐).\n" +
                    "**1.0 = 게임 기본값**\n" +
                    "고속 안정 추천: 1.25–2.0+\n" +
                    "더 흔들리게: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "모드 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "모드의 기본 튜닝 값을 적용합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "게임 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "모든 슬라이더를 **100%** 로 되돌리고 게임 기본값으로 복원합니다."
                },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)), "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)), "표시 이름." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)), "현재 버전." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "작성자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "자전거 프리팹 덤프" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "자전거/킥보드 상세 값을 로그로 출력합니다.\n" +
                    "일반 플레이에는 필요하지 않습니다.\n\n" +
                    "게임 업데이트 이후 또는 문제 디버깅에 유용합니다.\n" +
                    "먼저 도시를 로드하세요. 데이터는 **FastBikes.log** 로 출력됩니다."
                },
            };
        }

        public void Unload()
        {
        }
    }
}
