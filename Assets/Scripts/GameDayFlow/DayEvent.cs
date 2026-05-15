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
    WaitUntilInteractWithObject,
    WaitForSeconds,
    WaitUntilSpecificIdolTrained,
    WaitUntilSpecificButtonPressed,
    WaitComputerOpen,
    WaitAfterDayEndEventStart,
    WaitTutorialEnd,
    EndDay
}
[CreateAssetMenu(menuName = "DayFlow/Day Event")]
public class DayEvent : ScriptableObject
{
    public string eventId;// 可用來查找避免重複
    public IdolWho TriggerPeople;//某人在隊伍中時才會觸發
    public List<IdolTeamIndex> TriggerTeam;//某些隊伍時才會觸發
    //觸發的時間點序號
    //0~99: 白天事件
    //100:  WaitAfterDayEndEventStart事件
    //101~200: 一天結束事件
    //999: EndDay事件
    public int TriggerTimeIndex;

    public EventType Type;//觸發事件類型
    public DialogueSaveData DialogueWhenTrigger;//觸發的對話劇本
    public bool onlyTriggerOnce = true;// 教學 / 劇情只跑一次
    [Header("特定事件需要的參數")]
    public GameObject UIToShow;//若ShowUIAndWaitExit->顯示的UI物件
    public string targetSceneName;//若WaitUntilSceneChange->等待傳送至目標場景名稱
                                  //若Teleport->傳送至目標場景名稱
    public Vector3 targetPlayerPositionMin;//若WaitUntilPlayerPosition->等待玩家移動至目標區間
    public Vector3 targetPlayerPositionMax;
    public string interactableObjectKey;//若WaitUntilInteractWithObject->等待與目標互動物件互動
    public float waitSeconds;//若WaitForSeconds->等待秒數
    public IdolWho targetIdol;//若WaitUntilSpecificIdolTrained->等待特定偶像培育完成
    public string targetButtonName;//若WaitUntilSpecificButtonPressed->等待特定按鈕被按下
    [Header("提示系統")]
    public bool isHintEvent = false;//是否需要提示
    public string hint;//提示內容
    [TextArea]
    public string description;//自由敘述
}
