using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在購物車項目的 prefab 上（不看 Wrapper）*/
public class SetCartItemUI : MonoBehaviour
{
    private Product product;
    private int quantity; // 購物車內的商品數量
    //-----------------------------------------------------------------//
    [Header("購物車項目的 UI 設定")]
    [SerializeField] private TextMeshProUGUI productNameText;
    [SerializeField] private TextMeshProUGUI productPriceText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button addButton; // 增加購物車內商品數量的按鈕
    [SerializeField] private Button reduceButton; // 減少購物車內商品數量的按鈕
    //-----------------------------------------------------------------//
    private CartController controller; // 方便存取其實例

    public void SetController(CartController ctrl)
    {
        controller = ctrl;
    }

    public void Initialize(Product product, int qty)
    {
        // 設定該購物車項目對應的商品資料
        this.product = product;
        quantity = qty;

        // 設定 UI 顯示
        productNameText.text = product.item.itemName;
        productPriceText.text = $"${product.price}";
        UpdateCartQuantity(quantity);

        // 商品購買數量的增減控制
        addButton.onClick.AddListener(OnAddClicked);
        reduceButton.onClick.AddListener(OnReduceClicked);
    }

    public void UpdateCartQuantity(int qty)
    {
        quantity = qty;
        quantityText.text = quantity.ToString();
    }

    private void OnAddClicked()
    {
        controller.AddToCart(product);
    }

    private void OnReduceClicked()
    {
        controller.ReduceQuantity(product);
    }
}
