using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在商店視窗的 TypeSelector 底下 */
public class ChangeProductType : MonoBehaviour
{
    [Header("商品類型按鈕＆頁面切換")]

    [Tooltip("商品分類按鈕（需與商品分類頁面互相對應）")]
    public List<Button> productTypeButtons = new();

    [Tooltip("商品分類頁面（需與商品類型按鈕互相對應）")]
    public List<GameObject> productTypePages = new();
    public Sprite activeBtnImg;
    public Sprite normalBtnImg;
    public ScrollRect productScrollRect;
    // private int currentIndex = 0; // 當前選中的按鈕索引

    void Start()
    {
        // 設定按鈕的點擊事件
        foreach (Button btn in productTypeButtons)
        {
            Button tempBtn = btn; // 捕捉當下按鈕以避免閉包問題
            tempBtn.onClick.AddListener(() => OnButtonClick(tempBtn));
        }

        // 預設顯示第一個分類頁面，其他先隱藏
        for(int i = 0; i < productTypePages.Count; i++)
        {
            if (i == 0){
                productTypePages[i].SetActive(true);
                productScrollRect.content = productTypePages[i].GetComponent<RectTransform>();
            }
            else productTypePages[i].SetActive(false);
        }
    }

    public void OnButtonClick(Button clickedButton)
    {
        for (int i = 0; i < productTypeButtons.Count; i++)
        {
            // 一般按鈕：切換成普通圖片，並隱藏頁面
            Image img = productTypeButtons[i].GetComponent<Image>();
            img.sprite = normalBtnImg;
            productTypePages[i].SetActive(false);
        }

        // 被按下的按鈕（唯一）：切換成選中圖片，並顯示對應的商品頁面
        Image clickedImg = clickedButton.GetComponent<Image>();
        clickedImg.sprite = activeBtnImg;

        // 目前有四個預設類別（雖然還沒有明確的分類）
        if (clickedButton == productTypeButtons[0])
        {
            productTypePages[0].SetActive(true);
            productScrollRect.content = productTypePages[0].GetComponent<RectTransform>();
            // currentIndex = 0; // 更新當前索引
        }
        else if (clickedButton == productTypeButtons[1])
        {
            productTypePages[1].SetActive(true);
            productScrollRect.content = productTypePages[1].GetComponent<RectTransform>();
            // currentIndex = 1; // 更新當前索引
        }
        else if (clickedButton == productTypeButtons[2])
        {
            productTypePages[2].SetActive(true);
            productScrollRect.content = productTypePages[2].GetComponent<RectTransform>();
            // currentIndex = 2; // 更新當前索引
        }
        else if (clickedButton == productTypeButtons[3])
        {
            productTypePages[3].SetActive(true);
            productScrollRect.content = productTypePages[3].GetComponent<RectTransform>();
            // currentIndex = 3; // 更新當前索引
        }
    }
}
