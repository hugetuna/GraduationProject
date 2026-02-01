using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在購物車項目的 prefab 上（不看 Wrapper）*/
public class SetCartItemUI : MonoBehaviour
{
    private ProductRuntime productRuntime;
    private int quantity; // 購物車內的商品數量
    //-----------------------------------------------------------------//
    [Header("購物車項目的 UI 設定")]
    [SerializeField] private TextMeshProUGUI productNameText;
    [SerializeField] private TextMeshProUGUI productPriceText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private Button addButton; // 增加購物車內商品數量的按鈕
    [SerializeField] private Button reduceButton; // 減少購物車內商品數量的按鈕
    //-----------------------------------------------------------------//
    private CartController cartController;

    public void Initialize(ProductRuntime product, int qty)
    {
        // 設定該購物車項目對應的商品資料
        productRuntime = product;
        quantity = qty;

        // 設定購物車控制器
        cartController = GetComponentInParent<CartController>();

        // 設定 UI 顯示
        productNameText.text = product.product.item.itemName;
        productPriceText.text = $"${product.product.price}";
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
        cartController.AddToCart(productRuntime);
    }

    private void OnReduceClicked()
    {
        cartController.ReduceQuantity(productRuntime);
    }
}
