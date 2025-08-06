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
    public Image appIcon; // App 圖示
    public TextMeshProUGUI appName; // App 名稱
    public Button appButton; // 這個 App 本身（按鈕）
    //-----------------------------------------------------------------//
    public static event Action<GameObject, AppData> OnAppClicked; // 定義點擊圖示跳出視窗的事件（參數為圖示物件與資料）
    public GameObject miniInstance = null; // 綁定該 App 的最小化實例

    void Start()
    {
        // 設定 App 的資料
        appIcon.sprite = appData.appIcon;
        appName.text = appData.appName;

        appButton = GetComponent<Button>(); // 獲取自己底下的按鈕組件
        if (appButton != null)
        {
            appButton.onClick.AddListener(() =>
            { // 點擊 App 圖示時觸發事件
                Debug.Log($"點擊了 {appData.appName} App");
                OnAppClicked?.Invoke(gameObject, appData);
            });
        }
    }

    // void Update()
    // {

    // }
}
