using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWindow : MonoBehaviour
{
    public GameObject windowToClose;
    // public static event Action OnCloseButtonClicked;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(CloseTheWindow); // 設置按鈕點擊事件
    }

    public void CloseTheWindow()
    {
        // 可使用 UI 上的叉叉關閉 UI
        Debug.Log("關閉視窗 UI");
        windowToClose.SetActive(false);

        SetAppUI appUI = windowToClose.GetComponent<SetAppUI>();
        Button button = appUI.gameObject.GetComponent<Button>();
        button.interactable = true; // 恢復按鈕可點擊狀態
        if (appUI != null && appUI.miniInstance != null)
        {
            Destroy(appUI.miniInstance); // 銷毀最小化按鈕實例
        }
        // OnCloseButtonClicked?.Invoke(); // 觸發關閉視窗的事件（看來暫時不會用到）
    }
}
