using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/CardEffects/Heal")]
public class HealEffect : CardEffectBase
{
    public int HealAmount;
    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
        //Debug.Log("開始演出動作...");
        //獲取加分量

        // 動作開始時播放動畫或特效
    }

    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        target.SetStamina(target.StageStamina + HealAmount);
    }
}
