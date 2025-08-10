using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [Header("視窗設定")]
    public RectTransform windowContainer; // 視窗們的父物件
    //-----------------------------------------------------------------//
    [Header("開窗位置")] // 目前還不用考慮超出桌面邊界的問題
    public Vector2 startPos = new Vector2(-60, 60); // 第一個視窗位置
    public Vector2 offset = new Vector2(30, -20); // 每個新視窗的偏移量
    //-----------------------------------------------------------------//
    private static int windowCount = 0;

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
    }

    public void DeregisterWindow() // 用於關閉視窗時的調整
    {
        // 更新視窗數量
        if(windowCount > 0) windowCount--;
    }

    // public void BringToFront(RectTransform window) // 讓視窗置頂（目前看起來不會用到）
    // {
    //     window.SetAsLastSibling();
    // }
}
