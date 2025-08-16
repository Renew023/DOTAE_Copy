##**1주차**
---
## 스크립트 구조
| 스크립트 이름            | 역할 요약                                              |
|------------------------|--------------------------------------------------------|
| Spawner                | 랜덤 위치에 오브젝트 생성, 충돌 방지                        |
| FlockingAttack        | 스폰 지점 → 군집 → 공격 → 복귀 상태 전환                  |
| Wander2                | 목표 반경 내 jitter + seek으로 무작위 이동                  |
| VelocityMatch          | 시야 내 이웃 유닛 속도 평균 내어 가속도 적용               |
| Separation             | 이웃 유닛 간 최소 간격 유지하는 분리 가속도 계산            |
| Cohesion               | 이웃 유닛의 무게중심으로 부드럽게 이동(Arrive)              |
| SteeringBasics         | Seek, Arrive, LookWhereYoureGoing 등 스티어링 핵심 함수    |
| MovementAIRigidbody   | 2D/3D Rigidbody + Collider 셋업, 속도·위치 포맷팅         |
| FlockingUnit           | cohesion, separation, velocityMatch, wander 조합 최종 군집 동작 |

---
### 부드러운 회전 문제
- 공격 상태에서는 `Arrive` 대신 `Seek`과 `LookAtDirection`을 사용해 즉시 방향 전환하도록 구현.
---
##**2주차**
---
## **주요 기능**
- **방 생성(Create Room)**: 입력된 방 이름으로 최대 **5명**까지 입장 가능한 방 생성  
- **방 입장(Join Room)**: 입력된 방 이름으로 기존 방에 입장  
- **씬 동기화**: 마스터 클라이언트의 씬 로딩을 자동으로 동기화  
- **플레이어 인스턴스화**: 게임 씬 로드 시 **PlayerPrefab** 자동 생성  
- **카메라 추적**: **Cinemachine Virtual Camera** 로컬 플레이어 추적

---

## **사용 방법**
- **StartScene 에디터에서 Play 클릭**

- **방 이름 입력 후 Create Room 또는 Join Room 클릭**

- **GameScene 으로 자동 전환**

- **최대 5명까지 함께 플레이 가능**

---
## **프로젝트 구조**
Assets/</br>
├─ Scenes/</br>
│   ├─ StartScene.unity   # 로비 및 방 입장 UI 씬</br>
│   └─ GameScene.unity    # 플레이어 및 카메라 세팅 씬</br>
├─ Scripts/</br>
│   └─ Launcher.cs        # 로비 / 씬 전환 / 플레이어 인스턴스화 로직</br>
├─ Resources/</br>
│   └─ PlayerPrefab.prefab  # PhotonView 및 Transform 동기화 설정된 플레이어</br>
├─ .gitignore            # Git 무시 파일 설정</br>
└─ README.md             # 프로젝트 문서</br>

---

## **씬 설정**
**로비 씬 (StartScene)**
Canvas 아래에 UI 요소 추가:

InputField (이름: roomNameInput)

Button (이름: createButton, 텍스트: Create Room)

Button (이름: joinButton, 텍스트: Join Room)

TextMeshPro - Text (이름: feedbackText)

빈 GameObject 생성 → 이름: Launcher

Launcher.cs 컴포넌트 추가 및 UI 레퍼런스와 PlayerPrefab 연결

**게임 씬 (GameScene)**
CinemachineVirtualCamera 배치

Main Camera 기본 설정 유지

Launcher 스크립트 실행 시

PlayerPrefab 자동 인스턴스화

Virtual Camera Follow 설정

---
## **Launcher 스크립트 개요**
**OnStart: PhotonNetwork.ConnectUsingSettings() 호출**

**방 생성/입장: CreateRoom(), JoinRoom() 호출 및 피드백 표시**

**OnJoinedRoom: PhotonNetwork.LoadLevel("GameScene") 트리거**

**OnSceneLoaded: PlayerPrefab 인스턴스화 및 카메라 Follow 설정**

---
##**3주차**
---

## ✨ 주요 기능

### 1. **로비/방 시스템**
- **Photon PUN2** 기반 멀티플레이어 구조
- 닉네임 입력 → **싱글/멀티 모드 선택** → 로비 진입 → **방 생성/입장/퇴장**  
- 방 목록: **비밀번호/설명/최대인원/상태**(대기/게임중) 표시  
- **방 설정**: 호스트만 수정(설명, 비밀번호, 최대인원, 표시이름 등)

---

### 2. **대기실(WaitingRoom)**
- **플레이어 준비/미준비 상태** 실시간 동기화  
- 모든 플레이어가 준비 완료 시 **호스트만 게임 시작** 가능  
- **준비/시작 버튼**, **나가기 버튼**  
- **플레이어 리스트** 자동 갱신

---

### 3. **게임 시작 카운트다운 & 로딩 UI**
- **카운트다운 패널**(5~0초) + **로딩바(Progress Bar)** UI
- Unity UI의 **Slider** 활용, **카운트다운과 함께 자연스럽게 0→100%** 채워짐  
- 카운트다운 종료 후, **MasterClient**가 `PhotonNetwork.LoadLevel()`로 게임 씬 동기화

> **로딩바 UI 구현 TIP**
> - Slider의 Handle(동그라미/네모)은 **OFF/삭제**하여 깔끔하게 만듦  
> - Fill Area의 Image Type은 **Filled**, Source Image는 **테두리 없는 네모** 이미지 사용  
> - Fill Area의 RectTransform Offset을 **0**으로 맞추면 양 끝까지 꽉 참

