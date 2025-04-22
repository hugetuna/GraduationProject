using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/CharmBoost")]
public class CharmBoostEffect : ItemEffect
{
    public int charmBoostAmount=0;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        target.charmInCount = target.charm + charmBoostAmount;
        Debug.Log($"[魅力增加] 暫時增加 {charmBoostAmount} 點，現在是 {target.charmInCount}");
    }
}
