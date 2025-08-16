# ✅ 개요

이번 프로젝트에서 저는 Unity 기반 2D 게임의 핵심 시스템 개발을 맡아 다음과 같은 기능을 구현했습니다:

- 슬롯머신 & 강화 시스템: JSON 기반 데이터로 릴 회전, 보상 지급, 장비 강화 로직 구현

- 데이터 자동화 도구: Excel → JSON 및 C# 클래스 자동 생성

- Addressables + CCD 전환: 리소스 보안성 강화 및 실시간 업데이트 대응

- 낮/밤 시스템: 시간 흐름에 따라 조명 및 게임 상태 전환

- NPC 호감도 & 마을 관계 시스템: 시간 기반 호감도 회복 및 적대 관계 연동

- 사운드 시스템 개선: Addressables 기반 BGM/SFX 비동기 로딩 및 캐싱 구조 적용

- 날씨 시스템 구현: 날씨 상태 저장 및 랜덤 날씨 변화, 파티클 시각 효과 적용

---

## 🗓️ 1주차 개발 내용

### SlotMachineController.cs

- 슬롯머신 회전 컨트롤러
- `Start()`에서 릴 초기화, 골드 UI 초기화
- Spin 버튼 클릭 시:
  - 골드 차감
  - 릴 회전 시작
  - 일정 시간 후 결과 체크
- 당첨 시 보상 골드 지급

### SlotReel.cs

- 슬롯 릴 애니메이션 처리
- `StartSpin(float duration)` 호출 시 일정 시간 동안 무작위로 아이템 스핀
- 스핀 완료 후 결과 보존

### 3. SlotData.json

- 슬롯 아이콘, 배율(multiplier) 등의 정보 포함
- `DataManager`가 JSON에서 불러와 `SlotReel`에 제공

---

### 💾 DataManager.cs

- 싱글톤 기반 전체 데이터 로더
- `Resources/Data` 폴더의 JSON 파일들을 읽어와 Dictionary 및 List로 보관
- 자동으로 파싱되는 주요 항목:
  - `Equipment`, `Consumable`, `CharacterData`, `SlotData`, `UnitStat`, `Part`, `TecData` 등
- JSON → 객체 변환은 `JsonConvert.DeserializeObject<List<T>>()` 사용

---

### 💡 강화 시스템

#### EnhanceItemSlot.cs

- 장비 아이템을 드래그 앤 드롭하여 강화창에 등록
- 드롭된 장비가 `EquipmentItem` 타입이 아니면 무시

#### EnhanceUI.cs

- 현재 선택된 장비 정보를 UI에 표시
- 강화 버튼 클릭 시 성공/실패 처리
- 성공 시 `enhanceLevel++`, 실패 시 (TODO: 패널티 추가 예정)

---

### 🎵 SoundManager.cs

- 배경음악(BGM)과 효과음(SFX) 담당
- `Resources/Sounds/BGM`, `Sounds/SFX` 폴더의 클립을 자동 로드
- BGM 반복 재생, SFX 단발 재생
- `PlayerPrefs`를 이용한 볼륨 저장 및 로드

---

### 📦 데이터 구조 및 자동화

#### ExcelToJsonConverter.cs

- `Assets/Excel` 내 `.xlsx` 파일을 자동 분석
- 각 시트에서 클래스 + JSON + Enum 자동 생성
- 열 타입:
  - `int`, `float`, `string`, `bool`
  - `Enum<EnumName>`, `List<Enum<EnumName>>`
  - `List<int>`, `List<float>`, `List<string>`
- 예외 처리:
  - 잘못된 타입 정의 시 오류 출력
  - EnumDefinition.xlsx로 enum 일괄 관리

---

## 🗓️ 2주차 개발 내용

### 🔄 Addressables + CCD 구조 전환

- 기존 Resources.Load<TextAsset>() 방식 대신 Addressables.LoadAssetAsync<TextAsset>() 방식으로 전환
- 리소스 보안 강화 및 런타임 유연성 향상
- CCD(Cloud Content Delivery)와 연동하여 실시간 데이터 핫픽스, 밸런스 조절 가능 ( 현재는 로컬로 테스트 상태)

#### 전환 목적 요약
| 항목    | 변경 전                        | 변경 후                                     |
| ----- | --------------------------- | ---------------------------------------- |
| 로딩 방식 | Resources.Load<TextAsset>() | Addressables.LoadAssetAsync<TextAsset>() |
| 저장 위치 | Resources 폴더                | Addressables 그룹 & CCD                    |
| 장점    | 빠른 접근                       | 보안 강화, 앱 크기 감소, 실시간 수정 가능          |

---

### 🌗 낮/밤 시스템

#### TimeManager.cs

