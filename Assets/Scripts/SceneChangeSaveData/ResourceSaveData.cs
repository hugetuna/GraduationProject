using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ItemSaveStack
{
    public string itemID;
    public int quantity;
    public bool isFansItem;//是否為粉絲道具，粉絲道具的ID會以F開頭
    public IdolWho Harvester;//如果是粉絲道具，則記錄是哪個偶像的粉絲道具
    public ItemSaveStack(string itemID, int quantity, IdolWho harvester=(IdolWho)(-1))
    {
        this.itemID = itemID;
        this.quantity = quantity;
        if(itemID.StartsWith("F"))
        {
            isFansItem = true;
            // 假設 IdolWho 枚舉有對應的偶像，可以根據 itemID 解析出 Harvester
            Harvester = harvester;
        }
        else
        {
            isFansItem = false;
            Harvester = default;
        }
    }
}

[System.Serializable]
public class ResourceSaveData
{
    public int Money;
    public float MoneyBonus = 1f;//賺錢倍率
    public BondData bondAB;//列表中，A與B的羈絆值，以下同
    public BondData bondBC;
    public BondData bondCA;
    public List<ItemSaveStack> saveItems = new List<ItemSaveStack>();
    public List<string> allEqupmentNames;
}
