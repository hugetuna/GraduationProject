using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemEffects/TrainingBoost")]
public class TrainingBoostEffect : ItemEffect {
    //�T�ؽm�߮Ĳv�[��
    public float voTrainingBoost=1;
    public float daTrainingBoost=1;
    public float viTrainingBoost=1;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        target.voTrainingBonus *= voTrainingBoost;
        target.daTrainingBonus *= daTrainingBoost;
        target.viTrainingBonus *= viTrainingBoost;
        // ���]�A���]�p training �Ҧ��|�h�ϥγo�ӥ[��
        Debug.Log($"[�q�۰V�m�[��] �]�w vocal bonus ���H {voTrainingBoost}");
        Debug.Log($"[�R�аV�m�[��] �]�w dance bonus ���H {daTrainingBoost}");
        Debug.Log($"[�q�۰V�m�[��] �]�w visual bonus ���H {viTrainingBoost}");
    }
}
