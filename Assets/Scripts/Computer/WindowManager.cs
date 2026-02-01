using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/* 掛在 UIManager 上（Singleton） */
public class WindowManager : MonoBehaviour
{
    public static WindowManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [Header("視窗設定")]
    public RectTransform windowContainer; // 視窗們的父物件
    private List<RectTransform> registeredWinRect = new();
    //-----------------------------------------------------------------//
    [Header("開窗位置")] // 目前還不用考慮超出桌面邊界的問題
    public Vector2 startPos = new Vector2(-60, 60); // 第一個視窗位置
    public Vector2 offset = new Vector2(30, -20); // 每個新視窗的偏移量
    private static int windowCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    public void RegisterWindow(RectTransform rect) // 設定新視窗位置＆排序
    {   
        // 計算 cascade 位置
        Vector2 pos = startPos + offset * windowCount;

        // 設定位置
        rect.SetParent(windowContainer, false);
        rect.anchoredPosition = pos;

        // 移到最上層
        BringToFront(rect);

        // 自動掛上「偵測點擊置前」的 WindowFocusTrigger 腳本
        if (!rect.gameObject.GetComponent<WindowFocusTrigger>())
        {
            rect.gameObject.AddComponent<WindowFocusTrigger>().Initialize(rect);
        }

        // 更新視窗數量
        windowCount++;
        registeredWinRect.Add(rect);
    }

    public void DeregisterWindow(RectTransform rect) // 用於關閉視窗時的調整
    {
        // 更新視窗數量
        if (windowCount > 0)
        {
            windowCount--;
            registeredWinRect.Remove(rect);
        }
    }

    public bool IsWindowRegistered(RectTransform rect) // 檢查視窗是否已註冊
    {
        return registeredWinRect.Contains(rect);
    }

    // public void CloseAllWindows() // 關閉所有視窗的實作與調整
    // {
    //     foreach (RectTransform window in registeredWinRect)
    //     {
    //         window.gameObject.SetActive(false);
    //     }
    //     registeredWinRect.Clear();
    //     windowCount = 0;
    // }

    public void BringToFront(RectTransform window) // 讓視窗置頂
    {
        window.SetAsLastSibling();
    }
}

public class WindowFocusTrigger : MonoBehaviour, IPointerDownHandler
{
    private RectTransform myRect;

    public void Initialize(RectTransform rect) => myRect = rect;

    public void OnPointerDown(PointerEventData eventData) // 檢查視窗是否被點擊
    {
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult r in results)
        {
            if (r.gameObject.name == "Close") return; // 點到叉叉按鈕就不置頂
        }

        // 通知 WindowManager 將該視窗置頂
        WindowManager.Instance.BringToFront(myRect);
    }
}