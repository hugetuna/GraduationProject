using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public enum TicketColor { Blue = 0, Green = 1, LightBlue = 2 }

/* 掛在預約視窗的活動票券 prefab 根部（不看 Wrapper）*/
public class SetTicketUI : MonoBehaviour
{
    [Header("活動票券的 UI 設定")]
    private Activity activity;
    private TicketColor ticketColor;
    [SerializeField] private TextMeshProUGUI activityNameText; // 活動名稱文字
    [SerializeField] private TextMeshProUGUI activityInfoText; // 活動描述文字
    [SerializeField] private TextMeshProUGUI activityFeeText; // 活動價格文字
    [SerializeField] private TextMeshProUGUI activityDateText; // 活動日期文字
    private int year = 2025;
    private int initialMonth = 9;
    private int initialDay = 22;

    public void Initialize(Activity newActivity, TicketColor color)
    {
        activity = newActivity;
        ticketColor = color;

        // 設定 UI 顯示
        activityNameText.text = activity.activityName;
        activityInfoText.text = activity.description;
        activityFeeText.text = $"${activity.fee}";
        
        int dayDiff = Math.Abs(activity.day - DayManager.Instance.day);
        int month = initialMonth;
        int day = initialDay + dayDiff;
        if (day > 30)  // 簡單處理一下跨月的情況（目前只有九月和十月）
        {
            month++;
            day -= 30;
        }

        activityDateText.text = $"{year}.{month:D2}.{day:D2}";

        // 確保字型正確渲染
        activityNameText.ForceMeshUpdate();
        activityInfoText.ForceMeshUpdate();
        activityFeeText.ForceMeshUpdate();
        activityDateText.ForceMeshUpdate();
    }

    public Activity GetActivity()
    {
        return activity;
    }

    public TicketColor GetTicketColor()
    {
        return ticketColor;
    }
}
