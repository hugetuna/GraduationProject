using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 單筆購物車資料
[System.Serializable]
public class CartItemData
{
    public Product productToBuy;
    public int qty;

    public CartItemData(Product product, int qty = 1)
    {
        this.productToBuy = product;
        this.qty = qty;
    }
}


/* 預計掛在商店視窗的 Sidebar 上（Singleton）*/
// 控制整個購物車
public class CartController : MonoBehaviour
{
    public static CartController Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [Header("購物車的 UI 設定")]
    [SerializeField] private Transform cartContent; // 購物車容器
    [SerializeField] private GameObject cartItemPrefab; // 購物車項目 Prefab
    [SerializeField] private TextMeshProUGUI totalPriceText; // 總價文字
    [SerializeField] private Button buyButton; // 購物車結帳按鈕
    //-----------------------------------------------------------------//
    private Dictionary<Product, CartItemData> cartData = new(); // 存放商品與其數量
    private Dictionary<Product, SetCartItemUI> cartItemUIDict = new(); // 存放商品與其 UI 控制器
    //-----------------------------------------------------------------//
    [Header("結帳功能設定")]
    private int totalPrice = 0; // 購物車總價
    [SerializeField] private TextMeshProUGUI moneyText; // 玩家持有金錢文字
    public AudioClip checkBillSound;
    private AudioSource audioSource;
    public static event Action OnPurchaseSuccess; // 購買成功的事件



    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例

        audioSource = gameObject.AddComponent<AudioSource>();
        // audioSource.clip = checkBillSound; 只有使用 audioSource.Play() 才須設定
    }

    void Start()
    {
        buyButton.onClick.AddListener(CheckBill);

        UpdateTotalPrice(); // 確保初始總價正確
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null; // 清除單一實例
    }

    public void AddToCart(Product product)
    {
        if (cartData.ContainsKey(product))
        {
            // 若該購物車項目已存在，增加其購買數量
            cartData[product].qty++;
            if(cartData[product].qty > product.stack) // 若購買數量超過庫存
            {
                Debug.Log("已達庫存上限，無法再增加購買數量");
                cartData[product].qty = product.stack; // 限制購買數量為庫存數量
            }

            cartItemUIDict[product].UpdateCartQuantity(cartData[product].qty);   
        }
        else
        {
            // 若該購物車項目不存在，則新增一個
            CartItemData cartItemData = new(product);
            cartData[product] = cartItemData; // 記錄商品與其數量

            GameObject cartObject = Instantiate(cartItemPrefab, cartContent); // 生成購物車項目
            GameObject card = cartObject.transform.Find("Card").gameObject; // Wrapper + Card
            SetCartItemUI setCartItemUI = card.GetComponent<SetCartItemUI>();

            setCartItemUI.Initialize(product, cartItemData.qty); // 傳遞商品資料與數量
            setCartItemUI.SetController(this); // 綁定個別購物車項目與整個購物車
            cartItemUIDict[product] = setCartItemUI; // 記錄商品與其 UI 控制器
        }

        UpdateTotalPrice();
    }

    public void ReduceQuantity(Product product)
    {
        if (!cartData.ContainsKey(product)) return;

        cartData[product].qty--;

        if (cartData[product].qty <= 0)
        {
            Destroy(cartItemUIDict[product].transform.parent.gameObject); // Wrapper + Card
            cartData.Remove(product);
            cartItemUIDict.Remove(product);
        }
        else
        {
            cartItemUIDict[product].UpdateCartQuantity(cartData[product].qty);
        }

        UpdateTotalPrice();
    }


    private void UpdateTotalPrice()
    {
        totalPrice = 0;
        foreach (var data in cartData.Values)
        {
            totalPrice += data.productToBuy.price * data.qty;
        }

        totalPriceText.text = $"${totalPrice:N0}";
        totalPriceText.ForceMeshUpdate();

        // 購物車為空時禁用結帳按鈕，反之
        buyButton.interactable = totalPrice > 0;
    }

    public void CheckBill() // 按下結帳按鈕以處理購物車訂單
    {
        ResourceManager resourceManager = WindowDataSetup.GetResourceManager();

        // 檢查例外狀況
        if (resourceManager.getMoney() < totalPrice)
        {
            Debug.Log("玩家持有金額不足，無法結帳");
            return;
        }

        // 將購買的商品交由 ResourceManager 管理（尚未與背包對接）
        List<Item> itemsToAdd = new(); // 以清單進行統整
        foreach (CartItemData value in cartData.Values)
        {
            int buyCount = Mathf.Min(value.qty, value.productToBuy.stack);
            for (int i = 0; i < buyCount; i++)
            {
                itemsToAdd.Add(value.productToBuy.item);
            }
            value.productToBuy.stack -= buyCount;
        }
        resourceManager.AddItem(itemsToAdd);

        // 處理金錢流向
        resourceManager.SpendMoney(totalPrice);

        // 播放結帳音效
        audioSource.PlayOneShot(checkBillSound);

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
