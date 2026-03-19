using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct ActionCardStack
{
    public ActionCard actionCard;
    public int quantity;

    public ActionCardStack(ActionCard newCard, int quantity)
    {
        this.actionCard = newCard;
        this.quantity = quantity;
    }
}
[System.Serializable]
public struct FansBonusSet
{
    public int fansRequire;//達成這個粉絲獎勵需要的粉絲數
    public int BonusPoint;//達成這個粉絲獎勵可以獲得的分數
    public FansBonusSet(int fansRequire, int BonusPoint)
    {
        this.fansRequire = fansRequire;
        this.BonusPoint = BonusPoint;
    }
}
[System.Serializable]
public struct RewardSet
{
    public int rewardSetPoint;//達成這個獎勵組合需要的分數
    public int rewardFans;
    public int rewardMoney;
    public List<ItemStack> rewardItems;
    public RewardSet(int rewardSetPoint, int rewardFans, int rewardMoney, List<ItemStack> rewardItems)
    {
        this.rewardSetPoint = rewardSetPoint;
        this.rewardFans = rewardFans;
        this.rewardMoney = rewardMoney;
        this.rewardItems = rewardItems;
    }
}
[CreateAssetMenu(fileName = "New Stage", menuName = "Stage/Stage Attribute")]
public class StageAttribute : ScriptableObject
{
    public int stageID;
    public string stageName;

    public Sprite backgroundImage;
    public AudioClip backgroundMusic;
    public string musicName;
    public List<ActionCardStack> actionCardStacks;
    [Header("計數相關")]
    public int roundMax;//總回合數
    public float secPerRound;//一回合的時間
    public List<FansBonusSet> fansBonusSets;//粉絲獎勵組合，達成特定粉絲數可以獲得對應的分數(預設高到低)
    [Header("結束後")]
    public DialogueSaveData clearDialogue;
    public string nextSceneName;
    public List<RewardSet> rewardSets;//獎勵組合，達成特定分數可以獲得對應的獎勵(預設高到低)
    [TextArea(2, 5)]
    public string description;
}
