using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CharSelectUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _prevButton;

    [Header("Child Panel")]
    [SerializeField] private CanvasGroup _childPanel;

    private Animator _animator;

    // 애니메이터 상태 이름
    private const string OPEN_STATE = "CharSelectUI_Open";
    private const string RIGHT_STATE = "CharSelectUI_Right";
    private const string LEFT_STATE = "CharSelectUI_Left";

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        if (_nextButton != null)
            _nextButton.onClick.AddListener(HandleNext);

        if (_prevButton != null)
            _prevButton.onClick.AddListener(HandlePrev);
    }

    private void Start()
    {
        if (_animator == null || _childPanel == null)
            return;

        // 초기에는 패널 숨김
        _childPanel.alpha = 0f;
        _childPanel.blocksRaycasts = false;

        // CharSelectUI_Open 애니메이션이 자동으로 재생된다면,
        // 해당 상태가 끝난 시점에 패널을 보이게끔 대기
        StartCoroutine(WaitForStateAndShow(OPEN_STATE));
    }

    private void HandleNext()
    {
        if (_animator == null || _childPanel == null) return;

        _childPanel.alpha = 0f;
        _childPanel.blocksRaycasts = false;

        // 1) 이전에 걸려 있던 트리거를 초기화
        _animator.ResetTrigger("isRight");
        // 2) 다시 트리거 발동
        _animator.SetTrigger("isRight");

        StartCoroutine(WaitForStateAndShow(RIGHT_STATE));
    }


    private void HandlePrev()
    {
        if (_animator == null || _childPanel == null) return;

        _childPanel.alpha = 0f;
        _childPanel.blocksRaycasts = false;

        // 1) 이전에 걸려 있던 트리거를 초기화
        _animator.ResetTrigger("isLeft");
        // 2) 다시 트리거 발동
        _animator.SetTrigger("isLeft");
        StartCoroutine(WaitForStateAndShow(LEFT_STATE));
    }

    private IEnumerator WaitForStateAndShow(string stateName)
    {
        // 상태 진입 대기
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // 애니메이션 길이만큼 대기
        float len = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(len);

        // 자식 패널 다시 보이기
        _childPanel.alpha = 1f;
        _childPanel.blocksRaycasts = true;
    }
}
