using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/* 掛在圖鑑視窗的粉絲卡片上（Button，非 Wrapper） */
public class SetGuideItemUI : MonoBehaviour
{
    private FansItem fansItem; // 粉絲資料
    private Image fansIcon; // 顯示粉絲圖示的 UI 元素
    public Sprite bgSprite; // 粉絲卡片背景
    //public Sprite collectedSprite; // 代表已收集的粉絲卡片背景
    //public Sprite uncollectedSprite; // 代表未收集的粉絲卡片背景
    //-----------------------------------------------------------------//
    private bool isCollected = true; // 暫時寫法，不確定之後會從哪取得資料

    void Start()
    {
        // 設定粉絲卡片 UI
        //GetComponent<Image>().sprite = isCollected ? collectedSprite : uncollectedSprite; // 背景
        GetComponent<Image>().sprite = bgSprite; // 背景
        fansIcon = transform.Find("Image").GetComponent<Image>(); // 圖示
        fansIcon.sprite = fansItem.icon;
    }

    public void SetFansItem(FansItem fans)
    {
        fansItem = fans;
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
