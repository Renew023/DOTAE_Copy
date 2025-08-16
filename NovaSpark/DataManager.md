# ✅ DataManager 사용법

데이터는 DataManager.Instance를 통해 접근해야 합니다.

### 🔍 아이템 정보 가져오기

```cs
var item = DataManager.Instance.GetItemById(1);

if (item != null)
{
    Debug.Log($"Name: {item.name}, Type: {item.type}, Value: {item.value}");
}
```
### 📋 모든 아이템 순회

```cs
foreach (var item in DataManager.Instance.AllItems)
{
    Debug.Log($"ID: {item.id}, Name: {item.name}");
}
```

## ⚠️ 주의사항

- Item은 직접 new 하지 마세요.

- 반드시 id 기반 조회 또는 AllItems 순회로 접근하세요.

- DataManager는 싱글톤이며, 자동으로 실행 씬에 존재해야 합니다.

- JSON 파일은 Resources/Data/파일명.json에 있어야 합니다.

## 📁 관련 클래스 및 파일

- DataManager.cs

- Item.cs

- ItemLoader.cs

- Resources/Data/Item.json



