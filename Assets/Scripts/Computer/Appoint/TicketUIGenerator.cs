using System;
using System.Collections.Generic;
using UnityEngine;

/* 放在預約視窗根部以生成（不同分類）ScrollView 中的活動 */
public class TicketUIGenerator : MonoBehaviour
{
    [Header("活動票券資訊")]
    public List<Activity> ticketList = new(); // 儲存活動資訊的清單
    //-----------------------------------------------------------------//
    public List<GameObject> ticketPrefab = new(); // 用於生成活動項目的預製件（先從三種樣式中隨便選）
    public List<Transform> ticketContent = new(); // 用於放置生成的活動票券的容器

    void Start()
    {
        // 一公前的所有活動清單
        foreach (Activity activity in ticketList) // 按清單生成初始的活動項目
        {
            int diff = Math.Abs(activity.day - DayManager.Instance.date);
            if (diff < 0 || diff > 5)
            {
                Debug.Log($"{activity.activityName} 不在可預約範圍內，跳過生成");
                continue;
            }

            // 生成活動票券（目前只有一個分類）
            int randomTicket = UnityEngine.Random.Range(0, ticketPrefab.Count); // 隨機生成票券樣式
            GameObject activityObject = Instantiate(ticketPrefab[randomTicket], ticketContent[0]);
            if (activityObject == null)
            {
                Debug.Log("活動票券生成失敗！");
                continue;
            }

            GameObject btn = activityObject.transform.Find("Button").gameObject; // Wrapper + "Button"
            // 設定活動票券的 UI 資料
            SetTicketUI setTicketUI = btn.GetComponent<SetTicketUI>();
            setTicketUI.Initialize(activity, (TicketColor)randomTicket);
        }

        GetComponent<TicketInfoUI>().Initialize(); // 初始化活動詳情 UI
    }
}
