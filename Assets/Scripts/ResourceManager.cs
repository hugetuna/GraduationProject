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
    private int Money=0;
    public float MoneyBonus=1f;//賺錢倍率
    public BondData bondAB;//列表中，A與B的羈絆值，以下同
    public BondData bondBC;
    public BondData bondCA;
    public List<IdolInstance> idolsPicked;//選進隊伍的三名偶像
    public List<ItemStack> items = new List<ItemStack>();
    void Start()
    {
        setupResourceFromGameManager();
    }
    public void setupResourceFromGameManager()
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
    //用列表方式新增道具(scriptable obj 可以用"=="來判斷相同)
    public void AddItem(Item newItem)
    {
        bool found = false;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == newItem)
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
    public void AddItem(List<Item> addList)
    {
        foreach (var newItem in addList)
        {
            bool found = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == newItem)
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
