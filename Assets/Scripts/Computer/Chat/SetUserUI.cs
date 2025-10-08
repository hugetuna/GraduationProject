using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/* 掛在用戶項目 UI 的 prefab 根部上（不看 Wrapper）*/
public class SetUserUI : MonoBehaviour
{
    [Header("用戶項目的 UI 設定")]
    private User user;
    [SerializeField] private Image userImage; // 用戶圖示
    [SerializeField] private TextMeshProUGUI userNameText; // 用戶名稱文字
    [SerializeField] private Image statusImage; // 未讀訊息之狀態圖示
    [SerializeField] private TextMeshProUGUI lastMessageText; // 最新未讀訊息文字
    [SerializeField] private TextMeshProUGUI lastTimeText; // 最新未讀訊息時間
    public static event Action<User> OnUserContentClicked; // 用戶項目被點擊的事件

    public void Initialize(User user)
    {
        // 設定用戶項目 UI
        this.user = user;
        userImage.sprite = user.userIcon;
        userNameText.text = user.userName;
        userNameText.ForceMeshUpdate();
        statusImage.gameObject.SetActive(false);

        // 註冊按鈕點擊事件
        GetComponent<Button>().onClick.AddListener(() =>
        {
            OnUserContentClicked?.Invoke(this.user);
        });

        // 未讀訊息相關的功能之後再寫
    }
}
