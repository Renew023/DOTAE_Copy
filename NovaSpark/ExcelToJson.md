# ✅ ExcelToJSonConverter

Unity 에디터에서 Excel 파일을 자동으로:

- JSON 데이터로 변환

- C# 데이터 클래스로 생성

- 전용 로더 클래스 생성

까지 한번에 처리해주는 툴입니다.

## 📁 파일 구조 예시
- Assets/Excel/ <- 변환 대상 .xlsx파일

- Assets/Resource/Dadta <- 생성된 JSON파일

- Assets/3. Scripts/BSE/Class <- 생성된 C# 데이터 클래스

- Assets/3. Scripts/BSE/Json <- 생성된 로더 클래스

## 🛠️ 사용 방법

1. Unity 프로젝트 내 Assets/Excel 폴더에 .xlsx 파일을 넣습니다.

2. 상단 메뉴에서 Tools > Excel -> Generate(JSON + Class + Loader) 클릭

3. 자동으로 다음 파일들이 생성됩니다:

   - 파일명.json (JSON 데이터)

   - 파일명.cs (데이터 클래스)

   - 파일명Loader.cs (로더 클래스)

## 📌 주의사항

- Excel 파일은 반드시 .xlsx 확장자여야 합니다.

- 임시 파일(~$)은 자동으로 무시됩니다.

- 열 타입은 자동 추론되며, 타입 혼합 시 에러가 발생합니다.

- 필드명에 한글이 있을 경우 자동으로 경고가 출력됩니다.

- Json.NET(Newtonsoft.Json)이 필요합니다.

## 📦 패키지 검색 안될 시

- 직접 패키지 추가

   Unity 메뉴 > Window > Package Manager > 왼쪽 상단 + 버튼 > Add package by name... 선택 후 아래 입력:

   com.unity.nuget.newtonsoft-json