---

### 4. **Photon 채팅(방별 채널)**
- **방마다 고유 채팅 채널 사용**  
- **방 입장 시 채널 구독(Subscribe)**  
- 입력창/채팅 토글/스크롤 자동/최대 메시지 제한  
- **자기 메시지와 타인 메시지 색상 구분**

> **TIP:**  
> - 반드시 `chatClient.CanChat` 상태에서만 subscribe  
> - 채팅 채널 구독은 **OnJoinedRoom 등 방 입장 후**에 호출!

---

### 5. **씬 전환/방 퇴장/로비 복귀**
- 각 상황에 맞는 **버튼/네트워크/씬 상태 관리**  
- `PhotonNetwork.LeaveRoom()`, `LeaveLobby()` 활용  
- 씬 전환 시 **패널(CanvasGroup) 알파/인터랙션/블록레이캐스트** 값 관리

---

## 🎨 UI/UX & 코드 팁

- **슬라이더(로딩바) 커스텀**: Fill Area만 보이게, Handle/Background 숨기기 or 스타일링
- **코루틴 활용**: 카운트다운, 페이크로딩, 실제로딩 등
- **Photon 자동 동기화**: `AutomaticallySyncScene` 사용, MasterClient에서 씬 전환 통제

---

## ⚠️ 개발 시 유의점

- **네트워크/채팅 매니저**는 중복 생성 안되게 **싱글톤** or **씬 배치 주의**
- **방/씬별 UI 오브젝트 Inspector에서 참조 연결 필수**
- Photon/ChatClient의 상태 체크 (`CanChat`, `InRoom` 등)

---

## 💡 참고/노하우

- **Photon Chat은 Photon Room과 별개!**  
  → 반드시 **Room.Name 등으로 채널명 만들어야 방별 채팅 가능**
- **Slider Fill이 100% 안 차면**  
  → Image 타입/RectTransform 설정, Source Image 체크!

**4주차**
---
## **목적**
기존에는 PhotonNetwork.LoadLevel()을 호출하는 StartGame() 메서드가 **NetworkManager**에 존재했으나,
**게임 시작 트리거(UI 버튼)**와 카운트다운 등의 실제 로직은 **WaitingRoomManager**가 담당하고 있었습니다.

이를 고려하여 StartGame()을 WaitingRoomManager로 이동시켜
기능의 응집도를 높이고, 불필요한 의존성을 제거하였습니다.

---
 **변경 전 구조**
WaitingRoomManager: 게임 시작 버튼, 카운트다운 처리

NetworkManager: 실제 StartGame() 실행 (LoadLevel)

즉, UI와 로직은 분리되어 있지만, 서로 강하게 결합되어 있어 가독성과 유지보수가 어려웠습니다.

---
**변경 후 구조**
WaitingRoomManager가 게임 시작을 직접 제어

StartGame() 제거: PhotonNetwork.LoadLevel() 직접 호출

NetworkManager는 씬 로딩 후 플레이어 생성 및 카메라 설정에만 집중

---
**효과**
**책임 분리(SRP**): WaitingRoomManager가 게임 시작을 온전히 책임

**의존성 감소**: NetworkManager.Instance.StartGame() 호출 제거

**가독성 향상**: 관련된 로직이 한 곳에 모여 있음

**유지보수성 향상**: 로직 추적이 쉬워짐

---
**참고 사항**
PhotonNetwork.LoadLevel()은 마스터 클라이언트만 호출 가능하므로,
호출 전 PhotonNetwork.IsMasterClient 여부를 꼭 확인해야 합니다.

씬 전환 후 플레이어 생성 및 카메라 설정은 기존대로 NetworkManager에서 유지합니다.

---
**5주차**
---
# 🧭 Unity 탭 전환 UI 구현 및 구조 개선

## 📌 개요

이 프로젝트에서는 Unity에서의 **탭 전환 기반 UI**(예: "방 설정" / "게임 속성") 구조를 구현하고,  
전환 오류 및 UI 상호작용 문제를 해결한 내용을 기반으로 전체 UI 구조를 안정적으로 재정비하였습니다.

---
## 🧩 주요 문제

처음에는 특정 탭(예: 게임 속성)으로 전환한 뒤,  
다른 탭(예: 방 설정)으로 다시 돌아가려고 해도 **버튼이 동작하지 않거나 UI가 갱신되지 않는 문제**가 발생했습니다.

---
## 🔍 문제 원인

- UI 오브젝트가 시각적으로는 꺼진 것처럼 보여도 실제로는 **Raycast를 막고 있는 상태로 남아있었고**,  
- 이로 인해 다른 버튼들이 **클릭되지 않는 상황**이 발생했습니다.
- 또한, 패널과 버튼의 계층 구조 정리가 부족해 **UI 충돌 가능성**이 있었습니다.

---
## ✅ 개선 내용

- 탭 전환 방식에 있어 **CanvasGroup 기반 제어 대신 GameObject 자체를 on/off** 하는 방식으로 전환
- **UI 계층 구조를 정리하여 버튼이 항상 위에 위치하도록 구성**
- 불필요한 Raycast 충돌을 방지하고, UI 충돌 없이 **정상적으로 모든 탭이 전환되도록 구조화**

---
## 💡 결과

- 탭 간 전환이 안정적으로 이루어지며,
- UI 충돌, 클릭 불가 문제 등이 해결되었고,
- 유지보수 및 확장성 측면에서도 보다 명확한 구조를 갖추게 되었습니다.
