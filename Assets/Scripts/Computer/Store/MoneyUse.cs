using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* 掛在商店視窗的 MoneyText 上 */
public class MoneyUse : MonoBehaviour
{
    private ResourceManager resourceManager; // 從 WindowDataSetup 取得靜態變數
    private TextMeshProUGUI moneyText;

    void Start()
    {
        resourceManager = WindowDataSetup.GetResourceManager();

        moneyText = GetComponent<TextMeshProUGUI>();
        moneyText.text = $"${resourceManager.getMoney()}";
    }

    void Update()
    {
        
    }

}
