using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/* 掛在販賣頁面根部 */
public class SetSellUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private List<GameObject> characterUIList = new();
    [SerializeField] private Button closeButton; // 關閉販賣頁面按鈕
    //-----------------------------------------------------------------//
    private List<ItemStack> testFansList = new(); // 測試用粉絲資料
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

        /* 處理測試用粉絲 */
        AddTestFansData(idolList);

        // 初始化角色粉絲字典
        foreach (var idol in idolList) idolFansDict[idol] = new List<ItemStack>();

        // 抓取所有粉絲資料並根據 harvester 分類 
        foreach (var itemStack in ResourceManager.Instance.items)
        {
            if (itemStack.item is FansItem fansItem) AddToIdolFansDict(fansItem, itemStack.quantity);
        }

        // 初始化販賣頁面（角色部分）- 2
        for (int i = 0; i < characterUIList.Count; i++)
        {
            var characterUI = characterUIList[i].GetComponent<SetCharacterUIForSell>();
            characterUI.Initialize(idolList[i], idolFansDict[idolList[i]]); // 場景角色和角色 UI 相對應
            Debug.Log($"初始化販賣頁面角色 UI：{idolList[i].idolIndex}");
        }
    }

    private void AddTestFansData(List<IdolInstance> idolList)
    {
        // 測試用粉絲已預先加入 ResourceManager
        foreach(var itemStack in ResourceManager.Instance.items)
        {
            if (itemStack.item is FansItem)
            {
                testFansList.Add(itemStack);
            }
        }
        
        // 確保測試用粉絲的 harvester 不為空
        for (int i = 0; i < testFansList.Count; i++)
        {
            var item = testFansList[i].item as FansItem;
            item.harvester = idolList[i % idolList.Count].idolIndex;
        }
    }

    private void AddToIdolFansDict(FansItem fansItem, int amount = 1)
    {
        // 直接從 Keys 中找出符合 harvester 的那個 Idolindex (IdolInstance)
        var idol = idolFansDict.Keys.FirstOrDefault(k => k.idolIndex == fansItem.harvester);

        if (idol != null)
        {
            var fansList = idolFansDict[idol];

            // 檢查該角色的粉絲清單，是否已有此粉絲項目
            int index = fansList.FindIndex(f => f.item == fansItem);

            if (index != -1)
            {
                // 已有此粉絲：更新數量 (因為是 struct，所以取出來改完再塞回去)
                var stack = fansList[index];
                stack.quantity += amount;
                fansList[index] = stack;
            }
            else
            {
                // 沒找到該粉絲：新增一格
                fansList.Add(new ItemStack(fansItem, amount));
            }
        }
    }
}