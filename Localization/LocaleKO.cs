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
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsResetGrp), "리셋" },
                { m_Setting.GetOptionGroupLocaleID(Setting.ActionsPathSpeedGrp), "경로" },

                { m_Setting.GetOptionGroupLocaleID(Setting.AboutInfoGrp), "모드 정보" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutLinksGrp), "링크" },
                { m_Setting.GetOptionGroupLocaleID(Setting.AboutDebugGrp), "디버그" },

                // Master toggle
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableFastBikes)), "Fast Bikes 활성화" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableFastBikes)),
                    "모드를 ON/OFF 합니다.\n" +
                    "OFF이면 자전거/스쿠터가 게임 기본값으로 복원됩니다."
                },

                // Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SpeedScalar)), "자전거/스쿠터 속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SpeedScalar)),
                    "**최고 속도 배율**\n" +
                    "고속에서 가속/감속이 부드러운 공식을 사용합니다.\n" +
                    "**0.30 = 기본값의 30%**\n" +
                    "**1.00 = 게임 기본값**\n" +
                    "참고: 도로 제한속도/게임 조건은 여전히 적용될 수 있습니다."
                },

                // Stability
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.StiffnessScalar)), "강성" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.StiffnessScalar)),
                    "**흔들림(기울기) 폭** 배율.\n" +
                    "**높을수록 = 덜 기움** (더 단단해 보임).\n" +
                    "**낮을수록 = 더 흔들림.**\n" +
                    "참고: 스쿠터는 기본값이 달라 더 기울 수 있습니다.\n" +
                    "고속 안정 추천: 1.25–1.75.\n" +
                    "더 흔들리게: < 0.75."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DampingScalar)), "감쇠" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DampingScalar)),
                    "높을수록 = 더 빨리 안정됨(진동이 빨리 사라짐).\n" +
                    "**1.0 = 게임 기본값**\n" +
                    "고속 안정 추천: 1.25–2.0+\n" +
                    "더 흔들리게: < 0.75"
                },

                // Reset buttons
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToModDefaults)), "모드 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToModDefaults)),
                    "모드의 기본 튜닝 값으로 설정합니다."
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetToVanilla)), "게임 기본값" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetToVanilla)),
                    "모든 슬라이더를 **100%** 로 되돌리고 게임 기본값을 복원합니다."
                },

                // Path Speed
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PathSpeedScalar)), "경로 제한속도" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.PathSpeedScalar)),
                    "**경로(Path)** 제한속도를 배율로 조정합니다 (경로는 도로가 아닙니다).\n" +
                    "**1.00 = 게임 기본값**\n" +
                    "대상: 자전거 도로, 보행자+자전거 분리, 보행자 전용 경로.\n" +
                    "새 Beta 기능 — GitHub/포럼에 피드백 부탁합니다."
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
                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.DumpBicyclePrefabs)), "자전거 디버그 리포트" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.DumpBicyclePrefabs)),
                    "자전거/스쿠터 상세 프리팹 값을 1회 로그 리포트로 출력합니다.\n" +
                    "일반 플레이에는 필요 없습니다.\n\n" +
                    "게임 업데이트 후 확인/디버깅에 유용합니다.\n" +
                    "도시를 먼저 로드한 뒤 사용; 출력: **Logs/FastBikes.log**"
                },
            };
        }

        public void Unload( )
        {
        }
    }
}
