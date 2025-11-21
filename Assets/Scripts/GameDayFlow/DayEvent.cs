using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ApplyAtWhere
{
    MainWorld,
    Dialogue,
    Everywhere
}
[CreateAssetMenu(menuName = "Game/Day Event")]
public class DayEvent : ScriptableObject
{
    public string eventId;// 可用來查找避免重複

    public int TriggerDay;//觸發的遊戲天數
    public ApplyAtWhere applyAtWhere;//觸發地點(需不需要換場)
    public DialogueSaveData DialogueWhenTrigger;//觸發的對話劇本
    public bool onlyTriggerOnce = true;// 教學 / 劇情只跑一次

    [TextArea]
    public string description;//自由敘述
}
