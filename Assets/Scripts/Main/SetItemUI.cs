using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetItemUI : MonoBehaviour
{
    public Item item; // 道具資料
    private TextMeshProUGUI itemNameText; // 顯示道具名稱的 UI 元素
    private TextMeshProUGUI itemStackText; // 顯示道具數量的 UI 元素
    private Image itemIcon; // 顯示道具圖示的 UI 元素

    void Start()
    {
        itemNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        itemStackText = transform.Find("StackText").GetComponent<TextMeshProUGUI>();
        itemIcon = transform.Find("Image").GetComponent<Image>();

        itemNameText.text = item.itemName;
        itemStackText.text = "x" + item.maxStack.ToString();
        itemIcon.sprite = item.icon;

        // 確保字型正確渲染
        itemNameText.ForceMeshUpdate();
        itemStackText.ForceMeshUpdate();
    }

    // void Update()
    // {

    // }
    
}
