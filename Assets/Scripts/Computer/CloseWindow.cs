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
    public AudioClip closeAppSound; // 關閉視窗的音效
    public static event Action OnAppWithTutorialClosed;

    void Start()
    {
        closeButton.onClick.AddListener(CloseTheWindow); // 設置按鈕點擊事件
    }

    public void CloseTheWindow()
    {
        // 可使用 UI 上的叉叉關閉 UI
        Debug.Log("關閉視窗 UI");
        gameObject.SetActive(false);
        if (closeAppSound != null) AudioManager.Instance.PlaySFX(closeAppSound, 0.5f); // 播放音效

        GameObject miniInstance = setAppUI.GetMiniInstance();
        if (miniInstance != null) Destroy(miniInstance); // 銷毀最小化按鈕實例

        RectTransform windowRect = GetComponent<RectTransform>();
        WindowManager.Instance.DeregisterWindow(windowRect);

        // 處理新手教學
        if(DayManager.Instance.chapter == 0) OnAppWithTutorialClosed?.Invoke();
    }

    public void BindSetAppUI(SetAppUI setAppUI)
    {
        this.setAppUI = setAppUI;
    }
}
