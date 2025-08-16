using UnityEngine;
using UnityEngine.UI;

public class DragSlot : Singleton<DragSlot> 
{

    [HideInInspector] public IItemSlot dragSlot;

    [SerializeField] private Image imageItem;

    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        // 처음엔 투명하게
        SetColor(0);
    }
    /// <summary>
    /// 드래그할 슬롯을 설정
    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        imageItem.raycastTarget = false; // 추가
        SetColor(1);
    }

    /// <summary>
    /// 아이콘의 알파값만 변경
    public void SetColor(float _alpha)
    {
        var color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
