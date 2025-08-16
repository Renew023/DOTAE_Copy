using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuantitySelectUI : PopupUI
{
    [SerializeField] private TMP_InputField numberInput;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI maxAmountText;

    private int maxValue = 1;
    private Action<int> onConfirm;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        numberInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }

    public void Open(int max, Action<int> confirmCallback)
    {
        maxValue = max;
        maxAmountText.text = $"최대 수량: {maxValue}";
        numberInput.text = "1";
        onConfirm = confirmCallback;
        ShowPanel();
        numberInput.ActivateInputField();
    }

    public void OnConfirmButton()
    {
        Debug.Log("확인 버튼 눌림");

        if (int.TryParse(numberInput.text, out int value))
        {
            Debug.Log($"입력된 값: {value}, 최대값: {maxValue}");

            if (value < 1) value = 1;
            if (value > maxValue) value = maxValue;

            if (onConfirm != null)
            {
                Debug.Log("onConfirm 실행됨");
                onConfirm?.Invoke(value);
            }
            else
            {
                Debug.LogWarning("onConfirm이 null입니다.");
            }

            HidePanel();
        }
        else
        {
            Debug.LogWarning("숫자 입력이 올바르지 않습니다.");
        }
    }

    public void OnCancelButton()
    {
        Debug.Log("취소 버튼 눌림");
        HidePanel();
    }
}