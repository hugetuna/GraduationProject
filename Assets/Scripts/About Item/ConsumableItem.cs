using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : Item
{
    public float voTrainingBonus;
    public float daTrainingBonus;
    public float viTrainingBonus;
    public int vigourHeal;
    public float moneyBonus;
    public int charmAmout;
    public override void Use()
    {
        Debug.Log(itemName + " 被使用，回復 ");
        // TODO: 加入邏輯
    }
}