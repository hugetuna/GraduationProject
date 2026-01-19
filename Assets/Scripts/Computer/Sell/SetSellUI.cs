using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在販賣頁面根部 */
public class SetSellUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private List<GameObject> characterUIList = new();
    [SerializeField] private Button closeButton; // 關閉販賣頁面按鈕
    [SerializeField] private Button transformButton; // 轉換按鈕
    //-----------------------------------------------------------------//
    private Dictionary<IdolInstance, List<FansItem>> idolFansDict = new();

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
        // 初始化販賣頁面（角色部分）- 1
        var idolList = TeamDataUtility.IdolInstanceList;
        
        // 例外狀況處理
        if (idolList.Count != characterUIList.Count)
        {
            Debug.LogWarning($"角色數量 {idolList.Count} 與 UI 數量 {characterUIList.Count} 不符");
            return;
        }

        // 取得角色粉絲
        List<FansItem> fansList = new();
        foreach(var itemStack in ResourceManager.Instance.items)
        {
            if(itemStack.item is FansItem fansItem)
            {
                fansList.Add(fansItem);
            }
        }
        foreach(var fans in fansList)
        {
            // 本來應該按照 harvester 分配，但這裡先寫死
            idolFansDict.Add(TeamDataUtility.IdolInstanceList[0], new List<FansItem>());
            idolFansDict[idolList[0]].Add(fans);
        }

        // 初始化販賣頁面（角色部分）- 2
        for (int i = 0; i < characterUIList.Count; i++)
        {
            var characterUI = characterUIList[i].GetComponent<SetCharacterUIForSell>();
            // 場景角色和角色 UI 相對應
            if(i == 0) characterUI.Initialize(idolList[i], idolFansDict[idolList[i]]);
            else characterUI.Initialize(idolList[i], null); 
            Debug.Log($"初始化販賣頁面角色 UI：{idolList[i].idolIndex}");
        }
    }
}
