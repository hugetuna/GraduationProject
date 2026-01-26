using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在販賣頁面根部 */
public class SetSellUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private List<GameObject> characterUIList = new();
    [SerializeField] private Button closeButton; // 關閉販賣頁面按鈕
    //-----------------------------------------------------------------//
    private Dictionary<IdolInstance, List<ItemStack>> idolFansDict = new();

    void Start()
    {
        // 設定關閉按鈕事件
        closeButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Initialize()
    {
        // 初始化販賣頁面（角色部分）- 1
        var idolList = TeamDataUtility.IdolInstanceList;

        // 例外狀況處理
        if (idolList.Count != characterUIList.Count)
        {
            Debug.LogWarning($"角色數量 {idolList.Count} 與 UI 數量 {characterUIList.Count} 不符");
            return;
        }

        // 取得角色粉絲
        List<ItemStack> fansList = new();
        foreach (var itemStack in ResourceManager.Instance.items)
        {
            if (itemStack.item.itemType == ItemType.Fans)
            {
                fansList.Add(itemStack);
            }
        }
        // 本來應該按照 harvester 分配，但這裡先寫死
        foreach (var idol in idolList)
        {
            idolFansDict[idol] = new List<ItemStack>();
        }
        idolFansDict[idolList[0]].Add(fansList[0]);

        // 初始化販賣頁面（角色部分）- 2
        for (int i = 0; i < characterUIList.Count; i++)
        {
            var characterUI = characterUIList[i].GetComponent<SetCharacterUIForSell>();
            // 場景角色和角色 UI 相對應
            characterUI.Initialize(idolList[i], idolFansDict[idolList[i]]);
            Debug.Log($"初始化販賣頁面角色 UI：{idolList[i].idolIndex}");
        }
    }
}