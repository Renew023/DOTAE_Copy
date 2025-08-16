using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SlotReel : MonoBehaviour
{
    [SerializeField] private Image _iconImage; // 슬롯 UI 이미지

    private List<SlotData> _slotDataList; // 슬롯 데이터
    private bool _isSpinning = false; // 슬롯 돌아가고 있는지 체크
    private float _interval = 0.1f; // 아이콘 바뀌는 간격
    private SlotData _resultData; // 회전 후 결과 데이터
    private Dictionary<string, Sprite> _spriteCache = new();

    // 릴 초기화
    public async Task InitAsync(List<SlotData> dataList)
    {
        this._slotDataList = dataList;
        _spriteCache.Clear();

        foreach (var data in dataList)
        {
            if (!_spriteCache.ContainsKey(data.icon))
            {
                var sprite = await AddressableManager.Instance.LoadIcon(data.icon);
                if (sprite != null)
                {
                    _spriteCache[data.icon] = sprite;
                }
                else
                {
                    Debug.LogError("아이콘 로드 실패: {data.icon}");
                }
            }
        }
    }

    // 릴 회전 시작
    public void StartSpin(float duration)
    {
        if (!_isSpinning)
            StartCoroutine(Spin(duration, _interval));
    }

    // 릴 회전 코루틴 일정 시간 동안 랜덤 아이콘 보여준 뒤 결과 표시
    private IEnumerator Spin(float duration, float interval)
    {
        _isSpinning = true;
        float timer = 0f;

        // 회전 중, 일정 시간 동안 무작위 아이콘을 표시
        while (timer < duration)
        {
            var randomIcon = _slotDataList[Random.Range(0, _slotDataList.Count)];
            if (_spriteCache.TryGetValue(randomIcon.icon, out var sprite))
            {
                _iconImage.sprite = sprite;
            }

            timer += interval;
            yield return new WaitForSeconds(interval);
        }

        // 회전 종료 후 확률 기반으로 최종 결과 선택
        _resultData = Pick(_slotDataList);

        // 결과 아이콘 표시
        if (_spriteCache.TryGetValue(_resultData.icon, out var resultSprite))
        {
            _iconImage.sprite = resultSprite;
        }

        // 회전 정지 사운드 재생
        SoundManager.Instance.PlaySFXAsync("Sound/SFX/slot_stop");
        _isSpinning = false;
    }

    // 확률 기반으로 하나를 선택
    public SlotData Pick(List<SlotData> dataList)
    {
        int total = 0;
        foreach (var data in dataList)
        {
            total += data.probability;
        }

        int rand = Random.Range(0, total);
        int cumulative = 0;

        foreach (var data in dataList)
        {
            cumulative += data.probability;
            if (rand < cumulative)
                return data;
        }

        return dataList[Random.Range(0, dataList.Count)];
    }

    // 마지막 결과 반환
    public SlotData GetResult() => _resultData;
}