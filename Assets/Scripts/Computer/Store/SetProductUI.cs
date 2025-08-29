using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在商店視窗的商品卡片根部（不看 Wrapper）*/
public class SetProductUI : MonoBehaviour
{
    [Header("商品卡片的 UI 設定")]
    private Item product;
    [SerializeField] private TextMeshProUGUI productNameText; // 商品名稱文字
    [SerializeField] private TextMeshProUGUI productPriceText; // 商品價格文字
    [SerializeField] private Image productImage; // 商品圖片
    [SerializeField] private Button cartButton; // 加入購物車的按鈕

    public void SetProduct(Item newProduct)
    {
        product = newProduct;

        // 設定 UI 顯示
        productNameText.text = product.itemName;
        productPriceText.text = $"${product.price}";
        productImage.sprite = product.icon;

        // 確保字型正確渲染
        productNameText.ForceMeshUpdate(); 
        productPriceText.ForceMeshUpdate();

        cartButton.onClick.RemoveAllListeners(); // 確保不會重複註冊事件
        cartButton.onClick.AddListener(() =>
        {
            CartController.Instance.AddToCart(product);
        });
    }
}
