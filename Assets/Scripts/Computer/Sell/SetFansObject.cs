using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在販賣頁面的粉絲 prefab 上 */
public class SetFansObject : MonoBehaviour
{
    [Header("UI 元素")]
    // [SerializeField] private Image fansIcon;
    [SerializeField] private Text fansNameText;
    [SerializeField] private Text fansNumText;
    //-----------------------------------------------------------------//
    [Header("粉絲資料")]
    private FansItem fansItem;

    public void Initialize(FansItem fans)
    {
        fansItem = fans;
        // fansIcon.sprite = item.iconSprite;
        // fansNameText.text = fans.itemName;
        // fansNumText.text = $"x{fans.quantity}";
    }
}
