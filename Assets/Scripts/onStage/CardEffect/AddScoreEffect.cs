using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/CardEffects/AddScore")]
public class AddScoreEffect : CardEffectBase
{
    //public int scoreToAdd;
    //[HideInInspector]
    //public ActionCard parentCard; // 由外部設置

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
            int score = parentCard.point;
            onStageManager.gainPoint(score, 1f);
            Debug.Log($"加了 {score} 分！");
        }
        else
        {
            Debug.LogWarning("parentCard 為 null，無法加分！");
        }
    }
}
