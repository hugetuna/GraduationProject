using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在商店視窗的商品卡片根部（不看 Wrapper）*/
public class SetProductUI : MonoBehaviour
{
    private ProductData product; // 商品資料
    private TextMeshProUGUI productNameText; // 顯示商品名稱的 UI 元素
    private TextMeshProUGUI productPriceText; // 顯示商品價格的 UI 元素
    private Image productImage; // 顯示商品圖示的 UI 元素
    //-----------------------------------------------------------------//
    [Header("商品與購物車的連接設定")]
    private Button cartButton; // 將商品加入購物車的按鈕
    private GameObject cartContent; // 購物車內容物件
    private GameObject cartItemPrefab; // 購物車項目預製件


    void Awake()
    {
        productNameText = transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        productPriceText = transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
        productImage = transform.Find("Image").GetComponent<Image>();
        cartButton = transform.Find("CartButton").GetComponent<Button>();
    }

    public void SetProduct(ProductData newProduct)
    {
        if (newProduct == null)
        {
            Debug.LogError("SetProduct 函式的參數為 null！");
            return;
        }

        if (productNameText == null || productPriceText == null || productImage == null)
        {
            Debug.LogError("UI 元件尚未設定完全！");
            return;
        }

        product = newProduct;

        productNameText.text = product.productName;
        productPriceText.text = $"${product.productPrice}";
        productImage.sprite = product.productImage;

        // 確保字型正確渲染
        productNameText.ForceMeshUpdate();
        productPriceText.ForceMeshUpdate();
    }

    public void SetCart(GameObject cartContentObj, GameObject cartItemPrefabObj)
    {
        if (cartContentObj == null || cartItemPrefabObj == null)
        {
            Debug.LogError("SetCart 函式的參數為 null！");
            return;
        }

        if (cartButton == null)
        {
            Debug.LogError("UI 元件尚未設定完全！");
            return;
        }

        cartContent = cartContentObj;
        cartItemPrefab = cartItemPrefabObj;

        // 設定按鈕點擊事件
        cartButton.onClick.AddListener(AddToCart);
    }

    public void AddToCart()
    {
        if (product == null)
        {
            Debug.LogError("product 變數為 null！");
            return;
        }

        // 將商品加入購物車
        GameObject cartItem = Instantiate(cartItemPrefab, cartContent.transform);
        if (cartItem == null)
        {
            Debug.LogError("購物車項目生成失敗！");
            return;
        }
        // 設定購物車項目的資料
        GameObject card = cartItem.transform.Find("Card").gameObject; // Wrapper + "Card"
        card.GetComponent<SetCartItemUI>().BindProduct(product);
    }
}
