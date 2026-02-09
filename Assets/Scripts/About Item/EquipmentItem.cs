using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class EquipmentItem : Item
{
    public int vocalBonus;
    public int danceBonus;
    public int visualBonus;
    public int staminaBonus;
    public List<ActionCard> actionCardsAddByEquipment;
    public override void Use(IdolInstance target)
    {
        // 透過函式引數可指定使用道具的偶像
        ResourceManager resourceManager = FindAnyObjectByType<ResourceManager>();
        if (target == null || resourceManager == null)
        {
            Debug.LogError("找不到 IdolInstance 或 ResourceManager");
            return;
        }
        // 將裝備指派給目標偶像
        // 這裡不再由 SO 自己處理裝備，而是交給 InventoryManager
        ResourceManager.Instance.InventoryManager.TryEquip(this, target);
    }
    
}
