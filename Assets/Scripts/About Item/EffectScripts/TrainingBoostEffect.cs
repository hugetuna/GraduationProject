using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/TrainingBoost")]
public class TrainingBoostEffect : ItemEffect {
    [Tooltip("唯一的 ID，格式為\"種類_數值\"，例如\"dance_5\"")]
    public string boostEffectID;
    public string displayName; // 方便顯示給玩家看的名稱

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
        // 儲存這個訓練加成效果的 ID 和 displayName，方便在訓練模式中顯示給玩家
        var effectInfo = new ItemEffectInfo { id = boostEffectID, displayName = displayName };
        ItemEffectUtility.SaveTrainingEffect(target.idolIndex, effectInfo);
    }
}
