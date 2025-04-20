using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/MoneyBoost")]
public class MoneyBoostEffect : ItemEffect
{
    public float MoneyBoost = 1f;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        manager.MoneyBonus *= MoneyBoost;
    }
}
