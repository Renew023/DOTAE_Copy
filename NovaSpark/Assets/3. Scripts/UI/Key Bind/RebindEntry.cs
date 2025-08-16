using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class RebindEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text _actionName;
    [SerializeField] private Button _rebindButton;

    private InputAction _action;
    private int _bindingIndex;
    private TMP_Text _buttonLabel;
    private System.Action<InputAction, int, TMP_Text> _onRebindRequested;

    public void Setup(InputAction action, System.Action<InputAction, int, TMP_Text> onRebind)
    {
        _action = action;
        _onRebindRequested = onRebind;
        _actionName.text = action.name;

        // Button 자식에서 TMP_Text 가져오기
        _buttonLabel = _rebindButton.GetComponentInChildren<TMP_Text>();

        // (가정) 바인딩 인덱스가 하나 뿐이라면 0
        _bindingIndex = 0;

        RefreshBindingDisplay();

        _rebindButton.onClick.AddListener(() =>
            _onRebindRequested?.Invoke(_action, _bindingIndex, _buttonLabel)
        );
    }

    public void RefreshBindingDisplay()
    {
        var binding = _action.bindings[_bindingIndex];
        // 사람 읽기 편한 문자열로 변환
        var human = InputControlPath.ToHumanReadableString(
            binding.effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        _buttonLabel.text = human;
    }
}
