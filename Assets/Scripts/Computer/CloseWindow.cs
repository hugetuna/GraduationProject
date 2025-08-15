using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 本來放在關閉按鈕上，目前改成放在視窗 prefab 根部 */
public class CloseWindow : MonoBehaviour
{
    private SetAppUI setAppUI; // 對應 App 的 UI 控制腳本 
    public Button closeButton;
    // public GameObject windowToClose; // App 視窗

    void Start()
    {
        closeButton.onClick.AddListener(CloseTheWindow); // 設置按鈕點擊事件
    }

    public void CloseTheWindow()
    {
        // 可使用 UI 上的叉叉關閉 UI
        Debug.Log("關閉視窗 UI");
        gameObject.SetActive(false);

        GameObject miniInstance = setAppUI.GetMiniInstance();
        if (miniInstance != null) Destroy(miniInstance); // 銷毀最小化按鈕實例

        WindowManager windowManager = setAppUI.windowManager;
        windowManager.DeregisterWindow();
    }

    public void BindSetAppUI(SetAppUI setAppUI)
    {
        this.setAppUI = setAppUI;
    }
}
