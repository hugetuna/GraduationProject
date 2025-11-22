using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    MainWorld,
    Dialogue,
    Teleport,
    ShowUIAndWaitExit,
    WaitUntilSceneChange,
    WaitUntilPlayerPosition,
    WaitUntilInteractWithObject
}
[CreateAssetMenu(menuName = "DayFlow/Day Event")]
public class DayEvent : ScriptableObject
{
    public string eventId;// 可用來查找避免重複
    public int TriggerDay;//觸發的遊戲天數
    public EventType Type;//觸發地點(需不需要換場)
    public DialogueSaveData DialogueWhenTrigger;//觸發的對話劇本
    public bool onlyTriggerOnce = true;// 教學 / 劇情只跑一次
    [Header("特定事件需要的參數")]
    public GameObject UIToShow;//若ShowUIAndWaitExit->顯示的UI物件
    public string targetSceneName;//若WaitUntilSceneChange->等待傳送至目標場景名稱
                                  //若Teleport->傳送至目標場景名稱
    public Vector3 targetPlayerPositionMin;//若WaitUntilPlayerPosition->等待玩家移動至目標區間
    public Vector3 targetPlayerPositionMax;
    public string interactableObjectKey;//若WaitUntilInteractWithObject->等待與目標互動物件互動
    [TextArea]
    public string description;//自由敘述
}
