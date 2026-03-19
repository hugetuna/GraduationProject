using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Stage/CardEffects/Heal")]
public class HealEffect : CardEffectBase
{
    public int HealAmount;
    public bool selfHeal = true;//治療自己，預設值
    public bool allHeal = false;//治療全體
    public bool sideHeal = false;//治療相鄰
    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
        //Debug.Log("開始演出動作...");
        //獲取加分量

        // 動作開始時播放動畫或特效
    }

    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        if (selfHeal)
        {
            target.SetStamina(target.StageStamina + HealAmount);
        }
        if (allHeal)
        {
            foreach (var idol in onStageManager.onStageIdols)
            {
                IdolOnStage idolOnStage = idol.GetComponent<IdolOnStage>();
                idolOnStage.SetStamina(idolOnStage.StageStamina + HealAmount);
            }
        }
        if (sideHeal)//治療相鄰
        {
            int targetIndex = onStageManager.onStageIdols.IndexOf(target.GetComponent<IdolInstance>());
            if (targetIndex > 0)
            {
                IdolOnStage leftNeighbor = onStageManager.onStageIdols[targetIndex - 1].GetComponent<IdolOnStage>();
                leftNeighbor.SetStamina(leftNeighbor.StageStamina + HealAmount);
            }
            if (targetIndex < onStageManager.onStageIdols.Count - 1)
            {
                IdolOnStage rightNeighbor = onStageManager.onStageIdols[targetIndex + 1].GetComponent<IdolOnStage>();
                rightNeighbor.SetStamina(rightNeighbor.StageStamina + HealAmount);
            }
        }

    }
}
