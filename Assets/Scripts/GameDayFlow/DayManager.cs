using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    public int chapter = 0; //保存遊戲中的章節
    public int date = 0; //保存遊戲中的日期
    public DayEventManager dayEventManager;
    public bool IsInStartOfDay = true;//是否處於新一天開始的階段
    [Header("為了EventManager")]
    public System.Action onDayFinish = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }
    public void OnGameFileLoad()
    {
        date=GameManager.Instance.DayData.day;
        IsInStartOfDay= GameManager.Instance.DayData.IsInStartOfDay;
        if (IsInStartOfDay)
        {
            StartDay();
        }
        else
        {
            dayEventManager.InitializeDayEvents(date, GameManager.Instance.DayData.currentEventIndex);
            dayEventManager.TriggerNextEvent();
        }
    }
    public void OnSceneLoaded(string SceneName)
    {
        if (IsInStartOfDay==true&&SceneName=="Floor_1")
        {
            StartDay();
        }
        else if (SceneName == "Floor_4"&&date == 0)
        {
            StartDay();
        }
        if (SceneName == "Floor_4"|| SceneName == "Floor_3" || SceneName == "Floor_2" || SceneName == "Floor_1" || SceneName == "Floor_B1")
        {
            dayEventManager.ShowEventHint(dayEventManager.currentEvent);
        }
    }
    // 用來更動日期的函式
    public void StartDay()
    {
        IsInStartOfDay = false;
        date++;
        dayEventManager.InitializeDayEvents(date,0);
        dayEventManager.TriggerNextEvent();
        
        TeamManager teamManager = FindAnyObjectByType<TeamManager>();
        teamManager.ResetIdolsTeam();
    }
    public void AfterDayEndEventStart()
    {
        Debug.Log("觸發結束一天後的事件");

        var temp = onDayFinish;   // 保護現有事件
        onDayFinish = null;       // 立刻清空，防止重複觸發

        temp?.Invoke();
    }
    public void EndDay()
    {
        foreach (var idol in TeamDataUtility.IdolInstanceList)
        {
            idol.ResetTemporaryEffect();
        }
        // 重置事件狀態
        Debug.Log($"結束一天 Date:{date}");
        onDayFinish = null;
        IsInStartOfDay = true;
        SceneTransitionManager.Instance.teleportByTargetSceneName("Floor_1");
    }  
}
