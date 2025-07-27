using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/TrainingBoost")]
public class TrainingBoostEffect : ItemEffect {
    //三種練習效率加成
    public float voTrainingBoost=1;
    public float daTrainingBoost=1;
    public float viTrainingBoost=1;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        target.voTrainingBonus *= voTrainingBoost;
        target.daTrainingBonus *= daTrainingBoost;
        target.viTrainingBonus *= viTrainingBoost;
        // 假設你有設計 training 模式會去使用這個加成
        Debug.Log($"[歌唱訓練加成] 設定 vocal bonus 乘以 {voTrainingBoost}");
        Debug.Log($"[舞蹈訓練加成] 設定 dance bonus 乘以 {daTrainingBoost}");
        Debug.Log($"[表現力訓練加成] 設定 visual bonus 乘以 {viTrainingBoost}");
    }
}
