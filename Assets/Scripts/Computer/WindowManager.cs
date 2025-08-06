using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour
{
    public List<GameObject> windowList = new(); // 儲存各個視窗的 UI 物件（預計會先在場景中建好，而非動態生成）
    //-----------------------------------------------------------------//
    public GameObject miniPrefab; // 位於工具列的最小化視窗圖示（預計使用動態生成的方式）
    public GameObject theMinimized; // 最小化視窗圖示們的父元件

    void Start()
    {
        SetAppUI.OnAppClicked += OpenWindow; // 訂閱 App 點擊事件
        // CloseWindow.OnCloseButtonClicked += CloseMiniButton; // 訂閱視窗關閉事件
        foreach (GameObject window in windowList)
        {
            window.SetActive(false); // 隱藏所有視窗
        }
    }

    void OnDestroy()
    {
        SetAppUI.OnAppClicked -= OpenWindow; // 取消訂閱 App 點擊事件
        // CloseWindow.OnCloseButtonClicked -= CloseMiniButton; // 取消訂閱視窗關閉事件
    }

    void Update()
    {

    }

    private void OpenWindow(GameObject app, AppData appData)
    {
        // 根據 AppData 的 appType 開啟對應的視窗，並生成最小化按鈕
        GameObject newMiniButton = Instantiate(miniPrefab, theMinimized.transform);
        Image icon = newMiniButton.transform.Find("Icon").GetComponent<Image>(); // 設定最小化按鈕的圖示

        SetAppUI appUI = newMiniButton.GetComponent<SetAppUI>(); // 綁定最小化視窗與相應的 App
        Button button = appUI.appButton; // 取得 app 按鈕
        switch (appData.appType)
        {
            case AppTypes.Store:
                windowList[0].SetActive(true); // 開啟商店視窗
                button.interactable = false; // 避免重複點擊（記得關閉視窗時恢復）
                appUI.miniInstance = newMiniButton; // 綁定最小化實例
                icon.sprite = appData.appIcon; // 設定商店的最小化圖示
                break;
                // case AppTypes.Chat:
                //     windowList[1].SetActive(true);
                //     miniInstances[1] = newMiniButton;
                //     icon.sprite = appData.appIcon;
                //     break;
                // case AppTypes.Appointment:
                //     windowList[2].SetActive(true);
                //     miniInstances[2] = newMiniButton;
                //     icon.sprite = appData.appIcon;
                //     break;
                // case AppTypes.Sell:
                //     windowList[3].SetActive(true);
                //     miniInstances[3] = newMiniButton;
                //     icon.sprite = appData.appIcon;
                //     break;
                // case AppTypes.GuideBook:
                //     windowList[4].SetActive(true);
                //     miniInstances[4] = newMiniButton;
                //     icon.sprite = appData.appIcon;
                //     break;
        }
    }

    // private void CloseMiniButton()
    // {
        
    // }
}