- DayState Enum(Day, Night)을 기준으로 게임 내 시간 흐름을 관리
- ChangeState() 호출 시 조명의 색상과 밝기를 부드럽게 전환
- 낮/밤 전환에 따라 게임 내 여러 시스템이 반응하도록 이벤트 전달
- 하루가 지날 때 OnDayPassed()호출 -> 호감도 회복 등광 연결
- 야간에는 몬스터 스폰, 상점 폐쇄 등 추가 예정

---

### 🤝 호감도 시스템

#### AffectionManager.cs

- NPC와 플레이어 간의 개별 호감도 수치(-100 ~ 100)를 관리
- affection < 0 일 경우 해당 NPC는 플레이어를 적대적으로 인식
- 하루가 지나면 OnDayPassded()를 통해 호감도 회복됨
- GetAffection(npcId)로 NPC별 호감도 조회 가능

---

### 🏘  마을 간 관계 시스템

#### VillageRelationManager.cs

- 각 캐릭터는 VillageID를 가지며, 마을 간 관계를 별도로 관리
- 플레이어가 특정 마을과 적대 관계일 경우 해당 마을의 NPC 전체가 공격적으로 반응
- 관계 수치는 -100 ~ 100 범위
- IsHostile(NPC self, CharacterObject target) 함수를 통해 개인 호감도 및 마을 관계에 따른 적대 여부를 판단

---

## 🗓️ 3주차 개발 내용

### 💡 플레이어 조명 시스템 리팩토링

#### Player.cs

- 낮/밤 상태(DayState) 및 실내/실외 여부에 따라 플레이어 조명(playerLight) 상태 제어
- 실외인 경우: 낮에는 꺼짐, 밤에는 부드럽게 켜짐 (AnimationCurve 사용)
- 실내인 경우: 시간과 무관하게 조명 항상 꺼짐
- TimeManager의 OnDayStateChanged 이벤트를 구독하여 상태 변화에 실시간 반응

#### TransitionPlayerLight()

- Coroutine 기반으로 조명의 Intensity를 부드럽게 변화
- transitionDuration과 intensityCurve를 통해 자연스러운 조명 전환 구현

---

### 🎵 SoundManager Addressables 전환

#### SoundManager.cs

- 기존 Resources.Load 기반 사운드 재생 구조 제거
- AddressableManager를 통해 BGM / SFX 비동기 로딩
- BGM: 루프 재생, SFX: OneShot
- PlayerPrefs 기반 볼륨 저장 및 불러오기 기능 유지
- 사용 예: SoundManager.Instance.PlaySFXAsync("Sound/SFX/slot_drumroll")

#### AddressableManager.cs - 사운드 로딩

- LoadAudioClip(string key)을 통해 오디오 Addressables 비동기 로딩
- 내부 Dictionary 캐시 구조로 중복 로딩 방지
- 실패 시 예외 처리 및 로그 출력

---

### 📦 AddressableManager 확장 기능

#### 제네릭 기반 로딩 함수

- LoadWithCache<T>() 제네릭 함수 도입
- Sprite, Prefab, AudioClip, JSON 등 모든 리소스에 동일한 방식 적용
- 명시적 래퍼: LoadIcon, LoadPrefab, LoadAudioClip 등

#### 오브젝트 풀링 통합

- GetFromPool(string key)로 Addressables 프리팹 풀링 구현
- 미사용 오브젝트는 ReturnToPool(string key, GameObject obj)로 반환
- 생성/파괴 반복 최소화 → 메모리 및 성능 최적화

#### 리소스 일괄 다운로드 시스템

- DownloadAllWithProgress(Action<float>) 메서드를 통해 Label 기반 리소스 사전 다운로드 구현
- 리소스 그룹(Label): Icons, Prefabs, Audios, Jsons
- 진행률은 Action<float>으로 전달되어 UI 반영 가능

---

## 🗓️ 4주차 개발 내용

### 💾 저장 시스템

#### SaveManager.cs

#### 저장 메서드 통합

- 자동 저장과 수동 저장을 하나의 메서드로 통합하여 코드 중복을 제거하고 유지보수를 용이하게 함

- 저장 경로를 인자로 받아 null일 경우 자동 저장, 문자열이 있을 경우 사용자 지정 이름으로 저장 파일 생성

#### 사용자 지정 저장 경로 생성

- 입력된 저장 이름을 기반으로 파일명을 생성하며, 공백은 언더스코어로 치환

- 향후 특수문자 제거 및 정규화 기능 추가 가능

#### 최근 저장 파일 정렬

- 저장 파일의 생성일을 기준으로 내림차순 정렬

- 가장 최근의 저장 파일부터 UI에 노출되도록 구성하여 UX 개선

#### 자동 저장 파일 정리

- 일정 개수 이상의 저장 파일이 생성될 경우, 가장 오래된 파일부터 자동으로 삭제

- 저장 공간 낭비를 방지하고 관리 효율성을 높임

- 최대 유지 개수는 상수로 설정되어 있으며 추후 옵션화 가능

---

