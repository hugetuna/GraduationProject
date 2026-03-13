using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/MoneyBoost")]
public class MoneyBoostEffect : ItemEffect
{
    public float MoneyBoost = 1f;
    public string boostEffectID;
    public string displayName; // 方便顯示給玩家看的名稱
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        manager.MoneyBonus *= MoneyBoost;

        var effectInfo = new ItemEffectInfo { id = boostEffectID, displayName = displayName };
        ItemEffectUtility.SaveGlobalEffect(effectInfo);
    }
}
