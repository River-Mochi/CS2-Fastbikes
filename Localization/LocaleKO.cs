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
                { m_Setting.GetOptionTabLocaleID(Setting.ActionsTab),       "작업" },
                { m_Setting.GetOptionTabLocaleID(Setting.AboutTab),        "정보" },

                // Groups
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsSpeedGrp),     "속도" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStabilityGrp), "안정성" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp),     "리셋" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "경로" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp),   "모드 정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp),  "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp),  "디버그" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes 활성화" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "모드를 **ON/OFF** 합니다.\n" +
                    "OFF일 때는 자전거와 스쿠터 동작이 게임 기본값으로 복원됩니다."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "자전거 & 스쿠터 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**최대 속도를 배율로 조정**\n" +
                    "고속에서는 가속/제동이 부드럽게 보이도록 계산식을 사용합니다.\n" +
                    "**0.30 = 게임 기본값의 30%**\n" +
                    "**1.00 = 게임 기본값**\n" +
                    "참고: 도로 제한속도와 게임 조건이 여전히 적용될 수 있습니다."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "강성" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**흔들림(스웨이) 진폭** 배율입니다.\n" +
                    "**높을수록 = 기울어짐 감소** (더 단단한 느낌).\n" +
                    "**낮을수록 = 더 흔들림.**\n" +
                    "참고: 스쿠터는 기본값이 달라 더 많이 기울 수 있습니다.\n" +
                    "- 고속 안정 추천: 1.25–1.75.\n" +
                    "- 더 흔들리게: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "감쇠" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "높을수록 = 더 빨리 안정됨(진동이 빨리 사라짐).\n" +
                    "**1.0 = 게임 기본값**\n" +
                    "- 고속 안정 추천: 1.25–2.0+\n" +
                    "- 더 흔들리게: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "모드 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "모드의 기본 튜닝 값을 적용합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "게임 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "모든 슬라이더를 **100%** 로 되돌리고 게임 기본값(바닐라)을 복원합니다."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "경로 속도 제한" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**경로(Path)** 속도 제한을 배율로 조정합니다(경로는 도로가 아닙니다).\n" +
                    "**1.00 = 게임 기본값**\n" +
                    "대상: 자전거 도로, 보행+자전거 분리 경로, 보행자 경로.\n" +
                    "모드를 제거하려면 이 값을 1.00(및 모든 값)으로 되돌린 뒤 도시를 로드하여 경로 속도 제한을 복원하세요.\n" +
                    "그 후 모드를 안전하게 제거할 수 있습니다. 이 단계를 건너뛰면,\n" +
                    "기존 경로는 현재 속도 제한을 유지하고 새 경로는 바닐라 기본 속도 제한을 사용합니다."
                },

                // Status fields
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsStatusGrp), "개인 차량 상태" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary1)), "자전거 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary1)),
                    "자전거 및 전동 스쿠터.\n" +
                    "**활동 중** = 현재 레인을 가진 엔티티(이동 중).\n" +
                    "**주차 중** = **ParkedCar** 를 가진 엔티티.\n" +
                    "자전거 그룹은 prefab의 **BicycleData** 로 구분됩니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StatusSummary2)), "자동차 그룹" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StatusSummary2)),
                    "개인 자동차만(위 자전거 그룹 제외).\n" +
                    "**활동 중** = 현재 레인을 가진 엔티티(이동 중).\n" +
                    "**주차 중** = **ParkedCar** 를 가진 엔티티(갓길/연석 포함).\n" +
                    "참고: 게임 패널의 주차 수와 일치하지 않을 수 있습니다(여기는 주차장만이 아니라 모든 주차를 카운트).\n" +
                    "스캔은 옵션 메뉴가 열려 있을 때만 실행됩니다(도시에서 매 프레임 실행하지 않아 성능에 유리)."
                },

                { "FAST_STATUS_NOT_LOADED", "상태가 로드되지 않았습니다." },
                { "FAST_STATS_NOT_AVAIL", "도시 없음... ¯\\_(ツ)_/¯ ...통계 없음" },
                { "FAST_STATS_CARS_NOT_AVAIL", "데이터를 위해 도시를 몇 분간 실행하세요." },

                { "FAST_STATS_BIKES_ROW1", "{0} 활동 중 | {1} 자전거 | {2} 스쿠터 | {3} / {4} 주차/합계" },
                { "FAST_STATS_CARS_ROW2",  "{0} 활동 중 | {1} 주차 중 | {2} 합계 | {3} 업데이트" },

                // About: Info
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutName)),      "모드" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutName)),       "표시 이름." },
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.AboutVersion)),   "버전" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.AboutVersion)),    "현재 버전." },

                // Links
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenParadoxMods)), "Paradox Mods" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenParadoxMods)), "작성자의 Paradox Mods 페이지를 엽니다." },

                // Debug
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "자전거 디버그 리포트" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "자전거 관련 값을 1회 로그로 출력합니다.\n" +
                    "일반 플레이에는 필요하지 않습니다.\n\n" +
                    "게임 업데이트 후 prefab 확인이나 디버깅에 유용합니다.\n" +
                    "클릭 전에 도시를 로드하세요. 출력: **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
