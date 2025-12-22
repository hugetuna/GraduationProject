using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : Item
{
    public override void Use(IdolInstance target)
    {
        // 透過函式引數可指定使用道具的偶像
        ResourceManager resourceManager = FindAnyObjectByType<ResourceManager>();
        if (target == null || resourceManager == null)
        {
            Debug.LogError("找不到 IdolInstance 或 ResourceManager");
            return;
        }

        Debug.Log($"{itemName} 使用完畢");
    }
}
