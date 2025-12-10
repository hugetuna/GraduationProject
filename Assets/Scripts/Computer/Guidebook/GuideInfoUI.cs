using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在圖鑑視窗的粉絲詳細資料上 */
public class GuideInfoUI : MonoBehaviour
{
    [Header("粉絲圖鑑的 UI 設定")]
    public Image fansInfoIcon; // 粉絲詳細資訊的圖示
    public TextMeshProUGUI fansInfoName; // 粉絲詳細資訊的名稱
    public TextMeshProUGUI fansInfoDescription; // 粉絲詳細資訊的描述
    public TextMeshProUGUI quoteText; // 粉絲詳細資訊的座右銘 -> None
    public TextMeshProUGUI goldText; // 粉絲詳細資訊的奉獻金
    public TextMeshProUGUI powerText; // 粉絲詳細資訊的推坑力
    public TextMeshProUGUI harvestText; // 粉絲詳細資訊的累積收成數 -> None
    //-----------------------------------------------------------------//
    public GameObject relevantItems; // 粉絲詳細資訊的掉落道具（父物件）
    private List<Image> itemSlots = new(); // 掉落道具的欄位列表（子物件們）
    //-----------------------------------------------------------------//
    private List<Button> fansButtons = new(); // 所有粉絲卡片皆有點擊效果
    [SerializeField] private Color32 normalColor = new(255, 255, 255, 255); // 按鈕正常顏色
    [SerializeField] private Color32 pressedColor = new(200, 200, 200, 255); // 按鈕被按下的顏色
    //-----------------------------------------------------------------//
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
        
        fansInfoIcon.sprite = fans.icon;
        
        fansInfoName.text = fans.itemName;
        fansInfoDescription.text = fans.description;
        goldText.text = $"奉獻金：{fans.moneyPower}";
        powerText.text = $"推坑之力：{fans.OShiPower}";
        harvestText.text = $"累積收成數 1"; // 暫時寫死，之後再改

        foreach (Image slot in itemSlots) slot.sprite = null; // 先清空欄位
        for(int i = 0; i < fans.dropableItems.Count; i++)
        {
            itemSlots[i].sprite = fans.dropableItems[i].icon;
        }

        // 確保字型正確渲染
        fansInfoName.ForceMeshUpdate();
        fansInfoDescription.ForceMeshUpdate();
        goldText.ForceMeshUpdate();
        powerText.ForceMeshUpdate();
        harvestText.ForceMeshUpdate();
    }
}
