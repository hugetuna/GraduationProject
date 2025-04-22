using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/FansIncrese")]
public class FansIncreseEffect : ItemEffect
{
    public int fansIncreseAmount=0;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        target.FansIncrease(fansIncreseAmount);
        Debug.Log($"[�����W�[] �W�[ {fansIncreseAmount} �A�{�b����{target.fans}");
    }
}
