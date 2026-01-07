using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

/* 掛在 UIManager 上 */
public class PackUIHandler : MonoBehaviour
{
    // public static Action OnPackUIOpened; // 當背包 UI 開啟時觸發的事件（打開背包介面並更新內容）
    public static Action OnPackUIClosed; // 當背包 UI 關閉時觸發的事件（重置道具＆角色選取狀態）
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject packUI; // 背包 UI
    [SerializeField] private Button packButton; // 背包按鈕
    [SerializeField] private Button closeButton; // 關閉背包按鈕
    [SerializeField] private Button panelBackground; // 點擊背景關閉 UI 的按鈕
    //-----------------------------------------------------------------//
    [SerializeField] private AudioClip openPackSound;

    void Start()
    {
        packUI.SetActive(false); // 初始化背包 UI 狀態

        packButton.onClick.AddListener(OpenPackUI); // 設置按鈕點擊事件
        closeButton.onClick.AddListener(ClosePackUI);
        panelBackground.onClick.AddListener(ClosePackUI);
    }

    private void OpenPackUI()
    {
        UIAndPlayerInput.DisableAllPlayerInputs(); // 禁用所有玩家的輸入系統
        packUI.SetActive(true);
        packUI.GetComponentInChildren<ItemUIGenerator>().RefreshPackUI(); // 開啟時刷新背包內容
        AudioManager.Instance.PlaySFX(openPackSound); // 播放音效
    }

    public void ClosePackUI() // 使用 UI 上的叉叉關閉 UI
    {
        Debug.Log("關閉 UI");
        UIAndPlayerInput.EnableAllPlayerInputs(); // 啟用所有玩家的輸入系統
        packUI.SetActive(false);
        OnPackUIClosed?.Invoke(); // 觸發背包 UI 關閉事件
    }
}
