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

[CreateAssetMenu(fileName = "New Stage", menuName = "Stage/Stage Attribute")]
public class StageAttribute : ScriptableObject
{
    public int stageID;
    public string stageName;

    public Sprite backgroundImage;
    public AudioClip backgroundMusic;
    public List<ActionCardStack> actionCardStacks;

    public int baseRewardFans;

    public int requiredScoreToPass;

    [TextArea(2, 5)]
    public string description;
}
