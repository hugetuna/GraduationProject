using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在背包 UI 底下的 ItemInfo 上 */
public class ItemInfoUI : MonoBehaviour
{
    public Image itemInfoIcon; // 道具詳細資訊的圖示
    public TextMeshProUGUI itemInfoName; // 道具詳細資訊的名稱
    public TextMeshProUGUI itemInfoDescription; // 道具詳細資訊的描述
    //-----------------------------------------------------------------//
    public List<Button> itemButtons = new(); // 儲存所有道具項目按鈕
    public Item selectedItem; // 當前選擇的道具
    //-----------------------------------------------------------------//
    public Vector2 originalPos = Vector2.zero; // 按鈕們的起始位置
    private Vector2 offset = new(11.0f, 0); // 被按下的按鈕會往右移動的距離


    public void OnButtonClick(Button clickedButton)
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            // 一般按鈕
            RectTransform rt = itemButtons[i].GetComponent<RectTransform>();
            rt.localPosition = originalPos;
        }

        // 被按下的按鈕（唯一）
        RectTransform clickedRt = clickedButton.GetComponent<RectTransform>();
        clickedRt.localPosition = originalPos + offset;

        selectedItem = clickedButton.GetComponent<SetItemUI>().item;
        itemInfoName.text = selectedItem.itemName;
        itemInfoDescription.text = selectedItem.description;
        itemInfoIcon.sprite = selectedItem.icon;

        // 確保字型正確渲染
        itemInfoName.ForceMeshUpdate();
        itemInfoDescription.ForceMeshUpdate();
    }

    public void ResetItemInfo()
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            RectTransform rt = itemButtons[i].GetComponent<RectTransform>();
            rt.localPosition = originalPos;
        }

        // 重置道具詳細資訊，並確保字形能正確渲染
        itemInfoIcon.sprite = null;
        itemInfoName.text = "";
        itemInfoName.ForceMeshUpdate();
        itemInfoDescription.text = "";
        itemInfoDescription.ForceMeshUpdate();

        selectedItem = null;
    }
}
