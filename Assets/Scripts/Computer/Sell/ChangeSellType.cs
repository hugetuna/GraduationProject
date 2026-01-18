using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeSellType : MonoBehaviour
{
    [Header("轉換類別按鈕")]
    [SerializeField] private List<Button> sellTypeButtons = new(); // 儲存按鈕的列表，分別代表金錢、粉絲數和道具
    [SerializeField] private Sprite activeBtnImg;
    [SerializeField] private Sprite normalBtnImg;
    private float activeBtnY;
    private float normalBtnY;
    //-----------------------------------------------------------------//
    // private int currentIndex; // 當前選中的按鈕索引
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private GameObject moneyPage;
    [SerializeField] private GameObject fansPage;
    [SerializeField] private GameObject itemPage;

    void Start()
    {
        // 設定按鈕的點擊事件
        foreach (Button btn in sellTypeButtons)
        {
            Button tempBtn = btn; // 捕捉當下按鈕以避免閉包問題
            tempBtn.onClick.AddListener(() => OnButtonClick(tempBtn));
        }

        activeBtnY = sellTypeButtons[0].GetComponent<RectTransform>().anchoredPosition.y;
        normalBtnY = sellTypeButtons[1].GetComponent<RectTransform>().anchoredPosition.y;

        // 預設顯示金錢頁面（已在 Unity 編輯器中預先設定好圖片）
        moneyPage.SetActive(true);
        fansPage.SetActive(false);
        itemPage.SetActive(false);
        typeText.text = "轉換成金錢";
        // currentIndex = 0;
    }

    public void OnButtonClick(Button clickedButton)
    {
        for (int i = 0; i < sellTypeButtons.Count; i++)
        {
            // 一般按鈕：切換成普通圖片＆位置
            Image img = sellTypeButtons[i].GetComponent<Image>();
            RectTransform rt = sellTypeButtons[i].GetComponent<RectTransform>();
            img.sprite = normalBtnImg;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, normalBtnY);
        }

        // 被按下的按鈕（唯一）：切換成選中圖片＆位置，並顯示對應的道具頁面
        Image clickedImg = clickedButton.GetComponent<Image>();
        RectTransform clickedRt = clickedButton.GetComponent<RectTransform>();

        clickedImg.sprite = activeBtnImg;
        clickedRt.anchoredPosition = new Vector2(clickedRt.anchoredPosition.x, activeBtnY);

        if (clickedButton == sellTypeButtons[0]) // 假設第一個按鈕是金錢
        {
            moneyPage.SetActive(true);
            fansPage.SetActive(false);
            itemPage.SetActive(false);
            typeText.text = "轉換成金錢";
            // currentIndex = 0; // 更新當前索引
        }
        else if (clickedButton == sellTypeButtons[1]) // 假設第二個按鈕是粉絲數
        {
            moneyPage.SetActive(false);
            fansPage.SetActive(true);
            itemPage.SetActive(false);
            typeText.text = "轉換成粉絲數";
            // currentIndex = 1; // 更新當前索引
        }
        else if (clickedButton == sellTypeButtons[2]) // 假設第三個按鈕是道具
        {
            moneyPage.SetActive(false);
            fansPage.SetActive(false);
            itemPage.SetActive(true);
            typeText.text = "轉換成道具";
            // currentIndex = 2; // 更新當前索引
        }
    }
}
