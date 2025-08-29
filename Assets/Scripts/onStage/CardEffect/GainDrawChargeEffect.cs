using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/CardEffects/GainDrawCharge")]
public class GainDrawChargeEffect : CardEffectBase
{
    public int drawChargeAmount;
    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
        //Debug.Log("開始演出動作...");
        //獲取加分量

        // 動作開始時播放動畫或特效
    }

    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        onStageManager.GaindrawCharge(drawChargeAmount);
        Debug.Log($"獲得共{drawChargeAmount}充能");
    }
}
