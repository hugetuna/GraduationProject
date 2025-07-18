using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfoUI : MonoBehaviour
{
    private Image itemInfoIcon; // 道具詳細資訊的圖示
    private TextMeshProUGUI itemInfoName; // 道具詳細資訊的名稱
    private TextMeshProUGUI itemInfoDescription; // 道具詳細資訊的描述
    //-----------------------------------------------------------------//
    public Button[] itemButtons;
    private Vector3[] originalButtonPos; // 儲存按鈕的原始位置
    //-----------------------------------------------------------------//
    public Item sellectedItem; // 當前選擇的道具

    void Start()
    {
        itemInfoName = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        itemInfoDescription = transform.Find("EffectText").GetComponent<TextMeshProUGUI>();
        itemInfoIcon = transform.Find("Image").GetComponent<Image>();

        originalButtonPos = new Vector3[itemButtons.Length];
        for (int i = 0; i < itemButtons.Length; i++)
        {
            Button btn = itemButtons[i];
            RectTransform rt = btn.GetComponent<RectTransform>();
            originalButtonPos[i] = rt.localPosition;

            Button tempBtn = btn; // 捕捉當下按鈕以避免閉包問題
            tempBtn.onClick.AddListener(() => OnButtonClick(tempBtn));
        }
    }

    public void OnButtonClick(Button clickedButton)
    {
        for (int i = 0; i < itemButtons.Length; i++)
        {
            // 一般按鈕
            RectTransform rt = itemButtons[i].GetComponent<RectTransform>();
            rt.localPosition = originalButtonPos[i];
        }

        // 被按下的按鈕
        RectTransform clickedRt = clickedButton.GetComponent<RectTransform>();
        float x = originalButtonPos[0].x + 11.0f; // 每個按鈕的原始 x 位置都一樣
        float y = clickedRt.localPosition.y;
        clickedRt.localPosition = new Vector3(x, y, 0);

        Item item = sellectedItem = clickedButton.GetComponent<SetItemUI>().item;
        itemInfoName.text = item.itemName;
        itemInfoDescription.text = item.description;
        itemInfoIcon.sprite = item.icon;

        // 確保字型正確渲染
        itemInfoName.ForceMeshUpdate();
        itemInfoDescription.ForceMeshUpdate();
    }
}
