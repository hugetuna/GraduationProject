using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* 掛在販賣頁面掉落道具的 prefab 根部 */
public class SetFansDropItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 元素")]
    [SerializeField] private GameObject numBg; // 顯示道具數量時所需的背景
    [SerializeField] private Image icon; // 道具圖示
    [SerializeField] private TextMeshProUGUI numText; // 道具數量文字
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject hoverEffect; // 滑鼠懸停效果
    [SerializeField] private TextMeshProUGUI nameText; // 道具名稱文字
    [SerializeField] private TextMeshProUGUI descriptionText; // 道具描述文字

    public void Initialize(ItemStack itemStack)
    {
        Item item = itemStack.item;
        int quantity = itemStack.quantity;

        // 設定圖示
        icon.sprite = item.icon;

        // 設定數量
        if (quantity > 1)
        {
            numBg.SetActive(true);
            numText.gameObject.SetActive(true);
            numText.text = quantity.ToString();
        }
        else
        {
            numBg.SetActive(false);
            numText.gameObject.SetActive(false);
        }

        // 設定名稱和描述
        nameText.text = item.itemName;
        descriptionText.text = item.description;

        // 初始化 hover 效果
        hoverEffect.SetActive(false);
    }

    // 當滑鼠游標「進入」物件範圍時觸發 hover 效果
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverEffect.SetActive(true);
    }

    // 當滑鼠游標「離開」物件範圍時觸發
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverEffect.SetActive(false);
    }


}
