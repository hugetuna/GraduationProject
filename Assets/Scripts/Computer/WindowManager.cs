using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowManager : MonoBehaviour
{
    [Header("視窗設定")]
    public RectTransform windowContainer; // 視窗們的父物件
    private List<RectTransform> registeredWinRect = new();
    //-----------------------------------------------------------------//
    [Header("開窗位置")] // 目前還不用考慮超出桌面邊界的問題
    public Vector2 startPos = new Vector2(-60, 60); // 第一個視窗位置
    public Vector2 offset = new Vector2(30, -20); // 每個新視窗的偏移量
    //-----------------------------------------------------------------//
    [Header("傳給視窗的資料")]
    public ResourceManager resourceManager;
    //-----------------------------------------------------------------//
    private static int windowCount = 0;

    void Start()
    {
        // 預先設定好，讓視窗 prefab 之後能自行取用
        WindowDataSetup.SetResourceManager(resourceManager); 
    }

    void Update()
    {
        // 若滑鼠點擊了視窗，則將該視窗置頂
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            // 如果滑鼠點到了叉叉按鈕就直接 return
            foreach (RaycastResult r in results)
            {
                if (r.gameObject.name == "Close") return;
            }

            // 視窗的置前效果
            RectTransform topWindow = null;
            int topIndex = -1;

            foreach (RectTransform window in registeredWinRect)
            {
                if (IsPointerOverUIObject(window))
                {
                    int index = window.GetSiblingIndex();
                    if (index > topIndex)
                    {
                        topIndex = index;
                        topWindow = window;
                    }
                }
            }

            if (topWindow != null)
            {
                BringToFront(topWindow);
            }
        }
    }

    public void RegisterWindow(RectTransform rect) // 設定新視窗位置＆排序
    {
        // 計算 cascade 位置
        Vector2 pos = startPos + offset * windowCount;

        // 設定位置
        rect.SetParent(windowContainer, false);
        rect.anchoredPosition = pos;

        // 移到最上層
        rect.SetAsLastSibling();

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

    private bool IsPointerOverUIObject(RectTransform uiElement) // 檢查特定 UI 元件是否被滑鼠點擊
    {
        PointerEventData pointerData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform == uiElement ||
                result.gameObject.transform.IsChildOf(uiElement))
            {
                return true;
            }
        }
        return false;
    }
}
