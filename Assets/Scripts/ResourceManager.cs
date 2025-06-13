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
    public int Money;
    public float MoneyBonus=1f;//賺錢倍率
    public BondData bondAB;//列表中，A與B的羈絆值，以下同
    public BondData bondBC;
    public BondData bondCA;
    public List<IdolInstance> idolsPicked;//選進隊伍的三名偶像
    public List<ItemStack> items = new List<ItemStack>();
    //每天結束時必須重製資源暫時狀態
    public void ResetTemporaryEffect()
    {
        MoneyBonus = 1f;
    }
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
