using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SoundUI : MonoBehaviour
{

    [Header("Options Sound Settings")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Button _bgmToggleButton;
    [SerializeField] private Button _sfxToggleButton;
    [SerializeField] private TextMeshProUGUI _bgmButtonText;
    [SerializeField] private TextMeshProUGUI _sfxButtonText;
    [SerializeField] private TextMeshProUGUI _bgmVolumeText;
    [SerializeField] private TextMeshProUGUI _sfxVolumeText;

    private float prevBGMVolume;
    private float prevSFXVolume;
    private void Start()
    {
        // 볼륨 초기화 및 UI 상태 설정
        prevBGMVolume = Init("BGMVolume", _bgmSlider, _bgmButtonText, _bgmVolumeText);
        prevSFXVolume = Init("SFXVolume", _sfxSlider, _sfxButtonText, _sfxVolumeText);



        // 슬라이더 값 변경 시 볼륨 적용
        _bgmSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
        _sfxSlider.onValueChanged.AddListener(OnSFXSliderValueChanged);

        // 토글 버튼 클릭 시 음소거/해제
        _bgmToggleButton.onClick.AddListener(OnBGMToggleButtonClicked);
        _sfxToggleButton.onClick.AddListener(OnSFXToggleButtonClicked);



    }
    // 슬라이더 및 버튼 UI 초기화
    private float Init(
        string key,
        Slider slider,
        TextMeshProUGUI buttonText,
        TextMeshProUGUI volumeText
    )
    {
        float volume = PlayerPrefs.GetFloat(key, 0.5f);
        slider.value = volume;

        bool isMuted = volume <= 0.0001f;
        buttonText.text = isMuted ? "off" : "on";
        volumeText.text = Mathf.RoundToInt(volume * 100f).ToString();

        ApplyVolume(key, volume);
        return volume;
    }

    private void OnBGMSliderValueChanged(float value)
    {
        OnVolumeChanged("BGMVolume", value, _bgmVolumeText);
    }

    private void OnSFXSliderValueChanged(float value)
    {
        OnVolumeChanged("SFXVolume", value, _sfxVolumeText);
    }

    // 슬라이더 값 변경 시 볼륨 적용 및 저장
    private void OnVolumeChanged(string key, float value, TextMeshProUGUI volumeText)
    {
        ApplyVolume(key, value);
        volumeText.text = Mathf.RoundToInt(value * 100f).ToString();
        SaveVolume(key, value);
    }

    private void OnBGMToggleButtonClicked()
    {
        prevBGMVolume = ToggleVolume(_bgmSlider, _bgmButtonText, prevBGMVolume, "BGMVolume");
    }

    private void OnSFXToggleButtonClicked()
    {
        prevSFXVolume = ToggleVolume(_sfxSlider, _sfxButtonText, prevSFXVolume, "SFXVolume");
    }

    // 슬라이더 값을 0으로 설정하여 음소거 또는 이전 볼륨으로 복원
    private float ToggleVolume(
        Slider slider,
        TextMeshProUGUI buttonText,
        float prevVolume,
        string key
    )
    {
        if (slider.value > 0.0001f)
        {
            // 음소거
            prevVolume = slider.value;
            slider.value = 0f;
            buttonText.text = "off";
        }
        else
        {
            // 음소거 해제
            slider.value = prevVolume;
            buttonText.text = "on";
        }

        ApplyVolume(key, slider.value);
        SaveVolume(key, slider.value);

        return prevVolume;
    }

    // 사운드 매니저에 볼륨 적용
    private void ApplyVolume(string key, float value)
    {
        if (key == "BGMVolume")
            SoundManager.Instance.SetBGMVolume(value);
        else if (key == "SFXVolume")
            SoundManager.Instance.SetSFXVolume(value);
    }

    // PlayerPrefs에 볼륨 저장
    private void SaveVolume(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        PlayerPrefs.Save();
    }

}
