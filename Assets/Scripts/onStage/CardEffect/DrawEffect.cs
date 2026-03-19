using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Stage/CardEffects/Draw")]
public class DrawEffect : CardEffectBase
{
    public int cardsToDraw = 1; // 預設抽一張牌
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
            onStageManager.DrawCards(cardsToDraw);
            Debug.Log($"抽了 {cardsToDraw} 張牌！");
        }
    }
}
