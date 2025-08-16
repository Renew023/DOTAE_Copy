using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveListUI : MonoBehaviour
{
    [SerializeField] private GameObject _saveSlotPrefab;
    [SerializeField] private Transform _contentRoot;
    [SerializeField] private Button _saveButton;
    [SerializeField] private TMP_InputField _inputField;

    void OnEnable() => _saveButton.onClick.AddListener(OnClickSave);
    void OnDisable() => _saveButton.onClick.RemoveListener(OnClickSave);

    private void Start()
    {
        var savePaths = SaveManager.Instance.GetRecentSaveFiles();

        foreach (var path in savePaths)
        {
            var slotGO = Instantiate(_saveSlotPrefab, _contentRoot);
            var slot = slotGO.GetComponent<SaveSlotUI>();
            slot.SetData(path);
        }
    }

    private void OnClickSave()
    {
        string saveName = _inputField.text;

        if (string.IsNullOrWhiteSpace(saveName))
        {
            Debug.LogWarning("저장 이름이 비어 있습니다");
            return;
        }

        SaveManager.Instance.SaveGame(saveName);
    }
}