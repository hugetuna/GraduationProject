using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 掛在商店視窗的 MoneyText 上 */
public class MoneyUsage : MonoBehaviour
{
    private ResourceManager resourceManager;
    private TextMeshProUGUI moneyText;

    void Awake()
    {
        moneyText = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        resourceManager = ResourceManager.Instance;

        // 先在 ResourceManager 設定金錢，避免無法測試商店購買功能
        // resourceManager.GainMoney(1000); 
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
