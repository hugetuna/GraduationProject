using UnityEngine;

public abstract class CardEffectBase : ScriptableObject
{
    [HideInInspector]
    public ActionCard parentCard; // 母卡牌，由外部設置
    public abstract void OnApply(IdolOnStage target, OnStageManager onStageManager);
    public abstract void OnEnd(IdolOnStage target, OnStageManager onStageManager);
}

