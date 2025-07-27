using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 主要用來生成會放在 ScrollView 的道具項目 UI */
public class ItemUIGenerator : MonoBehaviour
{
    public ResourceManager resourceManager; // 資源管理器，用於獲取道具清單
    private List<ItemStack> itemList = new(); // 儲存道具資訊的清單
    //-----------------------------------------------------------------//
    public GameObject itemPrefab; // 用於生成道具項目的預製件
    // public GameObject hintTextPrefab; // 用於生成提示文字的預製件（針對沒有道具的頁面）
    public Transform consumableContent; // 用於放置生成的道具物件的容器（消耗品）
    public Transform fansContent; // 用於放置生成的道具物件的容器（粉絲）
    public Transform equipContent; // 用於放置生成的道具物件的容器（裝備）
    //-----------------------------------------------------------------//
    private SetItemUI setItemUI;
    public ItemInfoUI itemInfoUI;

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
            itemInfoUI.itemButtons.Add(inside.GetComponent<Button>()); // 設定按鈕的點擊效果
            setItemUI = inside.GetComponent<SetItemUI>();
            setItemUI.item = itemStack.item; // 設置道具資料
            setItemUI.quantity = itemStack.quantity;
        }

        // 如果沒有道具，則顯示提示文字
        // if(consumableContent.childCount == 0)
        // {
        //     Instantiate(hintTextPrefab, consumableContent);
        // }else if(fansContent.childCount == 0)
        // {
        //     Instantiate(hintTextPrefab, fansContent);
        // }
        // else if(equipContent.childCount == 0)
        // {
        //     Instantiate(hintTextPrefab, equipContent);
        // }
    }

    void Update()
    {

    }
}
