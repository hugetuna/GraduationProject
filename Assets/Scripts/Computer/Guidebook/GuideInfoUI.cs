using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在圖鑑視窗的粉絲詳細資料上 */
public class GuideInfoUI : MonoBehaviour
{
    [Header("粉絲圖鑑的 UI 設定")] // 相關道具還不確定會怎麼設定，先放著
    public Image fansInfoIcon; // 粉絲詳細資訊的圖示
    public TextMeshProUGUI fansInfoName; // 粉絲詳細資訊的名稱
    public TextMeshProUGUI fansInfoDescription; // 粉絲詳細資訊的描述
    public TextMeshProUGUI quoteText; // 粉絲詳細資訊的座右銘 -> None
    public TextMeshProUGUI goldText; // 粉絲詳細資訊的奉獻金
    public TextMeshProUGUI powerText; // 粉絲詳細資訊的推坑力 -> None
    public TextMeshProUGUI harvestText; // 粉絲詳細資訊的累積收成數 -> None
    //-----------------------------------------------------------------//
    private List<Button> fansButtons = new(); // 所有粉絲卡片皆可點擊
    //private FansItem selectedFans; // 當前選擇的粉絲卡片
    public TextMeshProUGUI collectText; // 顯示目前收集的粉絲種類總數
    private int collectedFansSum = 0;

    void Start()
    {
        foreach (Button btn in fansButtons)
        {
            Button tempBtn = btn; // 捕捉當下按鈕以避免閉包問題
            tempBtn.onClick.AddListener(() => OnButtonClick(tempBtn));

            if (!tempBtn.GetComponent<SetGuideItemUI>().IsCollected())
            {
                tempBtn.interactable = false; // 不得查看未收集的粉絲資訊
            }
            else
            {
                collectedFansSum++; // 計算已收集的粉絲總數
            }
        }

        collectText.text = $"{collectedFansSum:D2} / {fansButtons.Count} 種";

        // 預設文字（之後會再改）
        quoteText.text = "這是屬於我的長椅";
        powerText.text = "推坑之力：0";
        harvestText.text = "累積收成數：0";
    }

    public void AddToFansButtons(Button newButton)
    {
        fansButtons.Add(newButton);
    }

    public void OnButtonClick(Button clickedButton)
    {
        // 被按下的按鈕（唯一）
        FansItem fans = clickedButton.GetComponent<SetGuideItemUI>().GetFansItem();
        fansInfoIcon.sprite = fans.icon;
        fansInfoName.text = fans.itemName;
        fansInfoDescription.text = fans.description;
        goldText.text = $"奉獻金：{fans.price}";

        // 確保字型正確渲染
        fansInfoName.ForceMeshUpdate();
        fansInfoDescription.ForceMeshUpdate();
        goldText.ForceMeshUpdate();
    }
}
