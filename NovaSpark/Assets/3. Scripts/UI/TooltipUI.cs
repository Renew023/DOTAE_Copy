using UnityEngine;
using TMPro;

public class TooltipUI : PopupUI
{
    [Header("Tooltip Parts")]
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _bodyText;

    [Tooltip("마우스 기준 오프셋")]
    [SerializeField] private Vector2 _cursorOffset = new(0.05f, 0f);

    private RectTransform _canvasRect;
    private Canvas _parentCanvas;
    private Camera _canvasCamera;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private bool _followCursor = false;

    public static TooltipUI Instance { get; private set; }

    private void Awake()
    {
        base.Awake();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _parentCanvas = GetComponentInParent<Canvas>();
        if (_parentCanvas != null)
        {
            _canvasRect = _parentCanvas.GetComponent<RectTransform>();
            if (_parentCanvas.renderMode == RenderMode.ScreenSpaceCamera || _parentCanvas.renderMode == RenderMode.WorldSpace)
                _canvasCamera = _parentCanvas.worldCamera;
            else
                _canvasCamera = null;
        }

        // 초기 숨김
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _isOpen = false;
    }

    private void Update()
    {
        if (_followCursor)
            UpdatePositionToCursor();
    }

    private void UpdatePositionToCursor()
    {
        Vector2 screenPos = Input.mousePosition;
        Vector2 anchoredPos;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        Vector2 cursorPos = new Vector2(screenWidth * _cursorOffset.x, screenHeight * _cursorOffset.y);

        if (_canvasRect != null)
        {
            bool converted = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect,
                screenPos + cursorPos,
                _canvasCamera,
                out anchoredPos);

            if (converted)
            {
                Vector2 clamped = ClampToCanvas(anchoredPos);
                _rectTransform.localPosition = clamped;
            }
        }
        else
        {
            transform.position = (Vector3)screenPos + (Vector3)cursorPos;
        }
    }

    private Vector2 ClampToCanvas(Vector2 pos)
    {
        if (_canvasRect == null)
            return pos;

        Vector2 halfSize = _rectTransform.rect.size * 0.5f;
        Vector2 min = _canvasRect.rect.min + halfSize;
        Vector2 max = _canvasRect.rect.max - halfSize;

        return new Vector2(
            Mathf.Clamp(pos.x, min.x, max.x),
            Mathf.Clamp(pos.y, min.y, max.y)
        );
    }

    // 즉시 보여줌 (alpha 0->1)
    public void Show(string title, string body)
    {
        if (_titleText != null)
            _titleText.text = title;
        if (_bodyText != null)
            _bodyText.text = body;

        _rectTransform.SetAsLastSibling();


        _followCursor = true;

        // 직접 노출
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _isOpen = true;

        // PopupUI 스택에 올리기 (중복 보호는 base 구현에 맡김)
        base.ShowPanel();

        UpdatePositionToCursor();
    }

    // 즉시 숨김 (alpha 1->0)
    public void HideTooltip()
    {
        _followCursor = false;

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _isOpen = false;

        base.HidePanel();
    }
}
