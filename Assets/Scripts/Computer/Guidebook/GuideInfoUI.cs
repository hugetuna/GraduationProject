using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在圖鑑視窗的粉絲詳細資料上 */
public class GuideInfoUI : MonoBehaviour
{
    [Header("粉絲圖鑑的 UI 設定")]
    [SerializeField] private Image fansInfoIcon; // 粉絲詳細資訊的圖示
    [SerializeField] private TextMeshProUGUI fansInfoName; // 粉絲詳細資訊的名稱
    [SerializeField] private TextMeshProUGUI fansInfoDescription; // 粉絲詳細資訊的描述
    [SerializeField] private TextMeshProUGUI quoteText; // 粉絲詳細資訊的座右銘 -> 待補充
    [SerializeField] private TextMeshProUGUI goldText; // 粉絲詳細資訊的奉獻金
    [SerializeField] private TextMeshProUGUI powerText; // 粉絲詳細資訊的推坑力
    [SerializeField] private TextMeshProUGUI harvestText; // 粉絲詳細資訊的累積收成數 -> None
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject relevantItems; // 粉絲詳細資訊的掉落道具（父物件）
    private List<Image> itemSlots = new(); // 掉落道具的欄位列表（子物件們）
    //-----------------------------------------------------------------//
    private List<Button> fansButtons = new(); // 所有粉絲卡片皆有點擊效果
    [SerializeField] private Color32 normalColor = new(255, 255, 255, 255); // 按鈕正常顏色
    [SerializeField] private Color32 pressedColor = new(200, 200, 200, 255); // 按鈕被按下的顏色
    //-----------------------------------------------------------------//
    [SerializeField] private TextMeshProUGUI collectText; // 顯示目前收集的粉絲種類總數
    private int collectedFansSum = 0;

    void Start()
    {
        // 預設圖片與文字
        fansInfoIcon.sprite = null;
        fansInfoName.text = "";
        fansInfoDescription.text = "";
        quoteText.text = "";
        goldText.text = "奉獻金：";
        powerText.text = "推坑之力：";
        harvestText.text = "累積收成數";

        // 預設相關道具欄位
        foreach (Transform slot in relevantItems.transform)
        {
            itemSlots.Add(slot.GetComponent<Image>());
            slot.GetComponent<Image>().sprite = null;
        }
    }

    public void AddToFansButtons(Button newButton)
    {
        fansButtons.Add(newButton);
    }

    public void UpdateCollectNumber()
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
    }

    public void OnButtonClick(Button clickedButton)
    {
        // 一般按鈕
        foreach (Button btn in fansButtons)
        {
            btn.gameObject.GetComponent<Image>().color = normalColor;
        }

        // 被按下的按鈕（唯一）
        clickedButton.gameObject.GetComponent<Image>().color = pressedColor;

        // 根據按下的按鈕更新粉絲詳細資訊
        FansItem fans = clickedButton.GetComponent<SetGuideItemUI>().GetFansItem();

        // 文字與圖片更新
        fansInfoIcon.sprite = fans.icon;
        fansInfoName.text = fans.itemName;
        fansInfoDescription.text = fans.description;
        quoteText.text = fans.quote;
        goldText.text = $"奉獻金：{fans.moneyPower}";
        powerText.text = $"推坑之力：{fans.OShiPower}";
        harvestText.text = $"累積收成數 1"; // 暫時寫死，之後再改

        // 先清空原有道具欄位再更新
        foreach (Image slot in itemSlots) slot.sprite = null;
        for (int i = 0; i < fans.dropableItems.Count; i++)
        {
            itemSlots[i].sprite = fans.dropableItems[i].icon;
        }

        // 確保字型正確渲染
        fansInfoName.ForceMeshUpdate();
        fansInfoDescription.ForceMeshUpdate();
        quoteText.ForceMeshUpdate();
        goldText.ForceMeshUpdate();
        powerText.ForceMeshUpdate();
        harvestText.ForceMeshUpdate();
    }
}
