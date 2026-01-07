using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在背包 UI 根部，用來生成會放在 ScrollView 的道具項目 UI */
public class ItemUIGenerator : MonoBehaviour
{
    private ResourceManager resourceManager; // 資源管理器，用於獲取道具清單
    [SerializeField] private List<ItemStack> itemList = new(); // 儲存道具資訊的清單
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject itemPrefab; // 用於生成道具項目的預製件
    [SerializeField] private Transform consumableContent; // 用於放置生成的道具物件的容器（消耗品）
    [SerializeField] private Transform fansContent; // 用於放置生成的道具物件的容器（粉絲）
    [SerializeField] private Transform equipContent; // 用於放置生成的道具物件的容器（裝備）
    //-----------------------------------------------------------------//
    private List<GameObject> itemSlotPool = new(); // 物件池，儲存已生成的道具項目物件
    private ItemInfoUI itemInfoUI; // 使生成的道具項目能夠與詳細資訊的腳本連接

    void Awake()
    {
        itemInfoUI = GetComponentInChildren<ItemInfoUI>();
        resourceManager = ResourceManager.Instance;
    }

    public void RefreshPackUI()
    {
        if(!resourceManager.IsItemChanged) return; // 如果道具沒有變化就不刷新
        
        // 取代全部銷毀，把舊的道具項目全部「關掉」收回池子
        ReturnAllToPool();

        itemList = resourceManager.items;
        // Debug.Log("道具清單長度：" + itemList.Count);

        foreach (ItemStack itemStack in itemList)
        {
            // 從池子拿物件，拿不到才生成
            GameObject itemObject = GetObjectFromPool();

            // 根據類型決定它的父物件
            Transform targetContent = itemStack.item.itemType switch
            {
                ItemType.Consumable => consumableContent,
                ItemType.Fans => fansContent,
                ItemType.Equipment => equipContent,
                _ => consumableContent
            };

            itemObject.transform.SetParent(targetContent, false);
            itemObject.SetActive(true); // 啟用該道具項目

            // 處理內部按鈕與資料設定
            var inside = itemObject.transform.Find("Button");
            var btn = inside.GetComponent<Button>();
            var setItemUI = inside.GetComponent<SetItemUI>();

            setItemUI.Initialize(itemStack.item, itemStack.quantity);

            btn.onClick.RemoveAllListeners(); // 移除舊的監聽事件避免重複
            btn.onClick.AddListener(() => itemInfoUI.OnItemClicked(btn));

            itemInfoUI.AddToItemButtons(btn);
            if (itemInfoUI.OriginalPos == Vector2.zero)
            {
                itemInfoUI.OriginalPos = inside.localPosition;
            }
        }

        resourceManager.SetItemChanged(false); // 重置變更標記
    }

    private GameObject GetObjectFromPool()
    {
        // 找看看池子裡有沒有已生成、但沒人在用的道具項目
        foreach (GameObject obj in itemSlotPool)
        {
            if (!obj.activeSelf) return obj;
        }

        // 真的沒人了，才當場生成一個，並加入池子
        GameObject newObj = Instantiate(itemPrefab);
        itemSlotPool.Add(newObj);
        return newObj;
    }

    private void ReturnAllToPool()
    {
        // 把所有池子裡的物件隱藏
        foreach (GameObject obj in itemSlotPool)
        {
            obj.SetActive(false);
        }

        // 按鈕清單還是要清空，因為重新記錄
        itemInfoUI.ClearItemButtons();
    }

    public List<Transform> GetAllItemTypeContent()
    {
        return new List<Transform> { consumableContent, fansContent, equipContent };
    }
}
