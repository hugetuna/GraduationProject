using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在惡魔頁面 prefab 的根部 */
public class InternalDemonUI : MonoBehaviour
{
    [Header("惡魔頁面內部元素")]
    [SerializeField] private Button talkButton; // 對話按鈕
    [SerializeField] private Button problemButton; // 提問按鈕
    [SerializeField] private Button dialogueObject; // 對話框本身（點擊以推進對話）
    [SerializeField] private TextMeshProUGUI dialogueText; // 惡魔頁面的對話框文字
    [SerializeField] private GameObject hintIcon; // 提示按鈕是否被點擊過
    private int talkStage = 0;
    //-----------------------------------------------------------------//
    [SerializeField] private Button sellButton; // 可開啟販賣頁面的按鈕
    [SerializeField] private GameObject sellUI; // 販賣頁面
    private Button closeSellButton; // 關閉販賣頁面的按鈕

    // 退出惡魔頁面的按鈕寫在 DemonUIHandler 腳本（讓惡魔頁面的開關能統一管理）

    void Start()
    {
        talkButton.onClick.AddListener(OnTalkButtonClick);
        dialogueObject.onClick.AddListener(OnDialogueBgClick);
        problemButton.onClick.AddListener(OnProblemButtonClick);

        sellButton.onClick.AddListener(() =>
        {
            sellUI.SetActive(true); // 開啟販賣頁面
            dialogueObject.interactable = false; // 開啟販賣頁面後禁用對話框點擊
        });

        closeSellButton = sellUI.transform.Find("Close").GetComponent<Button>();
        closeSellButton.onClick.AddListener(() =>
        {
            sellUI.SetActive(false); // 關閉販賣頁面
            dialogueObject.interactable = true; // 關閉販賣頁面後啟用對話框點擊
        });

        dialogueText.text = "你好，找我有什麼事？"; // 初始對話框文字
        hintIcon.SetActive(true); // 預設提示圖示為顯示狀態
        sellUI.SetActive(false); // 預設隱藏販賣頁面
    }

    void OnEnable()
    {
        // 每次開啟惡魔頁面時重置對話狀態
        dialogueText.text = "你好，找我有什麼事？";
        talkStage = 0;
    }

    private void OnTalkButtonClick()
    {
        if (talkStage == 0 || talkStage == 2)
        {
            dialogueText.text = "培養偶像的過程還順利嗎？"; // 對話一
            hintIcon.SetActive(false);
            talkStage = 1;
        }
    }

    private void OnDialogueBgClick()
    {
        if (talkStage == 1)
        {
            dialogueText.text = "有我的協助肯定不會失敗的"; // 對話二
            talkStage = 2;
        }
    }

    private void OnProblemButtonClick()
    {
        dialogueText.text = "我是沒遇到什麼問題……其實有問題想問的是你吧？"; // 提問按鈕的對話
    }
}
