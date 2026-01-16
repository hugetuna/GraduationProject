using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在販賣頁面根部（Singleton） */
public class SellManager : MonoBehaviour
{
    public static SellManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [SerializeField] private List<SetCharacterUIForSell> characterUIList = new();
    [SerializeField] private Button closeButton; // 關閉販賣頁面按鈕

    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    void Start()
    {
        // 初始化販賣頁面（角色部分）
        var idolList = TeamDataUtility.IdolInstanceList;
        for (int i = 0; i < characterUIList.Count; i++)
        {
            characterUIList[i].Initialize(idolList[i]); // 場景角色和角色 UI 相對應
        }

        // 設定關閉按鈕事件
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
