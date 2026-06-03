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
    public List<Activity> specialList = new(); // 公演等劇情的日期資料
    public GameObject specialPrefab; // 用於生成公演等劇情顯示的預製件
    public List<GameObject> DayContents; // 行事曆容器（照日期順序）
    
    void Start()
    {
        int totalDays = DayManager.Instance.date + DayManager.Instance.chapter * 3; // 僅適用於新手教學＆第一章

        // 設定行事曆日期
        for (int i = 0; i < dateTexts.Count; i++)
        {
            int month = initialMonth;
            int day = initialDay + totalDays + i - 1;
            if(day > 30){ // 簡單處理一下跨月的情況（目前只有九月和十月）
                month++;
                day -= 30; 
            }

            if(month == 9) dateTexts[i].text = $"SEP {day:D2}";
            else if(month == 10) dateTexts[i].text = $"OCT {day:D2}";
        }

        // 生成公演等劇情顯示
        foreach (Activity special in specialList)
        {
            int daydiff = special.day - totalDays;
            if (daydiff < 0 || daydiff > 5)
            {
                Debug.Log($"{special.activityName} 不在顯示範圍內，跳過生成");
                continue;
            }

            GameObject specialObject = Instantiate(specialPrefab, DayContents[daydiff].transform);
            if (specialObject == null)
            {
                Debug.Log("公演等劇情顯示生成失敗！");
                continue;
            }

            // 設定公演等劇情顯示的 UI 資料
            TextMeshProUGUI specialNameText = specialObject.GetComponentInChildren<TextMeshProUGUI>();
            specialNameText.text = special.activityName;
        }
        
    }
}
