using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Parsed;

/* 放在預約視窗根部，處理活動票券生成以外的 UI 設定 */
public class SetAppointUI : MonoBehaviour
{
    [Header("行事曆日期")]
    public List<TextMeshProUGUI> dateTexts = new(); // 用於顯示行事曆日期的文字（照順序放）
    private int initialMonth = 9;
    private int initialDay = 22;
    //-----------------------------------------------------------------//
    [Header("公演顯示")]
    public List<Activity> stageList = new(); // 公演日期資料
    public GameObject stagePrefab; // 用於生成公演顯示的預製件
    public List<GameObject> DayContents; // 行事曆容器（照日期順序）
    
    void Start()
    {
        // 設定行事曆日期
        for (int i = 0; i < dateTexts.Count; i++)
        {
            int month = initialMonth;
            int day = initialDay + DayManager.Instance.day + i - 1;
            if(day > 30){ // 簡單處理一下跨月的情況（目前只有九月和十月）
                month++;
                day -= 30; 
            }

            if(month == 9) dateTexts[i].text = $"SEP {day:D2}";
            else if(month == 10) dateTexts[i].text = $"OCT {day:D2}";
        }

        // 生成公演顯示
        foreach (Activity stage in stageList)
        {
            int daydiff = Math.Abs(stage.day - DayManager.Instance.day);
            if (daydiff < 0 || daydiff > 5)
            {
                Debug.Log($"{stage.activityName} 不在顯示範圍內，跳過生成");
                continue;
            }

            GameObject stageObject = Instantiate(stagePrefab, DayContents[daydiff].transform);
            if (stageObject == null)
            {
                Debug.Log("公演顯示生成失敗！");
                continue;
            }

            // 設定公演顯示的 UI 資料
            TextMeshProUGUI stageNumText = stageObject.transform.Find("NumText").GetComponent<TextMeshProUGUI>();
            stageNumText.text = stage.activityName;
        }
        
    }
}
