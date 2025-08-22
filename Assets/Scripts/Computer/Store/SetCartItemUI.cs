using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在購物車項目的 prefab 上（不看 Wrapper）*/
public class SetCartItemUI : MonoBehaviour
{
    private ProductData product; // 商品資料
    private TextMeshProUGUI productNameText; // 顯示商品名稱的 UI 元素
    private TextMeshProUGUI productPriceText; // 顯示商品價格的 UI 元素


    void Awake()
    {
        productNameText = transform.Find("ProductText").GetComponent<TextMeshProUGUI>();
        productPriceText = transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
    }

    public void BindProduct(ProductData newProduct)
    {
        if (newProduct == null)
        {
            Debug.LogError("SetProduct 函式的參數為 null！");
            return;
        }

        if (productNameText == null || productPriceText == null)
        {
            Debug.LogError("UI 元件尚未設定完全！");
            return;
        }

        product = newProduct;

        productNameText.text = product.productName;
        productPriceText.text = $"${product.productPrice}";

        // 確保字型正確渲染
        productNameText.ForceMeshUpdate();
        productPriceText.ForceMeshUpdate();
    }
}
