using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainUI;
    void Start()
    {
        ComputerInteraction.OnComputerInteracted += HideMainUI; // 訂閱並監聽與電腦互動事件
        DesktopUIHandler.OnDesktopUIClosed += ShowMainUI; // 訂閱並監聽桌面 UI 關閉事件
    }

    void OnDestroy()
    {
        ComputerInteraction.OnComputerInteracted -= HideMainUI; // 取消訂閱與電腦互動事件
        DesktopUIHandler.OnDesktopUIClosed -= ShowMainUI; // 取消訂閱桌面 UI 關閉事件
    }

    private void HideMainUI()
    {
        mainUI.SetActive(false);
    }

    private void ShowMainUI()
    {
        mainUI.SetActive(true);
    }
}
