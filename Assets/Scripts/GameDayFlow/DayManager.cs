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
    public List<StageAttribute> bossStages;
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
        //if (IsInStartOfDay)
        //{
        //    StartDay();
        //}
        //else
        //{
        //    dayEventManager.InitializeDayEvents(date, GameManager.Instance.DayData.currentEventIndex);
        //    dayEventManager.TriggerNextEvent();
        //}
    }
    public void OnSceneLoaded(string SceneName)
    {
        if (IsInStartOfDay==true&&SceneName=="Floor_1")
        {
            StartDay();
        }
        else if (SceneName == "Floor_3"&&date == 1&&chapter==0 && IsInStartOfDay == true)
        {
            StartDay();
            IdolInstance[] allIdols = FindObjectsByType<IdolInstance>(FindObjectsSortMode.None);
            //第一天，封鎖特定偶像的行動
            IdolInstance whoGoesToTeain = null;
            // 根據優先順序選擇偶像：Sirius > Aicor > Kuma
            foreach (var idol in allIdols)
            {
                if (idol.idolIndex == IdolWho.Sirius)
                {
                    whoGoesToTeain = idol;
                    break;
                }
                else if (idol.idolIndex == IdolWho.Aicor&&  whoGoesToTeain?.idolIndex!= IdolWho.Sirius)
                {
                    whoGoesToTeain = idol;
                }
                else if (idol.idolIndex == IdolWho.Kuma&& whoGoesToTeain?.idolIndex != IdolWho.Sirius&& whoGoesToTeain?.idolIndex != IdolWho.Aicor)
                {
                    whoGoesToTeain = idol;
                }
            }
            //封鎖隊裡的其他人
            foreach (var idol in allIdols)
            {
                if (idol != whoGoesToTeain)
                {
                    idol.trainRecord.RestrictTrainingOneDay();
                }
            }
            Debug.Log($"第一天，封鎖了{whoGoesToTeain.idolIndex}以外的練習");
        }
        if (SceneName == "Floor_3"|| SceneName == "Floor_3" || SceneName == "Floor_2" || SceneName == "Floor_1" || SceneName == "Floor_B1")
        {
            dayEventManager.ShowEventHint(dayEventManager.currentEvent);
        }
    }
    // 用來更動日期的函式
    public void StartDay()
    {
        IsInStartOfDay = false;
        if (date == 3&&chapter==0)
        {
            // 教學章最終天
            GameManager.Instance.onStageStage=bossStages[0];
        }
        else if (date == 13 && chapter == 1)
        {
            // 正式第一章章最終天
            GameManager.Instance.onStageStage = bossStages[1];
        }
        dayEventManager.InitializeDayEvents(chapter, date, 0);
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
        ItemEffectUtility.ResetAllEffects();
        // 重置事件狀態
        Debug.Log($"結束一天 Date:{date}");
        onDayFinish = null;
        IsInStartOfDay = true;
        //更新農場狀態
        GameManager.Instance.UpdateFarmAndAnimalAtDayEnd();
        // 重置預約狀態
        GameManager.Instance.teacherSaveData.CleanTeacherAppointments();
        // 清除過期的商演預約紀錄
        GameManager.Instance.activitySaveData.CleanGoneActivities(); 
        // 每天結束時大保存一次
        date++;
        if(chapter==0&&date==4)
        {
            chapter = 1;
            date = 1;
        }
        GameManager.Instance.SaveDayData();
        GameManager.Instance.SaveToFile();
        SceneTransitionManager.Instance.teleportByTargetSceneName("Floor_1");
    }  
}
