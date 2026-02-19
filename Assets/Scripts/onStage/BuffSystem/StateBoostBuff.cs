using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Stage/Buffs/StatBoost")]
public class StateBoostBuff : BuffData
{
    public enum StatType { Vocal, Dance, Visual }
    public StatType statToBoost;
    public int boostAmount=50;
    public override void OnApply(IdolOnStage target) {
        if (statToBoost == StatType.Vocal)
        {
            target.StageVocal += boostAmount;
        }
        else if (statToBoost == StatType.Dance)
        {
            target.StageDance += boostAmount;
        }
        else if (statToBoost == StatType.Visual)
        {
            target.StageVisual += boostAmount;
        }
    }
    public override void OnTick() { }
    public override void OnEnd(IdolOnStage target) {
        if (statToBoost == StatType.Vocal)
        {
            target.StageVocal -= boostAmount;
        }
        else if (statToBoost == StatType.Dance)
        {
            target.StageDance -= boostAmount;
        }
        else if (statToBoost == StatType.Visual)
        {
            target.StageVisual -= boostAmount;
        }
    }
}
