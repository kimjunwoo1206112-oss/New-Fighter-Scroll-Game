# 테스트 실행 가이드

## Unity에서 테스트 실행 방법

### 1. 테스트 윈도우 열기
- Unity 메뉴 → **Window** → **General** → **Test Runner**
- 단축키: `Ctrl + Shift + T` (Windows)

### 2. 테스트 실행
- **Run All**: 모든 테스트 실행
- **Run Selected**: 선택한 테스트만 실행

### 3. Phase별 테스트 실행

| Phase | 테스트 파일 | 테스트 내용 |
|-------|------------|------------|
| Phase 1 | PlayerControllerTests.cs | 플레이어 이동, 공격, 피격, 필사기 |
| Phase 2 | EnemyAndBossTests.cs | 적 이동, 보스 패턴 |
| Phase 3 | ItemTests.cs | 아이템 드롭, 업그레이드 |
| Phase 4 | FighterSelectionTests.cs | 전투기 선택, 성능 |
| Phase 5 | GameFlowTests.cs | 게임 플로우, UI |

### 4. 테스트 결과 확인
- ✅ Pass: 테스트 통과
- ❌ Fail: 테스트 실패
- ⚠️ Ignored: 건너뛴 테스트

### 5. 각 Phase 완료 후
1. 해당 Phase 테스트 파일 실행
2. 모든 테스트 Pass 시 다음 Phase 진행
3. 실패 시 구현 수정 후 재테스트