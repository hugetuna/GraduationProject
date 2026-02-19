using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffData : ScriptableObject
{
    public string buffName;
    public string description;
    public Sprite icon;
    public float duration;
    public bool isDebuff;

    public abstract void OnApply(IdolOnStage target);
    public abstract void OnTick();
    public abstract void OnEnd(IdolOnStage target);
}
