using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 掛在商店視窗的 MoneyText 上 */
public class MoneyUsage : MonoBehaviour
{
    private ResourceManager resourceManager; // 從 WindowDataSetup 取得靜態變數
    private TextMeshProUGUI moneyText;

    void Awake()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        resourceManager = WindowDataSetup.GetResourceManager();
        resourceManager.GainMoney(1000); // 避免無法測試商店購買功能
        UpdateMoneyText();

        CartController.OnPurchaseSuccess += UpdateMoneyText; // 訂閱結帳事件
    }

    void OnDestroy()
    {
        CartController.OnPurchaseSuccess -= UpdateMoneyText; // 取消訂閱結帳事件
    }

    public void UpdateMoneyText()
    {
        moneyText.text = $"$ {resourceManager.getMoney():N0}";
    }

}
