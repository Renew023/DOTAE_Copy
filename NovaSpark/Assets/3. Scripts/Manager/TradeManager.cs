using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeManager : Singleton<TradeManager>
{
    // 판매 가능 수량 계산
    public int GetMaxSellAmount(Item item, int playerQty, int shopGold, int unitSellPrice)
    {
        if (!item.isStackable || unitSellPrice <= 0)
            return 1;

        int maxByMoney = shopGold / unitSellPrice;
        return Mathf.Min(playerQty, maxByMoney);
    }

    // 구매 가능 수량 계산
    public int GetMaxBuyAmount(Item item, int npcQty, int playerGold, float playerFreeWeight, int unitBuyPrice)
    {
        int maxByMoney = unitBuyPrice > 0 ? playerGold / unitBuyPrice : int.MaxValue;
        int maxByWeight = Mathf.FloorToInt(playerFreeWeight / item.weight);
        return Mathf.Min(npcQty, maxByMoney, maxByWeight);
    }

    // 창고 이동 가능 수량 계산
    public int GetMaxStorageTransferAmount(Item item, int sourceQty, float playerFreeWeight)
    {
        int maxByWeight = Mathf.FloorToInt(playerFreeWeight / item.weight);
        return Mathf.Min(sourceQty, maxByWeight);
    }

    // 판매 실행
    public void DoSell(PlayerInventory player, NPCInventory shop, Item item, int amount)
    {
        int totalPrice = CalculateSellPrice(item) * amount;

        if (shop.gold < totalPrice)
        {
            Debug.LogWarning("상점의 돈 부족");
            return;
        }

        if (player.RemoveItem(item, amount))
        {
            shop.AddItem(item, amount);
            player.gold += totalPrice;
            shop.gold -= totalPrice;
            Debug.Log($"판매 완료: {item.name_kr} x{amount} → {totalPrice}골드");
        }
    }

    // 구매 실행
    public void DoBuy(NPCInventory shop, PlayerInventory player, Item item, int amount)
    {
        int totalPrice = CalculateBuyPrice(item) * amount;
        float totalWeight = item.weight * amount;

        if (player.gold < totalPrice || player.GetTotalWeight() + totalWeight > player.PlayerMaxForce)
        {
            Debug.LogWarning("구매 불가 (돈 또는 무게 초과)");
            return;
        }

        if (shop.RemoveItem(item, amount))
        {
            player.AddItem(item, amount);
            player.gold -= totalPrice;
            shop.gold += totalPrice;
            Debug.Log($"구매 완료: {item.name_kr} x{amount} → {totalPrice}골드");
        }
    }

    // 창고 → 플레이어 이동
    public void DoStorageTransfer(Storage storage, PlayerInventory player, Item item, int amount)
    {
        float totalWeight = item.weight * amount;

        if (player.GetTotalWeight() + totalWeight > player.PlayerMaxForce)
        {
            Debug.LogWarning("무게 초과로 인해 이동 불가");
            return;
        }

        if (storage.RemoveItem(item, amount))
        {
            player.AddItem(item, amount);
            Debug.Log($"창고에서 인벤토리로 이동: {item.name_kr} x{amount}");
        }
    }

    private int CalculateBuyPrice(Item item) => item.price;
    private int CalculateSellPrice(Item item) => Mathf.FloorToInt(item.price * 0.5f);
}
