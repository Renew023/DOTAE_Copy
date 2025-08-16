using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopItemBase : MonoBehaviour
{
    //private DataManager _db => DataManager.Instance;

    //public List<Item> GetItemsByType(ShopType type)
    //{
    //    switch (type)
    //    {
    //        case ShopType.Weapon:
    //            return _db.AllEquipmentItems
    //                .Where(e => e.type == DesignEnums.EquipmentType.Weapon) // && e.isShopItem) 상점리스트에 들어갈 bool 엑셀에 작성
    //                .Cast<Item>()
    //                .ToList();

    //        case ShopType.Armor:
    //            return _db.AllEquipmentItems
    //                .Where(e => e.type == DesignEnums.EquipmentType.Armor)
    //                .Cast<Item>()
    //                .ToList();

    //        case ShopType.Consumable:
    //            return _db.AllConsumableItems.Cast<Item>().ToList();

    //        case ShopType.Material:
    //            return _db.AllMaterialItems.Cast<Item>().ToList();

    //        case ShopType.Mixed:
    //            return _db.AllEquipmentItems
    //                .Concat<Item>(_db.AllConsumableItems)
    //                .Concat(_db.AllMaterialItems)
    //                .ToList();

    //        default:
    //            Debug.LogWarning($"지원되지 않는 상점 타입: {type}");
    //            return new List<Item>();
    //    }
    //}
}
