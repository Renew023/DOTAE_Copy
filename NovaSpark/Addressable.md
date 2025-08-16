# ✅ AddressableManager 사용 가이드

## 🔄 Addressables 동작 흐름

Unity Addressables 시스템은 리소스를 다음과 같은 흐름으로 로딩합니다

### 📦 CCD (클라우드 저장소)

-  ↓ DownloadDependenciesAsync(label)
      
### 📁 로컬 캐시 (Application.persistentDataPath 내부)

-  ↓ LoadAssetAsync(key)
      
### 런타임 딕셔너리 캐시 (_audioCache, _prefabCache 등)

## 🔍 동작 요약

- DownloadDependenciesAsync(label)
해당 라벨에 포함된 에셋을 CCD → 로컬 캐시로 다운로드
(이미 로컬에 있을 경우 다운로드 생략)

- LoadAssetAsync(key)
지정된 키에 해당하는 리소스를 로컬 캐시에서 로드 (성공 시 메모리에 올라감)
단, 로컬에 없으면 자동으로 CCD에서 로드하여 캐시에 저장

- 딕셔너리 캐싱 (_audioCache 등)
자주 사용하는 리소스를 재사용하기 위해 런타임 딕셔너리에 추가로 캐싱
→ 중복 LoadAssetAsync 호출을 줄이고, 메모리 접근 비용 최소화

> 따라서, DownloadDependenciesAsync()로 미리 받아두고
LoadAssetAsync()는 실제 사용할 시점에 호출하는 구조

## 📌 개요

AddressableManager는 Unity Addressables 시스템을 기반으로 다음과 같은 기능을 제공합니다

- 비동기 리소스 로딩 (Prefab, Sprite, AudioClip, Json)
- 중복 로딩 방지를 위한 캐싱
- 프리팹 Object Pooling
- 리소스 일괄 다운로드 및 진행률 표시
- 메모리 해제 및 캐시 클리어 기능

## 주요 기능 요약
| 기능                                       | 설명                                |
| ---------------------------------------- | --------------------------------- |
| `LoadPrefab(string key)`                 | 프리팹 로딩 및 캐싱                       |
| `LoadIcon(string key)`                   | Sprite 아이콘 로딩                     |
| `LoadAudioClip(string key)`              | 사운드 로딩                            |
| `GetFromPool(string key, parent)`        | 풀링 기반 오브젝트 생성                     |
| `ReturnToPool(string key, obj)`          | 사용 완료 후 오브젝트 반환                   |
| `DownloadAllWithProgress(Action<float>)` | 지정 라벨 리소스 일괄 다운로드 + 진행률 표시        |
| `ReleaseAllCachedAssets()`               | 모든 캐시된 리소스 Addressables.Release() |

### 사용법 예시

#### 리소스 다운로드 진행률 표시
```cs
await AddressableManager.Instance.DownloadAllWithProgress(progress =>
{
    progressBar.fillAmount = progress;
    loadingText.text = $"리소스 다운로드 중... {(int)(progress * 100)}%";
});
```

#### 프리팹 로딩 및 풀링 사용

- 풀링된 프리팹 생성
```cs
GameObject obj = await AddressableManager.Instance.GetFromPool(프리팹키, 생성위치);
```

- 풀로 반환
```cs
AddressableManager.Instance.ReturnToPool(프리팹키, obj);
```

#### 아이콘 로딩
```cs
Sprite icon = await AddressableManager.Instance.LoadIcon(아이콘키);
myImage.sprite = icon;
```

#### 배경음, 효과음 재생
```cs
SoundManager.Instance.PlayerBGMAsync(배경사운드키);
SoundManager.Instance.PlayerSFXAsync(효과사운드키);
```

#### 메모리 해제

- 전체 캐시 해제
```cs
AddressableManager.Instance.ReleaseAllCacheAssets();
```
