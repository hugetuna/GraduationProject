using UnityEngine;

public abstract class CardEffectBase : ScriptableObject
{
    public abstract void OnApply(IdolOnStage target, OnStageManager onStageManager);
    public abstract void OnEnd(IdolOnStage target, OnStageManager onStageManager);
}

