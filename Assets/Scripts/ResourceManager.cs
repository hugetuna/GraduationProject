using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NUnit.Framework;

[System.Serializable]
public struct ItemStack
{
    public Item item;
    public int quantity;

    public ItemStack(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}
public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public InventoryManager InventoryManager;

    public int Money = 0;
    public float MoneyBonus = 1f;//賺錢倍率
    public BondData bondAB;//列表中，A與B的羈絆值，以下同
    public BondData bondBC;
    public BondData bondCA;
    public List<IdolInstance> idolsPicked;//選進隊伍的三名偶像
    public List<ItemStack> items = new List<ItemStack>();
    public bool IsItemChanged { get; private set; } = true; // 標記道具是否有變化
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnSceneLoaded()
    {
        //SetupResourceFromGameManager();
        IsItemChanged = true; // 每換場景即刷新一次道具相關 UI（例如背包）
    }
    public void OnGameFileLoad()
    {
        SetupResourceFromGameManager();
    }
    public void SetupResourceFromGameManager()
    {
        ResourceSaveData resourceSaveData = GameManager.Instance.ResourceData;
        Money = resourceSaveData.Money;
        MoneyBonus = resourceSaveData.MoneyBonus;
        bondAB = resourceSaveData.bondAB;//列表中，A與B的羈絆值，以下同
        bondBC = resourceSaveData.bondBC;
        bondCA = resourceSaveData.bondCA;
        items = resourceSaveData.items;
        //裝備類道具需要從名稱找回物件
        InventoryManager.ownedEquipments.Clear();
        foreach (var itemName in resourceSaveData.allEqupmentNames)
        {
            InventoryManager.ownedEquipments.Add(InventoryManager.FindEquipmentByName(itemName));
        }
    }
    //每天結束時必須重製資源暫時狀態
    public void ResetTemporaryEffect()
    {
        MoneyBonus = 1f;
    }
    //獲得金錢
    public void GainMoney(int gain)
    {
        Money += (int)(gain * MoneyBonus);
        
        // 避免找不到主介面時報錯
        var mainUI = FindAnyObjectByType<MainCanvasSetter>();
        if (mainUI != null) mainUI.setResourceUI();
    }
    public void SpendMoney(int spend)
    {
        if (spend <= Money)
        {
            Money -= spend;
        }
        else
        {
            Debug.Log("錢不夠花");
        }
    }
    public int getMoney() { return Money; }
    //新增道具(scriptable obj 可以用"=="來判斷相同)
    public void AddItem(Item newItem)
    {
        //裝備類道具直接加入擁有清單
        if (newItem.itemType==ItemType.Equipment)
        {
            InventoryManager.ownedEquipments.Add(newItem as EquipmentItem);
            return;
        }
        bool found = false;
        for (int i = 0; i < items.Count; i++)
        {
            //創建一個內容和查找對象相同的stack->改數量->設置成相同的
            if (items[i].item == newItem && items[i].quantity < items[i].item.maxStack)
            {
                ItemStack stack = items[i];
                stack.quantity += 1;
                items[i] = stack;
                found = true;
                break;
            }
        }
        if (!found)
        {
            items.Add(new ItemStack(newItem, 1));
        }
        Debug.Log("追加道具" + newItem.itemName);
        SetItemChanged(true); // 標記道具已更改
    }
    //指定數量來新增道具(scriptable obj 可以用"=="來判斷相同)
    public void AddItem(Item newItem, int amount = 1)
    {
        //裝備類道具直接加入擁有清單
        if (newItem.itemType == ItemType.Equipment)
        {
            for (int i = 0; i < amount; i++)
                InventoryManager.ownedEquipments.Add(newItem as EquipmentItem);
            return;
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == newItem)
            {
                int canAdd = items[i].item.maxStack - items[i].quantity;
                if (canAdd > 0)
                {
                    int addAmount = Mathf.Min(amount, canAdd);
                    ItemStack stack = items[i];
                    stack.quantity += addAmount;
                    items[i] = stack;
                    amount -= addAmount;
                    if (amount <= 0) return; // 已經加完
                }
            }
        }
        // 如果還有剩下的數量，就新開一格
        while (amount > 0)
        {
            int addAmount = Mathf.Min(amount, newItem.maxStack);
            items.Add(new ItemStack(newItem, addAmount));
            amount -= addAmount;
        }
        SetItemChanged(true); // 標記道具已更改
    }
    //用列表方式新增道具(scriptable obj 可以用"=="來判斷相同)
    public void AddItem(List<Item> addList)
    {
        foreach (var newItem in addList)
        {
            //裝備類道具直接加入擁有清單
            if (newItem.itemType == ItemType.Equipment)
            {
                InventoryManager.ownedEquipments.Add(newItem as EquipmentItem);
                break;
            }
            bool found = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == newItem && items[i].quantity < items[i].item.maxStack)
                {
                    ItemStack stack = items[i];
                    stack.quantity += 1;
                    items[i] = stack;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                items.Add(new ItemStack(newItem, 1));
            }
        }
        SetItemChanged(true); // 標記道具已更改
    }
    //使用道具
    public void UseItem(Item item, IdolInstance target)
    {
        if (RemoveItem(item, 1)) item.Use(target);
    }
    //移除道具
    public bool RemoveItem(Item itemRemoved, int quantity)
    {
        bool found = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == itemRemoved)
            {
                ItemStack stack = items[i];
                stack.quantity -= quantity;
                if (stack.quantity <= 0)
                {
                    items.RemoveAt(i);
                }
                else
                {
                    items[i] = stack; // 還要記得回存
                }
                found = true;
                SetItemChanged(true); // 標記道具已更改
                return found;
            }
        }
        if (!found)
        {
            Debug.Log("道具欄中沒有該道具");
        }
        return found;
    }
    //查找指定道具的數量
    public int GetItemCount(Item target)
    {
        foreach (var stack in items)
        {
            if (stack.item == target)
                return stack.quantity;
        }
        return 0;
    }

    public void SetItemChanged(bool isChanged)
    {
        IsItemChanged = isChanged;
        if (isChanged)
        {
            Debug.Log("背包資料已更新，標記為 Dirty");
        }
    }
}
