using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 預計掛在 App UI Prefab 根部 */
public class SetAppUI : MonoBehaviour
{
    public AppData appData; // 這個 App 的資料
    private Button appButton; // 這個 App 本身（按鈕）
    private Image appIcon; // App 圖示
    private TextMeshProUGUI appName; // App 名稱
    //-----------------------------------------------------------------//
    public GameObject appWindow; // 該 App 對應的視窗（預計會先在場景中建好，而非動態生成）
    public WindowManager windowmanager;
    //-----------------------------------------------------------------//
    public GameObject miniPrefab; // 位於工具列的最小化視窗圖示（預計使用動態生成的方式）
    public GameObject theMinimized; // 最小化視窗圖示們的父元件
    private GameObject miniInstance = null; // 最小化視窗圖示之實例

    void Start()
    {
        // 設定 App 的資料
        appIcon = transform.Find("Icon").GetComponentInChildren<Image>();
        appName = GetComponentInChildren<TextMeshProUGUI>();
        appIcon.sprite = appData.appIcon;
        appName.text = appData.appName;

        appButton = GetComponent<Button>();
        appButton.onClick.AddListener(OpenTheWindow);
    }

    void Update()
    {
        if (appWindow.activeSelf)
        {
            appButton.interactable = false; // 避免重複點擊按鈕（關閉視窗時恢復）
        }
        else
        {
            appButton.interactable = true; // 確保按鈕可互動
        }
    }

    public void OpenTheWindow()
    {
        Debug.Log($"點擊了 {appData.appName} App");

        appWindow.SetActive(true); // 開啟視窗
        windowmanager.RegisterWindow(appWindow.GetComponent<RectTransform>()); // 設定視窗位置與排序

        miniInstance = Instantiate(miniPrefab, theMinimized.transform); // 生成最小化按鈕
        Image icon = miniInstance.transform.Find("Icon").GetComponent<Image>();
        icon.sprite = appData.appIcon; // 設定最小化按鈕的圖示
        MinimizeController minimizeController = miniInstance.GetComponent<MinimizeController>();
        minimizeController.appWindow = appWindow; // 設定最小化按鈕所對應的視窗
    }

    public GameObject GetMiniInstance()
    {
        return miniInstance; // 返回最小化按鈕實例
    }
}
