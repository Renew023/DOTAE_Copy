using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ShopInfo : MonoBehaviour
{
    public DesignEnums.NPCType type; // 상점 타입 (무기, 방어구 등)
    public int refreshIntervalDays; // 며칠마다 갱신
    public int lastRefreshDay;// 마지막 갱신된 날짜

    public List<Item> itemPool; // 직접 지정 아이템

    public int maxTotalValue = 3000; // 상점 전체 아이템의 총합 가치 제한
}
