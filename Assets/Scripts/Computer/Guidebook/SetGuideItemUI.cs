using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/* 掛在圖鑑視窗的粉絲卡片上（Button，非 Wrapper） */
public class SetGuideItemUI : MonoBehaviour
{
    private FansItem fansItem; // 粉絲資料
    public Image fansBgImg; // 粉絲卡片的背景圖像
    public Image fansIcon; // 顯示粉絲圖示的 UI 元素
    public Sprite collectedSprite; // 代表已收集的粉絲卡片背景
    public Sprite uncollectedSprite; // 代表未收集的粉絲卡片背景
    //-----------------------------------------------------------------//
    private bool isCollected = true; // 暫時寫法，不確定之後會從哪取得資料

    // void Start()
    // {

    // }

    public void SetFansItemAndUI(FansItem fans)
    {
        fansItem = fans;

        // 設定粉絲卡片 UI
        if (fansItem.itemID.Contains("001") || fansItem.itemID.Contains("003"))
        {
            fansBgImg.sprite = collectedSprite;
        }
        else
        {
            fansBgImg.sprite = uncollectedSprite;
        }

        if (fansItem.icon != null) fansIcon.sprite = fansItem.icon;
        else fansIcon.color = new Color(0, 0, 0, 0); // 如果沒有圖示，將圖像設為透明

    }

    public FansItem GetFansItem()
    {
        return fansItem;
    }

    public bool IsCollected()
    {
        return isCollected;
    }
}
