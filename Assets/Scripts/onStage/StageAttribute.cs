using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "Stage/Stage Attribute")]
public class StageAttribute : ScriptableObject
{
    public int stageID;
    public string stageName;

    public Sprite backgroundImage;
    public AudioClip backgroundMusic;

    public int baseRewardFans;

    public int requiredScoreToPass;

    [TextArea(2, 5)]
    public string description;
}
