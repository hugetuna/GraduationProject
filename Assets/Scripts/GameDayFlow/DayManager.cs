using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance;
    public int date = 0; //保存遊戲中的日期
    public DayEventManager dayEventManager;
    public bool IsInStartOfDay = false;//是否處於新一天開始的階段
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }
    public void OnSceneLoaded(string SceneName)
    {
        if (IsInStartOfDay==false&&SceneName=="Floor_1")
        {
            StartDay();
        }
        else if (SceneName == "Floor_4"&&date == 0)
        {
            StartDay();
        }
    }
    // 用來更動日期的函式
    public void StartDay()
    {
        IsInStartOfDay = true;
        date++;
        dayEventManager.InitializeDayEvents(date);
        dayEventManager.TriggerNextEvent();
    }
    public void EndDay()
    {
        IsInStartOfDay = false;
    }  
}
