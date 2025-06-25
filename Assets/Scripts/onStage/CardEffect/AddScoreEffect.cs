using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/CardEffects/AddScore")]
public class AddScoreEffect : CardEffectBase
{
    public int scoreToAdd;

    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
        Debug.Log("開始演出動作...");
        // 動作開始時播放動畫或特效
    }

    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        onStageManager.gainPoint(scoreToAdd,1);
        Debug.Log($"加了 {scoreToAdd} 分！");
    }
}
