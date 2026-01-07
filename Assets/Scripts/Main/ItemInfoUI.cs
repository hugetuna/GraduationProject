using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在背包 UI 底下的 ItemInfo 上 */
public class ItemInfoUI : MonoBehaviour
{
    [SerializeField] private Image itemInfoIcon; // 道具詳細資訊的圖示
    [SerializeField] private TextMeshProUGUI itemInfoName; // 道具詳細資訊的名稱
    [SerializeField] private TextMeshProUGUI itemInfoDescription; // 道具詳細資訊的描述
    //-----------------------------------------------------------------//
    private List<Button> itemButtons = new(); // 儲存所有道具項目按鈕
    private static Item selectedItem = null; // 當前選擇的道具
    public static Item SelectedItem { get { return selectedItem; } }
    //-----------------------------------------------------------------//
    private Vector2 originalPos = Vector2.zero; // 按鈕們的起始位置
    public Vector2 OriginalPos { get { return originalPos; } set { originalPos = value; } }
    private Vector2 offset = new(11.0f, 0); // 被按下的按鈕會往右移動的距離


    void Start()
    {
        ResetItemInfo(); // 初始化道具詳細資訊為空
        PackUIHandler.OnPackUIClosed += ResetItemInfo; // 訂閱背包 UI 關閉事件
    }

    void OnDestroy()
    {
        PackUIHandler.OnPackUIClosed -= ResetItemInfo; // 取消訂閱背包 UI 關閉事件
    }

    public void OnItemClicked(Button clickedButton)
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

        selectedItem = clickedButton.GetComponent<SetItemUI>().Item;
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

    public void AddToItemButtons(Button btn)
    {
        itemButtons.Add(btn);
    }

    public void ClearItemButtons()
    {
        itemButtons.Clear();
    }
}
