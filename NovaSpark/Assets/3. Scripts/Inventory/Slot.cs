using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IItemSlot
{
    Item ItemData { get; }
    int Quantity { get; }
    Slot GetSlot(); 
    void Refresh();
    void SetSlotData(Item item, int quantity);
    void RequestUnequip();
     Inventory GetParentInventory();
    InventoryUI GetParentInventoryUI();
   
}

[System.Serializable]
public class Slot
{
    public Item item;
    [SerializeField]
    public string itemName;
    public int quantity;

    public bool IsEmpty => item == null || quantity <= 0;
  

    public void Clear()
    {
        item = null;
        itemName = string.Empty;
        quantity = 0;
    }
}
