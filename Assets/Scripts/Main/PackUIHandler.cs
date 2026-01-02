using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/* 掛在 UIManager 上 */
public class PackUIHandler : MonoBehaviour
{
    public GameObject packUI; // 背包 UI
    public Button packButton; // 背包按鈕
    public Button closeButton; // 關閉背包按鈕
    public Button panelBackground; // 點擊背景關閉 UI 的按鈕
    //-----------------------------------------------------------------//
    public AudioClip openPackSound;

    void Start()
    {
        packUI.SetActive(false); // 初始化背包 UI 狀態
        
        packButton.onClick.AddListener(OpenPackUI); // 設置按鈕點擊事件
        closeButton.onClick.AddListener(ClosePackUI);
        panelBackground.onClick.AddListener(ClosePackUI);
    }

    private void OpenPackUI()
    {
        if (!packUI.activeSelf) // 如果背包 UI 未開啟，則打開它
        {
            UIAndPlayerInput.DisableAllPlayerInputs(); // 禁用所有玩家的輸入系統
            packUI.SetActive(true);
            packUI.GetComponent<ItemUIGenerator>().RefreshPackUI(); // 刷新背包 UI 顯示
            AudioManager.Instance.PlaySFX(openPackSound); // 播放音效
        }
    }

    public void ClosePackUI() // 使用 UI 上的叉叉關閉 UI
    {
        Debug.Log("關閉 UI");
        // ResetPackUI();
        UIAndPlayerInput.EnableAllPlayerInputs(); // 啟用所有玩家的輸入系統
        packUI.SetActive(false);
    }

    // private void ResetPackUI()
    // {
    //     // 重置背包內的道具資訊顯示
    //     ItemInfoUI itemInfoUI = packUI.GetComponentInChildren<ItemInfoUI>();
    //     UseItem useItem = packUI.GetComponentInChildren<UseItem>();
    //     ChangeTypeUI changeTypeUI = packUI.GetComponentInChildren<ChangeTypeUI>();
    //     if (itemInfoUI != null && useItem != null)
    //     {
    //         itemInfoUI.ResetItemInfo();
    //         useItem.ResetDropdown();
    //         changeTypeUI.ResetTypeUI();
    //     }
    //     else
    //     {
    //         Debug.Log("背包 UI 重置失敗");
    //     }
    // }
}