#### 임시 저장 슬롯 & 수동 저장 구현

- 최근 저장 파일 목록을 정렬하여 UI 슬롯으로 자동 생성

- 각 슬롯은 해당 저장 파일명을 표시하며, 클릭 시 해당 저장 데이터를 불러옴

- TMP_InputField로 사용자가 저장 이름을 입력한 후 저장 버튼 클릭 시 수동 저장 가능

---

## 🗓️ 5주차 개발 내용

### 💡 GameManager 기반 매니저 구조 리팩토링

#### GameManager.cs

- 인게임에서 사용하는 매니저(TimeManager, WeatherManager, AffectionManager 등)의 직접 참조 방식으로 구조 개선

- 모든 인게임 매니저를 GameManager 하위에 배치하고, Init 순서를 명시적으로 제어

- 기존 Singleton 구조에서 발생하던 초기화 순서 문제, 의존성 불명확 문제 해결

- SaveManager, DataManager 등 전역 매니저는 기존처럼 Singleton 유지


### 🌨️ 날씨 시스템 구현 및 저장 연동

#### WeatherManager.cs

- ISaveable 인터페이스 구현을 통해 현재 날씨(WeatherType)를 저장/불러오기 지원

- 하루가 지날 때마다 무작위 날씨(Clear, Rain, Snow)를 선택하여 반영

- 날씨 변경 시 비/눈 파티클 오브젝트를 활성화/비활성화하여 시각적으로 표현

- LateUpdate()에서 카메라 위치를 따라 파티클 위치/회전을 갱신

### 🤝 호감도 시스템 저장 연동 개선

#### AffectionManager.cs

- 기존 호감도 로직에 저장/불러오기 기능 추가 (ISaveable 구현)

- 하루가 지나면 음수 호감도 자동 회복 기능 유지

- 외부 시스템이 OnAffectionChanged 이벤트를 통해 호감도 변화에 실시간 대응 가능

- 적대 상태는 호감도 수치 기반(Level: Hostile, Neutral, Friendly)으로 구분

### 🏘 마을 간 관계 시스템 저장 연동 개선

#### VillageRelationManager.cs

- 마을 간 관계 정보를 GameSaveData에 포함하여 저장/불러오기 가능

- 관계는 Dictionary<(int from, int to), VillageRelationData> 형태로 저장되며, 로딩 시 역방향 자동 복원

- 관계 수치에 따라 적대(Level: Hostile), 중립, 우호 관계로 판단

## 🗓️ 6주차 개발 내용

### 🕒 멀티플레이 시간 & 날씨 동기화 시스템

#### TimeManager.cs

- 싱글/멀티 모드 자동 분리: PhotonNetwork.IsConnected 여부로 판단
- MasterClient ㄱ지ㅜㄴ으로 PhotonNetwork.Time을 활용하여 시간 동기화
- 클라이언트 RPC_SyncTime(float syncedTime, double syncedPhotonTime) 호출을 통해 기준 시간 적용
- 하루 경과 시 이벤트(OnDayPassed) 발생  → NPC 호감도 회복 등 연동 처리

#### WeatherManager.cs
- MasterClient가 하루 경과 시 무작위 날씨 결정 후 RPC로 전체 전송
- 클라이언트는 RPC_ApplyWeather(int weatherType) 호출을 통해 날씨 적용
- 싱글모드에서는 로컬에서 랜덤 날씨 적용
- 날씨 변경 시 파티클 및 조명 효과 반영

#### GameManager.cs
- 게임 시작 시 싱글/멀티 모드 판단
- 싱글이면 직접 저장 데이터 로드
- 멀티이면 MasterClient만 저장 파일 로드 후 시간/날씨 동기화 전송
- 클라이언트는 수신만 수행

### 🔐 SaveManager AES 암호화 적용

#### AESUtil.cs
- AES 256비트 + CBC 모드 + PKCS7 패딩 방식 사용
- Encrypt(string plainText) / Decrypt(string encryptedText) 방식으로 암호화/복호화
- 내부에ㅓㅅ Aes.Create() 및 CryptoStream을 사용해 처리
- Base64로 암호문 인코딩 저장

#### AesKeyAsset (ScriptableObject)
- AES Key/IV 문자열을 TextArea 필드로 저장
- Addressables 그룹에 등록하여 런타임에 동적으로 로드 가능
- 민감 정보의 하드코딩을 방지하고 배포 후 키 교체 용이

#### AddressableManager.cs
- LoadAesKeyAsset(string key) 메서드로 AesKeyAsset을 Addressables에서 로드
- 로드 성공 시 AESUtil에 Key/IV 세팅 → 저장/불러오기 시점에서 사용 가능

#### SaveManager.cs
- 기존 JSON 저장/로드 구조는 유지
- Save 시 AESUtil로 암호화 후 파일 저장
- Load 시 복호화 후 JsonConvert.DeserializeObject로 파싱싱
