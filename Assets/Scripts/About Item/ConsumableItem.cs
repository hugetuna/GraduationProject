using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    public List<ItemEffect> effects;

    public override void Use()
    {
        //todo 使玩家自己選用道具施放對象
        IdolInstance target = FindObjectOfType<IdolInstance>();
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (target == null|| resourceManager==null)
        {
            Debug.LogError("找不到 IdolInstance或resourceManager！");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Apply(target, resourceManager);
        }

        Debug.Log($"{itemName} 被使用！");
    }
}
