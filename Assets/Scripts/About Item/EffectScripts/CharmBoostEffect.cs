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
        Debug.Log($"[�y�O�W�[] �ȮɼW�[ {charmBoostAmount} �I�A�{�b�O {target.charmInCount}");
    }
}
