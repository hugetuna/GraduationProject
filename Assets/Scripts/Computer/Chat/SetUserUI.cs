using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/* 掛在用戶項目 UI 的 prefab 根部上（不看 Wrapper）*/
public class SetUserUI : MonoBehaviour
{
    [Header("用戶項目的 UI 設定")]
    private User user;
    private UserRuntime userRuntime;
    [SerializeField] private Image userImage; // 用戶圖示
    [SerializeField] private RectTransform userNameContainer; // 用戶名稱容器（用於強制更新位置）
    [SerializeField] private TextMeshProUGUI userNameText; // 用戶名稱文字
    [SerializeField] private Image statusImage; // 未讀訊息之狀態圖示
    [SerializeField] private TextMeshProUGUI lastMessageText; // 最新未讀訊息文字
    [SerializeField] private TextMeshProUGUI lastTimeText; // 最新未讀訊息時間
    //-----------------------------------------------------------------//
    public static event Action<UserRuntime> OnUserContentClicked; // 用戶項目被點擊的事件
    public static event Action<UserRuntime> OnUserStatusChanged; // 用戶對話狀態改變的事件
    public static void InvokeUserStatusChanged(UserRuntime ur)
    {
        OnUserStatusChanged?.Invoke(ur);
    }

    void OnEnable()
    {
        // 每次物件被啟用時（例如切換分頁顯示），重新訂閱
        OnUserStatusChanged += UpdateUnreadStatus;

        // 一顯示出來就立刻強制刷新一次，避免漏掉隱藏期間的更新
        if (userRuntime != null)
        {
            UpdateUnreadStatus(userRuntime);
        }
    }

    void OnDisable()
    {
        // 物件隱藏時取消訂閱，避免記憶體洩漏
        OnUserStatusChanged -= UpdateUnreadStatus;
    }

    public void Initialize(UserRuntime userRuntime)
    {
        // 設定用戶項目 UI
        this.userRuntime = userRuntime;
        user = userRuntime.user;

        userImage.sprite = user.userIcon;
        userNameText.text = user.userName;
        userNameText.ForceMeshUpdate();

        // 未讀訊息的初始狀態
        UpdateUnreadStatus(userRuntime);

        // 註冊按鈕點擊事件
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnUserContentClicked?.Invoke(userRuntime);
        });

    }

    public void UpdateUnreadStatus(UserRuntime ur)
    {
        if (ur == null || ur != userRuntime) return; // 只更新相應 UI 的狀態

        statusImage.gameObject.SetActive(ur.isUnread);
        lastMessageText.text = ur.lastMessageText;

        int totalDays = DayManager.Instance.date + DayManager.Instance.chapter * 3; // 僅適用於新手教學＆第一章
        int dayDiff = totalDays - ur.lastMessageDay; // 上次讀訊息與現在的天數差距
        if (dayDiff > 0)
        {
            // 顯示 1 day, 2 days...
            lastTimeText.text = $"{dayDiff} day" + (dayDiff > 1 ? "s" : "");
        }
        else
        {
            // 同一天則顯示 today
            lastTimeText.text = "today";
        }

        // 確保 UI 位置正確
        Canvas.ForceUpdateCanvases();
        if (userNameContainer != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(userNameContainer);
        }
    }
}
