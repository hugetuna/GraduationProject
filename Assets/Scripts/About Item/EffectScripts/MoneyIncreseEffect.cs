using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/MoneyIncrese")]
public class MoneyIncreseEffect : ItemEffect
{
    public int moneyIncreseAmount = 0;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        manager.Money += moneyIncreseAmount;
        Debug.Log($"[錢增加] 增加 {moneyIncreseAmount} ，現在錢{manager.Money}");
    }
}
