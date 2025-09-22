using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProfile", menuName = "Dialogue/CharacterProfile")]
public class CharacterDialogueProfile : ScriptableObject
{
    public string characterTag;
    public string characterName;
    public Sprite defaultPortrait;
    public List<EmotionSprite> emotions;
}
[System.Serializable]
public class EmotionSprite
{
    public string emotion;
    public Sprite portrait;
}
