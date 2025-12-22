using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    public List<ItemEffect> effects;

    public override void Use(IdolInstance target)
    {
        // 透過函式引數可指定使用道具的偶像
        //IdolInstance target = FindObjectOfType<IdolInstance>();
        ResourceManager resourceManager = FindAnyObjectByType<ResourceManager>();
        if (target == null|| resourceManager==null)
        {
            Debug.LogError("找不到 IdolInstance 或 ResourceManager");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Apply(target, resourceManager);
        }

        Debug.Log($"{itemName} 使用完畢");
    }
}
