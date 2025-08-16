using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [SerializeField] private Button _button;

    [SerializeField] private TextMeshProUGUI _fileNameText;
    private string _saveFilePath;

    void OnEnable() => _button.onClick.AddListener(OnClick);
    void OnDisable() => _button.onClick.RemoveListener(OnClick);

    public void SetData(string path)
    {
        _saveFilePath = path;
        _fileNameText.text = Path.GetFileNameWithoutExtension(path); // 확장자 없애고 파일이름만
    }

    public void OnClick()
    {
        SaveSData.SelectedSavePath = _saveFilePath;
    }
}

public static class SaveSData
{
    public static string SelectedSavePath;
}