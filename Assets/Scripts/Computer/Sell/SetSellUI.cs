using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在販賣頁面根部 */
public class SetSellUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private List<SetCharacterUIForSell> characterUIList = new();
    [SerializeField] private Button closeButton; // 關閉販賣頁面按鈕
    [SerializeField] private Button transformButton; // 轉換按鈕

    void Start()
    {
        // 設定關閉按鈕事件
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        // 轉換按鈕預設為不可點擊
        transformButton.interactable = false;
    }

    public void Initialize()
    {
        // 初始化販賣頁面（角色部分）
        var idolList = TeamDataUtility.IdolInstanceList;
        if (idolList.Count != characterUIList.Count)
        {
            Debug.LogWarning("角色數量與 UI 數量不符，請檢查設定！");
            return;
        }
        for (int i = 0; i < characterUIList.Count; i++)
        {
            characterUIList[i].Initialize(idolList[i]); // 場景角色和角色 UI 相對應
            // Debug.Log($"初始化販賣頁面角色 UI：{idolList[i].idolIndex}");
        }
    }
}
