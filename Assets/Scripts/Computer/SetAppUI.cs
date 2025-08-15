using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/* 預計掛在 AppButton 的 Prefab 根部 */
public class SetAppUI : MonoBehaviour
{
    [Header("應用程式設定")]
    [Tooltip("選擇應用程式的種類")] public AppData appData; // 這個 App 的資料
    private Button appButton; // 這個 App 本身（按鈕）
    private Image appIcon; // App 圖示
    private TextMeshProUGUI appName; // App 名稱
    //-----------------------------------------------------------------//
    [Header("視窗設定")]
    private GameObject appWindow; // 該 App 對應的視窗
    [Tooltip("視窗們的父物件")] public RectTransform windowContainer;
    [Tooltip("用來控制視窗開關")] public WindowManager windowManager;
    //-----------------------------------------------------------------//
    [Header("最小化按鈕設定")]
    [Tooltip("位於工具列的最小化視窗（動態生成）")] public GameObject miniPrefab;
    [Tooltip("最小化按鈕們的父元件")] public GameObject theMinimized;
    private GameObject miniInstance = null; // 最小化視窗圖示之實例
    //-----------------------------------------------------------------//
    [Header("應用程式雙擊效果")]
    [SerializeField] private float doubleClickThreshold = 0.3f; // 該按鈕的允許雙擊間隔（秒）
    private float lastClickTime = -1f;

    void Start()
    {
        // 設定 App 的資料
        appIcon = transform.Find("Icon").GetComponentInChildren<Image>();
        appName = GetComponentInChildren<TextMeshProUGUI>();
        appIcon.sprite = appData.appIcon;
        appName.text = appData.appName;

        // 將對應視窗放入場景
        appWindow = Instantiate(appData.appWindowPrefab, windowContainer);
        CloseWindow closeWindow = appWindow.GetComponentInChildren<CloseWindow>();
        closeWindow.BindSetAppUI(this); // 綁定 SetAppUI
        appWindow.SetActive(false); // 初始隱藏視窗

        appButton = GetComponent<Button>();
        appButton.onClick.AddListener(OpenTheWindow);
        // appData.SetAppPrefab(appWindow); // 綁定資料
    }

    void Update()
    {
        if (appWindow.activeSelf)
        {
            appButton.interactable = false; // 避免重複點擊按鈕
            // 若滑鼠點擊了視窗，則將該視窗置頂
            if (Input.GetMouseButtonDown(0))
            {
                RectTransform winRect = appWindow.GetComponent<RectTransform>();
                if (IsPointerOverUIObject(winRect))
                {
                    windowManager.BringToFront(winRect);
                }
            }
        }
        else
        {
            appButton.interactable = true; // 確保按鈕可互動
        }
    }

    public void OpenTheWindow()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            Debug.Log($"雙擊開啟 {appData.appName} 視窗");

            appWindow.SetActive(true); // 開啟視窗
            windowManager.RegisterWindow(appWindow.GetComponent<RectTransform>()); // 設定視窗位置與排序

            miniInstance = Instantiate(miniPrefab, theMinimized.transform); // 生成最小化按鈕
            Image icon = miniInstance.transform.Find("Icon").GetComponent<Image>();
            icon.sprite = appData.appIcon; // 設定最小化按鈕的圖示
            MinimizeController minimizeController = miniInstance.GetComponent<MinimizeController>();
            minimizeController.SetAppWindow(appWindow); // 設定最小化按鈕所對應的視窗
        }

        lastClickTime = Time.time;
    }

    public GameObject GetMiniInstance()
    {
        return miniInstance; // 返回最小化按鈕實例
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
