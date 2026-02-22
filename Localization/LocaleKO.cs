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
                    "모드를 **ON/OFF** 합니다.\n" +
                    "OFF이면 자전거와 전동 스쿠터가 게임 기본으로 돌아갑니다.\n\n" +
                    "아래 상태 정보는 Fast Bikes 사용이 OFF여도 표시됩니다."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "자전거/스쿠터 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**최대 속도 스케일**.\n" +
                    "**0.30 = 30%** 게임 기본\n" +
                    "**1.00 = 게임 기본**\n" +
                    "참고: 도로 제한 및 게임 조건은 그대로 적용됩니다."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "강성" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**흔들림 진폭** 스칼라.\n" +
                    "**높을수록 = 덜 기울임**.\n" +
                    "**낮을수록 = 더 wobble**.\n" +
                    "추천: 고속 안정 1.25–1.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "감쇠" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "높을수록 = 더 빨리 안정(진동 감소).\n" +
                    "**1.00 = 게임 기본**\n" +
                    "추천: 고속 안정 1.25–2.00."
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "모드 기본" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "모드 기본 튜닝값을 적용합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "게임 기본" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "모든 슬라이더를 **100%**로 설정하고 게임 기본(바닐라)로 복원합니다."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "경로 속도 제한" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**경로(Path)** 속도 제한을 스케일(경로는 도로가 아님).\n" +
                    "**1.00 = 게임 기본**\n" +
                    "대상: 자전거길, 보행+자전거, 보행 경로.\n\n" +
                    "삭제: 1.00으로 돌리거나 리셋 버튼 사용 → 도시 저장 → 삭제.\n" +
                    "깜빡하면: 기존 경로는 수정값 유지, 새 경로는 게임 기본."
                },

                // Status lines
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "자전거 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "자전거와 전동 스쿠터.\n" +
                    "**활성** = 현재 레인 있음(이동 중).\n" +
                    "**총 주차** = 주차장만이 아니라 모든 Parked 플래그(예: 노상)를 포함."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "자동차 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "개인 차량만(자전거 그룹 제외).\n" +
                    "**활성** = 현재 레인 있음(이동 중).\n" +
                    "**주차** = **ParkedCar** 있음.\n" +
                    "참고: 게임 패널은 모든 주차 타입을 다 세지 않아 숫자가 더 낮습니다.\n" +
                    "스캔은 옵션 창 열려 있을 때만 실행되어 성능 부담 적음."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary3)), "숨김 주차 차량" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary3)),
                    "**경계 밖 숨김** = Outside City (OC) 연결에 경계 밖으로 주차된 차량.\n" +
                    "게임에서 보이지 않지만 전체 주차 수에 포함됩니다.\n" +
                    "일부 도시는 도시 내 Owner와 연결된 OC 차량이 많이 보입니다.\n" +
                    "추가 조사 필요: 게임 스테이징? 다른 원인?\n\n" +
                    "궁금하면: <숨김 차량 로그> 버튼으로 샘플 ID를 로그에 기록.\n" +
                    "Scene Explorer로 ID 확인 후 공유. 연결된 차량을 cims가 사용할 수 있을까?"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.LogBorderHiddenCars)), "숨김 차량 로그" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.LogBorderHiddenCars)),
                    "**Logs/FastBikes.log**에 작은 1회 리포트(앞+뒤 샘플)를 씁니다.\n" +
                    "Scene Explorer에서 Vehicle 엔티티 ID로 Jump To."
                },

                // Status fallback keys
                { "FAST_STATUS_NOT_LOADED",     "상태 로드 안 됨." },
                { "FAST_STATS_NOT_AVAIL",       "도시 없음... ¯\\_(ツ)_/¯ ...통계 없음" },
                { "FAST_STATS_CARS_NOT_AVAIL",  "도시를 몇 분 돌려봐." },

                // Status rows
                { "FAST_STATS_BIKES_ROW1", "{0} 활성 | {1} 자전거 | {2} 스쿠터 | {3} / {4} 주차/합계" },
                { "FAST_STATS_CARS_ROW2",  "{0} 활성 | {1} 주차 | {2} 합계 | {3} 트레일러" },
                { "FAST_STATS_CARS_ROW3",  "{0} 경계 OC 숨김 | 업데이트 {1}" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),    "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),     "표시 이름." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)), "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),  "현재 버전." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)),  "작성자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "자전거 디버그" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "디버그용 1회 로그 리포트.\n" +
                    "먼저 도시를 로드.\n" +
                    "출력: **Logs/FastBikes.log**"
                }
            };
        }

        public void Unload( )
        {
        }
    }
}
