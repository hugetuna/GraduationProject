using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在背包頁面根部，主要用來生成會放在 ScrollView 的道具項目 UI */
public class ItemUIGenerator : MonoBehaviour
{
    public ResourceManager resourceManager; // 資源管理器，用於獲取道具清單
    private List<ItemStack> itemList = new(); // 儲存道具資訊的清單
    //-----------------------------------------------------------------//
    public GameObject itemPrefab; // 用於生成道具項目的預製件
    public Transform consumableContent; // 用於放置生成的道具物件的容器（消耗品）
    public Transform fansContent; // 用於放置生成的道具物件的容器（粉絲）
    public Transform equipContent; // 用於放置生成的道具物件的容器（裝備）
    //-----------------------------------------------------------------//
    public ItemInfoUI itemInfoUI; // 使生成的道具項目能夠與詳細資訊的腳本連接

    void Start()
    {
        itemList = resourceManager.items; // 從資源管理器獲取道具清單
        foreach (ItemStack itemStack in itemList) // 按清單生成初始的道具項目
        {
            // 生成道具並為其分門別類
            GameObject itemObject = null; // "Wrapper" + Button
            if (itemStack.item.itemType == ItemType.Consumable)
            {
                itemObject = Instantiate(itemPrefab, consumableContent);
            }
            else if (itemStack.item.itemType == ItemType.Fans)
            {
                itemObject = Instantiate(itemPrefab, fansContent);
            }
            else if (itemStack.item.itemType == ItemType.Equipment)
            {
                itemObject = Instantiate(itemPrefab, equipContent);
            }
            //itemObject = Instantiate(itemPrefab, consumableContent); // 未分類測試用

            if (itemObject == null)
            {
                Debug.Log("道具項目生成失敗！");
                continue;
            }
            GameObject inside = itemObject.transform.Find("Button").gameObject; // Wrapper + "Button"
            Button btn = inside.GetComponent<Button>();
            itemInfoUI.itemButtons.Add(btn); // 設定按鈕的點擊效果
            btn.onClick.AddListener(() => itemInfoUI.OnButtonClick(btn)); // 設定按鈕的點擊事件
            if (itemInfoUI.originalPos == Vector2.zero) // 記錄按鈕的起始位置
            {
                itemInfoUI.originalPos = inside.GetComponent<RectTransform>().localPosition;
            }
            // 設置道具資料
            SetItemUI setItemUI = inside.GetComponent<SetItemUI>();
            setItemUI.item = itemStack.item;
            setItemUI.quantity = itemStack.quantity;
        }
    }
}
