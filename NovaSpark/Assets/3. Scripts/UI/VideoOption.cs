using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoOption : MonoBehaviour
{
    private const string PrefResolutionIndex = "video_resolution_index";
    private const string PrefIsFullScreen = "video_is_fullscreen";

    [Header("VideoOption Settings")]
    [SerializeField] private Button _applyResolutionButton;      // 해상도 적용 버튼
    [SerializeField] private Toggle _fullScreenToggle;          // 전체화면 토글
    [SerializeField] private TMP_Dropdown _resolutionDropdown;  // 해상도 드롭다운

    private FullScreenMode _screenMode;
    private List<Resolution> _resolutions = new List<Resolution>();
    private int _resolutionIndex = 0;

    // 16:9 해상도 후보
    private readonly List<(int width, int height)> _commonResolutions = new()
    {
        (1920, 1080),
        (1600, 900),
        (1366, 768),
        (1280, 720)
    };

    void Start()
    {
        if (_applyResolutionButton == null || _fullScreenToggle == null || _resolutionDropdown == null)
        {
            return;
        }

        // 리스너 초기화
        _applyResolutionButton.onClick.RemoveAllListeners();
        _fullScreenToggle.onValueChanged.RemoveAllListeners();
        _resolutionDropdown.onValueChanged.RemoveAllListeners();

        LoadSavedSettings();
        InitResolution();

        _applyResolutionButton.onClick.AddListener(ApplySelectedResolution);
        _fullScreenToggle.onValueChanged.AddListener(OnFullScreenToggleChanged);
        _resolutionDropdown.onValueChanged.AddListener(OnResolutionDropdownChanged);
    }

    private void LoadSavedSettings()
    {
        bool isFull = PlayerPrefs.GetInt(PrefIsFullScreen, Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? 1 : 0) == 1;
        _fullScreenToggle.isOn = isFull;
        _screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    private void InitResolution()
    {
        _resolutionDropdown.options.Clear();
        _resolutions.Clear();

        int optionIndex = 0;
        int savedIndex = PlayerPrefs.GetInt(PrefResolutionIndex, -1);
        int fallbackIndex = 0;

        foreach (var target in _commonResolutions)
        {
            foreach (var screenRes in Screen.resolutions)
            {
                if (screenRes.width == target.width && screenRes.height == target.height)
                {
                    _resolutions.Add(screenRes);
                    var option = new TMP_Dropdown.OptionData($"{screenRes.width} x {screenRes.height}");
                    _resolutionDropdown.options.Add(option);

                    if (Screen.width == screenRes.width && Screen.height == screenRes.height)
                        fallbackIndex = optionIndex;

                    optionIndex++;
                    break;
                }
            }
        }

        if (savedIndex >= 0 && savedIndex < _resolutions.Count)
            _resolutionIndex = savedIndex;
        else
            _resolutionIndex = fallbackIndex;

        _resolutionDropdown.value = _resolutionIndex;
        _resolutionDropdown.RefreshShownValue();

        _screenMode = _fullScreenToggle.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    private void ApplySelectedResolution()
    {
        _resolutionIndex = _resolutionDropdown.value;
        if (_resolutionIndex < 0 || _resolutionIndex >= _resolutions.Count)
        {
            return;
        }

        var resolution = _resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, _screenMode);

        // 저장
        PlayerPrefs.SetInt(PrefResolutionIndex, _resolutionIndex);
        PlayerPrefs.SetInt(PrefIsFullScreen, _fullScreenToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void OnFullScreenToggleChanged(bool isFull)
    {
        _screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        ApplySelectedResolution(); // 전체화면 변경도 즉시 적용+저장
    }

    private void OnResolutionDropdownChanged(int newIndex)
    {
        _resolutionIndex = newIndex;
    }
}
