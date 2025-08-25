using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在購物車項目的 prefab 上（不看 Wrapper）*/
public class SetCartItemUI : MonoBehaviour
{
    private Item product;
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

    public void SetProduct(Item item, int qty)
    {
        // 設定該購物車項目對應的商品資料
        product = item;
        quantity = qty;

        // 設定 UI 顯示
        productNameText.text = product.itemName;
        productPriceText.text = $"${product.price}";
        SetQuantity(quantity);

        // 商品購買數量的增減控制
        addButton.onClick.AddListener(OnAddClicked);
        reduceButton.onClick.AddListener(OnReduceClicked);
    }

    public void SetQuantity(int qty) // 變更購買數量用
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
