using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Stage/CardEffects/FansBonusMutipli")]
public class FansBonusMutipliEffect : CardEffectBase
{
    public float multiplier = 1.5f; // 粉絲加成倍數
    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
        Debug.Log("開始演出動作...");
        //獲取加分量

        // 動作開始時播放動畫或特效
    }

    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        if (parentCard != null)
        {
            target.StageFansPointMutiplier = multiplier;
        }
    }
}
