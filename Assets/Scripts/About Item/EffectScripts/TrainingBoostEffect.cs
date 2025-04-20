using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/TrainingBoost")]
public class TrainingBoostEffect : ItemEffect {
    //贺絤策瞯Θ
    public float voTrainingBoost=1;
    public float daTrainingBoost=1;
    public float viTrainingBoost=1;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        target.voTrainingBonus *= voTrainingBoost;
        target.daTrainingBonus *= daTrainingBoost;
        target.viTrainingBonus *= viTrainingBoost;
        // 安砞Τ砞璸 training 家Α穦ㄏノ硂Θ
        Debug.Log($"[簈佰癡絤Θ] 砞﹚ vocal bonus  {voTrainingBoost}");
        Debug.Log($"[籖列癡絤Θ] 砞﹚ dance bonus  {daTrainingBoost}");
        Debug.Log($"[簈佰癡絤Θ] 砞﹚ visual bonus  {viTrainingBoost}");
    }
}
