using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpUI : MonoBehaviour
{
    [Header("Data Source")]
    [SerializeField] private Player _player;  // 씬에 배치된 Player 컴포넌트

    [Header("HP Bar Settings")]
    [SerializeField]Image _hpBarImage;  // Hp 체력 바
    [SerializeField] private TMP_Text _hpValueText;  // "HP: 80 / 100" 같은 문자열 표시

    // 3. 캐시된 최대 HP (최초 한 번만 설정)
    private float _maxHp = -1f;  // 기본값 -1: 아직 캐싱되지 않음


    // Update는 매 프레임 호출됩니다.
    private void Update()
    {
        // 사전 체크: 필요한 참조가 모두 연결되어 있는지 확인
        if (_player == null ||                       // Player 연결 여부
            _player== null ||             // CharacterObject 연결 여부
            _player.characterRuntimeData == null ||  // StatInfo 연결 여부
            _hpBarImage == null ||                   // Image 연결 여부
            _hpValueText == null)                    // Text 연결 여부
        {
            return;  // 하나라도 없으면 더 이상 진행하지 않음
        }

        // 3-2. 테스트용 입력: G 키를 누르면 데미지를 주도록 함
        if (Input.GetKeyDown(KeyCode.G))
        {
                _player.TakeDamage(10f, _player);
        }

        // UI를 갱신하여 HP 변화 표시
        UpdateHpUI();
    }


    /// <summary>
    /// Blood 파트의 현재 HP 및 최대 HP를 가져와서
    /// - Image.fillAmount에 비율(0~1)을 설정하고,
    /// - 텍스트에 "HP: 현재 / 최대" 형식으로 표시합니다.
    /// </summary>
    private void UpdateHpUI()
    {
        // StatInfo -> CharacterRuntimeData에서 Blood 파트 정보 꺼내기
        var stat = _player.characterRuntimeData;

        if(stat.health.Current > 0)
        {
            float currentHp = Mathf.Max(stat.health.Current, 0f);
            float ratio = (stat.health.Max > 0f) ? Mathf.Clamp01(currentHp / stat.health.Max) : 0f;
            _hpBarImage.fillAmount = ratio;
            _hpValueText.text = $"HP: {currentHp:F0} / {stat.health.Max:F0}";
        }
        else
        {
            _hpBarImage.fillAmount = 0f;
            _hpValueText.text = "HP: N/A";
        }

        // 최대 HP 캐싱: 최초 한 번만 실행하여 _maxHp에 저장
        //if (_maxHp < 0f && stat.maxPartDictionary.TryGetValue(PartType.Blood, out var maxPart))
        //{
        //    _maxHp = maxPart.hp;  // Inspector에서 설정한 최대 HP 값
        //}

        // 현재 HP도 Dictionary에서 꺼내기
        //if (stat.partDictionary.TryGetValue(PartType.Blood, out var curPart))
        //{
        //    // 음수 HP 방지: 0 이하로 내려가지 않음
        //    float currentHp = Mathf.Max(curPart.hp, 0f);

        //    // 비율 계산: currentHp / _maxHp => 0~1 사이 값
        //    float ratio = (_maxHp > 0f) ? Mathf.Clamp01(currentHp / _maxHp) : 0f;

        //    // 1) bar 이미지 채우기
        //    _hpBarImage.fillAmount = ratio;

        //    // 2) 텍스트 갱신하기
        //    _hpValueText.text = $"HP: {currentHp:F0} / {_maxHp:F0}";
        //}
        //else
        //{
        //    // Blood 파트 정보가 없다면, bar와 텍스트를 초기화
        //    _hpBarImage.fillAmount = 0f;
        //    _hpValueText.text = "HP: N/A";
        //}
    }
}
