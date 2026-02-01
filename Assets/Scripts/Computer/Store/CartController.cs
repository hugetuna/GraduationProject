using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 單筆購物車資料
[Serializable]
public class CartItemData
{
    public ProductRuntime productToBuy;
    public int quantity;

    public CartItemData(ProductRuntime product, int qty = 1)
    {
        productToBuy = product;
        quantity = qty;
    }
}


/* 掛在商店視窗根部 */
// 控制整個購物車
public class CartController : MonoBehaviour
{
    [Header("購物車的 UI 設定")]
    [SerializeField] private Transform cartContent; // 購物車容器
    [SerializeField] private GameObject cartItemPrefab; // 購物車項目 Prefab
    [SerializeField] private TextMeshProUGUI totalPriceText; // 總價文字
    [SerializeField] private Button buyButton; // 購物車結帳按鈕
    //-----------------------------------------------------------------//
    private Dictionary<ProductRuntime, CartItemData> cartData = new(); // 存放商品與其數量
    private Dictionary<ProductRuntime, SetCartItemUI> cartItemUIDict = new(); // 存放商品與其 UI 控制器
    //-----------------------------------------------------------------//
    [Header("結帳功能設定")]
    private int totalPrice = 0; // 購物車總價
    [SerializeField] private TextMeshProUGUI moneyText; // 玩家持有金錢文字
    public static event Action OnPurchaseSuccess; // 購買成功的事件
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    public AudioClip checkBillSound;
    public AudioClip failToBuySound;

    void Start()
    {
        buyButton.onClick.AddListener(CheckBill);

        UpdateTotalPrice(); // 確保初始總價正確
    }

    public void AddToCart(ProductRuntime product)
    {
        if (cartData.ContainsKey(product))
        {
            // 若該購物車項目已存在，增加其購買數量
            cartData[product].quantity++;
            if (cartData[product].quantity > product.currentStack) // 若購買數量超過庫存
            {
                Debug.Log("已達庫存上限，無法再增加購買數量");
                cartData[product].quantity = product.currentStack; // 限制購買數量為庫存數量
            }

            cartItemUIDict[product].UpdateCartQuantity(cartData[product].quantity);
        }
        else
        {
            // 若該購物車項目不存在，則新增一個
            CartItemData cartItemData = new(product);
            cartData[product] = cartItemData; // 記錄商品與其數量

            GameObject cartObject = Instantiate(cartItemPrefab, cartContent); // 生成購物車項目
            GameObject card = cartObject.transform.Find("Card").gameObject; // Wrapper + Card
            SetCartItemUI setCartItemUI = card.GetComponent<SetCartItemUI>();

            setCartItemUI.Initialize(product, cartItemData.quantity); // 傳遞商品資料與數量
            cartItemUIDict[product] = setCartItemUI; // 記錄商品與其 UI 控制器
        }

        UpdateTotalPrice();
    }

    public void ReduceQuantity(ProductRuntime product)
    {
        if (!cartData.ContainsKey(product)) return;

        cartData[product].quantity--;
        if (cartData[product].quantity <= 0)
        {
            Destroy(cartItemUIDict[product].transform.parent.gameObject); // Wrapper + Card
            cartData.Remove(product);
            cartItemUIDict.Remove(product);
        }
        else
        {
            cartItemUIDict[product].UpdateCartQuantity(cartData[product].quantity);
        }

        UpdateTotalPrice();
    }


    private void UpdateTotalPrice()
    {
        totalPrice = 0;
        foreach (var data in cartData.Values)
        {
            totalPrice += data.productToBuy.product.price * data.quantity;
        }

        totalPriceText.text = $"${totalPrice:N0}";
        totalPriceText.ForceMeshUpdate();

        // 購物車為空時禁用結帳按鈕，反之
        buyButton.interactable = totalPrice > 0;
    }

    public void CheckBill() // 按下結帳按鈕以處理購物車訂單
    {
        ResourceManager resourceManager = ResourceManager.Instance;

        // 檢查例外狀況
        if (resourceManager.getMoney() < totalPrice)
        {
            // 執行總價文字的閃爍效果
            if (totalPriceText.TryGetComponent<UIFlashEffect>(out var flash))
            {
                flash.Flash();
            }

            // 播放錯誤音效
            AudioManager.Instance.PlaySFX(failToBuySound);

            Debug.Log("玩家持有金額不足，無法結帳");
            return;
        }

        // 將購買的商品交由 ResourceManager 管理（姑且與背包對接了，但不曉得效果好不好）
        List<Item> itemsToAdd = new(); // 以清單進行統整
        foreach (CartItemData value in cartData.Values)
        {
            int buyCount = Mathf.Min(value.quantity, value.productToBuy.currentStack);
            for (int i = 0; i < buyCount; i++)
            {
                itemsToAdd.Add(value.productToBuy.product.item);
            }
            value.productToBuy.currentStack -= buyCount;
        }
        resourceManager.AddItem(itemsToAdd);

        // 處理金錢流向
        resourceManager.SpendMoney(totalPrice);

        // 播放結帳音效
        AudioManager.Instance.PlaySFX(checkBillSound);

        // 清空購物車
        foreach (Transform child in cartContent.transform)
        {
            Destroy(child.gameObject); // 刪掉購物車項目 UI
        }
        cartData.Clear();
        cartItemUIDict.Clear();

        // 統一廣播外部 UI 更新及背包對接（尚未完成）事件
        UpdateTotalPrice();
        OnPurchaseSuccess?.Invoke();
    }
}
