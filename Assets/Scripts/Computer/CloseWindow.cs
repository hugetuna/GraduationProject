using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWindow : MonoBehaviour
{
    public SetAppUI setAppUI; // 對應 App 的 UI 控制腳本 
    public GameObject windowToClose; // App 視窗

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(CloseTheWindow); // 設置按鈕點擊事件
    }

    public void CloseTheWindow()
    {
        // 可使用 UI 上的叉叉關閉 UI
        Debug.Log("關閉視窗 UI");
        windowToClose.SetActive(false);

        GameObject miniInstance = setAppUI.GetMiniInstance();
        if (miniInstance != null) Destroy(miniInstance); // 銷毀最小化按鈕實例

        WindowManager windowManager = setAppUI.windowmanager;
        windowManager.DeregisterWindow();
    }
}
