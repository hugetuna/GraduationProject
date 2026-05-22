using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 預計掛在 AppButton 的 Prefab 根部 */
public class SetAppUI : MonoBehaviour
{
    [Header("應用程式設定")]
    [Tooltip("選擇應用程式的種類")] public AppData appData; // 這個 App 的資料
    [SerializeField] private Button appButton; // 這個 App 本身（按鈕）
    [SerializeField] private Image appIcon; // App 圖示
    [SerializeField] private TextMeshProUGUI appName; // App 名稱
    [SerializeField] private TextMeshProUGUI appNameOutline; // App 名稱描邊
    //-----------------------------------------------------------------//
    [Header("視窗設定")]
    private GameObject appWindow; // 該 App 對應的視窗
    [Tooltip("視窗們的父物件")] public RectTransform windowContainer;
    private WindowManager windowManager;
    //-----------------------------------------------------------------//
    [Header("最小化按鈕設定")]
    [Tooltip("位於工具列的最小化視窗（動態生成）")] public GameObject miniPrefab;
    [Tooltip("最小化按鈕們的父元件")] public GameObject theMinimized;
    private GameObject miniInstance = null; // 最小化視窗圖示之實例
    //-----------------------------------------------------------------//
    [Header("應用程式雙擊效果")]
    [SerializeField] private float doubleClickThreshold = 0.3f; // 該按鈕的允許雙擊間隔（秒）
    private float lastClickTime = -1f;
    //-----------------------------------------------------------------//
    [Header("音效設定")]
    public AudioClip openAppSound; // 開啟 App 的音效
    //-----------------------------------------------------------------//
    public static event Action<string> OnAppWithTutorialOpened;

    void Start()
    {
        // 設定 App 的資料
        appIcon.sprite = appData.appIcon;
        appName.text = appNameOutline.text = appData.appName;

        // 將對應視窗放入場景
        appWindow = Instantiate(appData.appWindowPrefab, windowContainer);
        CloseWindow closeWindow = appWindow.GetComponentInChildren<CloseWindow>();
        closeWindow.BindSetAppUI(this); // 綁定 SetAppUI
        appWindow.SetActive(false); // 初始隱藏視窗

        appButton = GetComponent<Button>();
        appButton.onClick.AddListener(OpenTheWindow);

        windowManager = WindowManager.Instance;
    }

    public void OpenTheWindow()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickThreshold)
        {
            Debug.Log($"雙擊開啟 {appData.appName} 視窗");

            appWindow.SetActive(true); // 開啟視窗
            if (openAppSound != null) AudioManager.Instance.PlaySFX(openAppSound, 0.5f); // 播放音效
            var winRect = appWindow.GetComponent<RectTransform>();

            if (!windowManager.IsWindowRegistered(winRect))
            {
                windowManager.RegisterWindow(winRect); // 設定視窗位置與排序
                miniInstance = Instantiate(miniPrefab, theMinimized.transform); // 生成最小化按鈕
                Image icon = miniInstance.transform.Find("Icon").GetComponent<Image>();
                icon.sprite = appData.appIcon; // 設定最小化按鈕的圖示
                TextMeshProUGUI text = miniInstance.GetComponentInChildren<TextMeshProUGUI>();
                text.text = appData.appName; // 設定最小化按鈕的名稱

                MinimizeController minimizeController = miniInstance.GetComponent<MinimizeController>();
                minimizeController.SetAppWindow(appWindow); // 設定最小化按鈕所對應的視窗
            }
            else
            {
                // 若視窗已註冊，將其置前
                windowManager.BringToFront(winRect);
            }

            // 處理新手教學
            if (DayManager.Instance.totalDays == 1)
            {
                if (appData.appName == "圖鑑")
                {
                    OnAppWithTutorialOpened?.Invoke("粉絲可以換成金錢、粉絲數或道具（詳細玩法將於下一天介紹）");
                }
                else if (appData.appName == "預約")
                {
                    // 預約商演須支付參加費，當天可到一樓大門消耗體力參加並獲取報酬
                    OnAppWithTutorialOpened?.Invoke("商演預約後隔天便無法取消，當天可到一樓大門消耗體力參加並獲取報酬");
                }
            }

        }

        lastClickTime = Time.time;
    }

    public GameObject GetMiniInstance()
    {
        return miniInstance; // 返回最小化按鈕實例
    }
}
