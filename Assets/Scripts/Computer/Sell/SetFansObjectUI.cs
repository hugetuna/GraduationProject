using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在販賣頁面的粉絲 prefab 上 */
public class SetFansObjectUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private Image fansIcon;
    [SerializeField] private TextMeshProUGUI fansNameText;
    [SerializeField] private TextMeshProUGUI fansNumText;
    //-----------------------------------------------------------------//
    [Header("粉絲資料")]
    private Item fansItem;

    public void Initialize(ItemStack fansItemStack)
    {
        fansItem = fansItemStack.item;
        fansIcon.sprite = fansItem.icon;
        fansNameText.text = fansItem.itemName;
        fansNumText.text = $"{fansItemStack.quantity}";
    }
}
