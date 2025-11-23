using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public int Money=0;
    public float MoneyBonus=1f;//賺錢倍率
    public BondData bondAB;//列表中，A與B的羈絆值，以下同
    public BondData bondBC;
    public BondData bondCA;
    public List<IdolInstance> idolsPicked;//選進隊伍的三名偶像
    public List<ItemStack> items = new List<ItemStack>();
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
        SetupResourceFromGameManager();
    }
    public void SetupResourceFromGameManager()
    {
        ResourceSaveData resourceSaveData=GameManager.Instance.ResourceData;
        Money = resourceSaveData.Money;
        MoneyBonus = resourceSaveData.MoneyBonus;
        bondAB = resourceSaveData.bondAB;//列表中，A與B的羈絆值，以下同
        bondBC = resourceSaveData.bondBC;
        bondCA = resourceSaveData.bondCA;
        items = resourceSaveData.items;
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
        FindAnyObjectByType<MainCanvasSetter>().setResourceUI();
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
        bool found = false;
        for (int i = 0; i < items.Count; i++)
        {
            //創建一個內容和查找對象相同的stack->改數量->設置成相同的
            if (items[i].item == newItem&& items[i].quantity< items[i].item.maxStack)
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
        Debug.Log("追加道具"+ newItem.itemName);
    }
    //指定數量來新增道具(scriptable obj 可以用"=="來判斷相同)
    public void AddItem(Item newItem, int amount = 1)
    {
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
    }
    //用列表方式新增道具(scriptable obj 可以用"=="來判斷相同)
    public void AddItem(List<Item> addList)
    {
        foreach (var newItem in addList)
        {
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
    }
    //使用道具
    public void UseItem(Item item, IdolInstance target)
    {
        if(RemoveItem(item, 1)) item.Use(target);
    }
    //移除道具
    public bool RemoveItem(Item itemRemoved,int quantity)
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
}
