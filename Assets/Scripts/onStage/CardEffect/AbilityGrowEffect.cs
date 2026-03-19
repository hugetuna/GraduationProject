using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AbilityGrowType
{
    vocal,
    dance,
    visual,
    all
}
[CreateAssetMenu(menuName = "Stage/CardEffects/AbilityGrow")]
public class AbilityGrowEffect : CardEffectBase
{
    public int GrowAmount;
    public AbilityGrowType abilityGrowType;
    public bool self = true;//自己，預設值
    public bool all = false;//全體
    public bool side = false;//相鄰
    public bool others = false;//自己以外
    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
    }
    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        List<IdolOnStage> idolsToApply = new List<IdolOnStage>();
        if (self)
        {
            idolsToApply.Add(target);
        }
        if (all)
        {
            idolsToApply.AddRange(onStageManager.onStageIdols.ConvertAll(idol => idol.GetComponent<IdolOnStage>()));
        }
        if (side)//治療相鄰
        {
            int targetIndex = onStageManager.onStageIdols.IndexOf(target.GetComponent<IdolInstance>());
            if (targetIndex > 0)
            {
                IdolOnStage leftNeighbor = onStageManager.onStageIdols[targetIndex - 1].GetComponent<IdolOnStage>();
                idolsToApply.Add(leftNeighbor);
            }
            if (targetIndex < onStageManager.onStageIdols.Count - 1)
            {
                IdolOnStage rightNeighbor = onStageManager.onStageIdols[targetIndex + 1].GetComponent<IdolOnStage>();
                idolsToApply.Add(rightNeighbor);
            }
        }
        if (others)
        {
            foreach (var idol in onStageManager.onStageIdols)
            {
                if (idol.GetComponent<IdolOnStage>() != target)
                {
                    idolsToApply.Add(idol.GetComponent<IdolOnStage>());
                }
            }
        }
        foreach (var idol in idolsToApply)
        {
            switch(abilityGrowType)
            {
                case AbilityGrowType.vocal:
                    idol.StageVocal += GrowAmount;
                    break;
                case AbilityGrowType.dance:
                    idol.StageDance += GrowAmount;
                    break;
                case AbilityGrowType.visual:
                    idol.StageVisual += GrowAmount;
                    break;
                case AbilityGrowType.all:
                    idol.StageVocal += GrowAmount;
                    idol.StageDance += GrowAmount;
                    idol.StageVisual += GrowAmount;
                    break;
            }
        }
    }
}
