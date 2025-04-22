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
            Debug.LogError("�䤣�� IdolInstance�I");
            return;
        }
        if (effects.Count == 3)
        {
            effects[(int)priceType].Apply(harvester, resourceManager);
        }
        else
        {
            Debug.LogError("�ĪG����������(�ݭnFans,Money,Item�T��)");
        }
            
        Debug.Log($"{itemName} �Q�ϥΡI");
    }
}
