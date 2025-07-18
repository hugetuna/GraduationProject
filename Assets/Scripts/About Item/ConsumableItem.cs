using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    public List<ItemEffect> effects;

    public override void Use(IdolInstance target)
    {
        //todo �Ϫ��a�ۤv��ιD��I���H
        //IdolInstance target = FindObjectOfType<IdolInstance>();
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        if (target == null|| resourceManager==null)
        {
            Debug.LogError("�䤣�� IdolInstance��resourceManager�I");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Apply(target, resourceManager);
        }

        Debug.Log($"{itemName} �Q�ϥΡI");
    }
}
