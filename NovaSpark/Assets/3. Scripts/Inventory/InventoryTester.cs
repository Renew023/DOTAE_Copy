using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [SerializeField] private IInventory playerInventory;
    [SerializeField] private Item _testItem; // 아이템 프리셋
    [SerializeField] private Item _testItem1; // 아이템 프리셋
    [SerializeField] private Item _testItem2; // 아이템 프리셋
    [SerializeField] private Item _testItem3; // 아이템 프리셋
    [SerializeField] private int _amount;
    [SerializeField] private IInventory connectedStorage;

    // public Consumable tet;
    public PlayerInventory inventory;
    public PlayerRecipe recipe;


    private async void Start()
    {
        Debug.Log("InventoryTester Start 진입");

        await DataManager.Instance.WaitUntilLoaded();

        if (playerInventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }
        recipe = GetComponent<PlayerRecipe>();


        _testItem = DataManager.Instance.GetEquipmentItem(1001);
        Debug.Log(_testItem);
        //tet = DataManager.Instance.GetEquipmentItem(1001);
        //Debug.Log(tet);

        _testItem1 = DataManager.Instance.GetEquipmentItem(1002);
        _testItem2 = DataManager.Instance.GetMaterialItem(3001);
        _testItem3 = DataManager.Instance.GetMaterialItem(3003);

        Debug.Log(_testItem == null ? "1001 is null" : "1001 loaded");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q 눌림");
            playerInventory.AddItem(_testItem, 1);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerInventory.AddItem(_testItem1, 1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            playerInventory.AddItem(_testItem2, 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            playerInventory.AddItem(_testItem3, 1);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventory.MergeSameItem(connectedStorage);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventory.SortItem();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventory.SendAllItem(connectedStorage);
        }
    }

    public void ConnectStorage(IInventory storage)
    {
        connectedStorage = storage;
        Debug.Log($"창고 연결됨 {connectedStorage}");
    }
    public void DisconnectStorage()
    {
        connectedStorage = null;
        Debug.Log("창고 연결 해제됨");
    }
} 
