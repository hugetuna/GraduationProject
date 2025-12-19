using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void Initialize(Activity newActivity, TicketColor color)
    {
        activity = newActivity;
        ticketColor = color;

        // 設定 UI 顯示
        activityNameText.text = activity.activityName;
        activityInfoText.text = activity.description;
        activityFeeText.text = $"${activity.fee}";
        activityDateText.text = activity.date; // 已在 Activity 中設定好格式

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
