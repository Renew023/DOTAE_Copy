using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PopupUI : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public UIType UIType; // 인스펙터에서 지정

    protected CanvasGroup _panel;
    protected RectTransform _rectTransform;
    protected Canvas _canvas;
    private Vector2 _dragOffset;

    [Header("드래그 설정")]
    [SerializeField] private bool _disableDrag = false;
    [SerializeField] private RectTransform _dragArea = null;

    [Header("자식 패널")]
    [SerializeField] private List<CanvasGroup> _subPanels = new List<CanvasGroup>();

    private Animator _animator;
    [SerializeField] private string openStateName = "Popup_Open";
    [SerializeField] private string closeStateName = "Popup_Close";

    protected bool _isOpen = false;

    private Vector2 _initialLocalPos;
    public virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _panel = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _initialLocalPos = _rectTransform.localPosition;
        if (_panel != null)
        {
            if (UIType == UIType.DefaultUI)
            {
                _panel.alpha = 1f;
                _panel.blocksRaycasts = true;
            }
            else
            {
                _panel.alpha = 0f;
                _panel.blocksRaycasts = false;
            }
        }

        foreach (var sub in _subPanels)
        {
            if (sub != null)
            {
                sub.alpha = 0f;
                sub.blocksRaycasts = false;
            }
        }
    }

    public virtual void ShowPanel()
    {
        StopAllCoroutines();
        ResetToDefault();            // 위치/자식 초기화
        UIManager.Instance.currentPopupUI.Push(this);
        _rectTransform.SetAsLastSibling();
        _isOpen = true;
        if (_panel != null)
        {
            _panel.blocksRaycasts = true;
            _panel.alpha = 1f;
        }
        StartCoroutine(PlayOpenAnimation());
    }
    public virtual void HidePanel()
    {
        // 초기화 코드를 먼저 수행
        ResetToDefault();
        UIManager.Instance.currentPopupUI.TryPop(out var popupUI);
        _rectTransform.SetAsFirstSibling();

        _isOpen = false;

        foreach (var sub in _subPanels)
        {
            if (sub != null)
            {
                sub.alpha = 0f;
                sub.blocksRaycasts = false;
            }
        }

        StartCoroutine(PlayCloseAnimation());
    }

    protected virtual IEnumerator PlayOpenAnimation()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("isOpen");

            // 한 프레임 대기해서 상태 전환 기다림
            yield return null;

            // 실제로 상태에 진입할 때까지 대기
            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(openStateName))
                yield return null;

            float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
        }

        foreach (var sub in _subPanels)
        {
            if (sub != null)
            {
                sub.alpha = 1f;
                sub.blocksRaycasts = true;
            }
        }
        // 애니메이션 끝난 뒤 훅
        AfterOpen();
    }

    protected virtual IEnumerator PlayCloseAnimation()
    {
        AfterClose();
        if (_animator != null)
        {
            _animator.SetTrigger("isClose");

            yield return null;

            while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(closeStateName))
                yield return null;

            float duration = _animator.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(duration);
        }

        if (_panel != null)
        {
            _panel.alpha = 0f;
            _panel.blocksRaycasts = false;
        }

    }
    protected virtual void AfterOpen()
    {
        // 열기 애니메이션 직후, 항상 자식 패널 보이도록 강제
        foreach (var sub in _subPanels)
        {
            if (sub != null)
            {
                sub.alpha = 1f;
                sub.blocksRaycasts = true;
            }
        }
    }

    protected virtual void AfterClose()
    {
        // 닫기 애니메이션 직후, CanvasGroup 완전 투명·비차단
        if (_panel != null)
        {
            _panel.alpha = 0f;
            _panel.blocksRaycasts = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (_disableDrag) return;

        if (_dragArea != null && !RectTransformUtility.RectangleContainsScreenPoint(_dragArea, eventData.position, eventData.pressEventCamera))
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _dragOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_disableDrag) return;

        if (_dragArea != null && !RectTransformUtility.RectangleContainsScreenPoint(_dragArea, eventData.position, eventData.pressEventCamera))
            return;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out var pointerPos))
        {
            Vector2 newPosition = pointerPos - _dragOffset;
            _rectTransform.localPosition = ClampToCanvas(newPosition);
        }
    }

    private Vector2 ClampToCanvas(Vector2 targetPosition)
    {
        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();

        Vector2 halfSize = _rectTransform.sizeDelta * _rectTransform.lossyScale / 2f;
        Vector2 canvasHalfSize = canvasRect.sizeDelta / 2f;

        float clampedX = Mathf.Clamp(targetPosition.x, -canvasHalfSize.x + halfSize.x, canvasHalfSize.x - halfSize.x);
        float clampedY = Mathf.Clamp(targetPosition.y, -canvasHalfSize.y + halfSize.y, canvasHalfSize.y - halfSize.y);

        return new Vector2(clampedX, clampedY);
    }
    private void ResetToDefault()
    {
        // 위치 복원
        _rectTransform.localPosition = _initialLocalPos;
        // 자식 패널도 원상복구
        foreach (var sub in _subPanels)
        {
            sub.alpha = 0f;
            sub.blocksRaycasts = false;
        }
        // 중첩된 코루틴 정리
        StopAllCoroutines();
        // 애니메이터 트리거 초기화
        if (_animator != null)
        {
            _animator.ResetTrigger("isOpen");
            _animator.ResetTrigger("isClose");
        }
    }
}
