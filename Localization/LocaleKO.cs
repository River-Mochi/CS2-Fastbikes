// File: Localization/LocaleKO.cs
// Purpose: Korean entries for FastBikes.

namespace FastBikes
{
    using Colossal;                    // IDictionarySource, IDictionaryEntryError
    using System.Collections.Generic;  // IEnumerable, Dictionary, KeyValuePair

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
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),   "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),      "속도" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp),  "안정성" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),      "리셋" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp),     "개인 차량 상태" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp),  "경로" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),  "모드 정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "디버그" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes 사용" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "모드를 **ON / OFF** 합니다.\n" +
                    "OFF이면 자전거와 스쿠터 동작이 게임 기본값으로 돌아갑니다.\n\n" +
                    "아래 상태 정보는 Enable Fast Bikes가 OFF여도 표시됩니다."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "자전거 & 스쿠터 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**최대 속도 배율**.\n" +
                    "**0.30 = 게임 기본의 30%**\n" +
                    "**1.00 = 게임 기본**\n" +
                    "참고: 도로 제한과 게임 조건은 그대로 적용됩니다."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "강성" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**흔들림 폭** 배율.\n" +
                    "**높을수록 = 덜 기울어짐**.\n" +
                    "**낮을수록 = 더 흔들림**.\n" +
                    "추천: 고속 안정은 1.25–1.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "감쇠" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "높을수록 빨리 안정(진동 감소).\n" +
                    "**1.00 = 게임 기본**\n" +
                    "추천: 고속 안정은 1.25–2.00."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "모드 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "모드 기본 튜닝값을 적용합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "게임 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "모든 슬라이더를 **100%**로 하고 게임 기본값(바닐라)로 복원합니다."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "경로 속도 제한" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**경로(Path)** 속도 제한을 배율로 조정합니다(경로는 도로가 아님).\n" +
                    "**1.00 = 게임 기본**\n" +
                    "대상: 자전거도로, 보행+자전거, 보행자 경로.\n\n" +
                    "삭제 시: 1.00으로 돌리거나 리셋 버튼 → 도시 저장 → 삭제.\n" +
                    "깜빡하면: 기존 경로는 수정값 유지, 새 경로는 바닐라 기본값."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "자전거 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "자전거와 전동 스쿠터.\n" +
                    "**활성** = 현재 레인 있음(이동 중).\n" +
                    "**총 주차** = 모든 주차 플래그(예: 갓길)를 포함. 주차장만이 아님."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "자동차 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "개인 자동차만(자전거 그룹 제외).\n" +
                    "**활성** = 현재 레인 있음(이동 중).\n" +
                    "**주차** = **ParkedCar** 보유.\n" +
                    "스캔은 Options를 열었을 때만 실행되고, 도시 fps에는 영향 없습니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "숨겨진 주차 차량" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**OC 경계 총합** = 도시 밖(OC) 연결에서 ParkedCar인 자동차 그룹 차량.\n" +
                    "도시에 따라 도시 밖 연결에 주차로 막힌 차가 아주 많을 수 있습니다.\n" +
                    "<숨겨진 차량 로그>로 숨겨진 차량 샘플 내역을 확인하세요."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "숨겨진 차량 로그" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "**Logs/FastBikes.log**에 1회 보고서를 씁니다.\n" +
                    "총합 + 구분 A/B/C 내역과 샘플 ID 번호가 포함됩니다.\n" +
                    "Scene Explorer mod로 목록의 차량 엔티티 ID로 이동해서 조사할 수 있습니다."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "상태를 불러오지 못했습니다." },
                { "FAST_STATS_NOT_AVAIL",       "도시 없음... ¯\\_(ツ)_/¯ ...통계 없음" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "도시를 몇 분 돌려서 데이터를 만든 뒤 다시 확인하세요." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 활성 | {1} 자전거 | {2} 스쿠터 | {3} / {4} 주차/총합" },
                { "FAST_STATS_CARS_ROW2",  "{0} 활성 | {1} 주차 | {2} 총합 | {3} 트레일러" },

                // Row3 shows TOTAL OC hidden (not bucket A)
                { "FAST_STATS_CARS_ROW3",  "OC 경계 총합 {0} | 업데이트 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "표시 이름." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "현재 버전." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "작성자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "자전거 디버그 리포트" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "디버그 또는 패치 날 확인용 상세 로그(1회).\n" +
                    "일반 플레이에는 필요 없습니다.\n" +
                    "먼저 도시를 로드하세요.\n" +
                    "출력 위치: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
