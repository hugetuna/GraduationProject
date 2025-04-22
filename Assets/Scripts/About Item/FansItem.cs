using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PriceType {Fans=0,Money=1,Item=2 }//0.1.2
[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Fans")]
public class FansItem : Item
{
    public PriceType priceType=PriceType.Fans;

    public IdolInstance harvester;
    public List<ItemEffect> effects;
    public void SetPriceType(PriceType Type)
    {
        priceType = Type;
    }
    public void SetHarvester(IdolInstance target)
    {
        harvester = target;
    }
    public override void Use()
    {
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (harvester == null)
        {
            Debug.LogError("找不到 IdolInstance！");
            return;
        }
        if (effects.Count == 3)
        {
            effects[(int)priceType].Apply(harvester, resourceManager);
        }
        else
        {
            Debug.LogError("效果種類不完善(需要Fans,Money,Item三種)");
        }
            
        Debug.Log($"{itemName} 被使用！");
    }
}
