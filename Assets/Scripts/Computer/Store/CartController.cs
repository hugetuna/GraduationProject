using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 單筆購物車資料
[System.Serializable]
public class CartItemData
{
    public Item item;
    public int qty;

    public CartItemData(Item item, int qty = 1)
    {
        this.item = item;
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
    private Dictionary<Item, CartItemData> cartData = new(); // 存放商品與其數量
    private Dictionary<Item, SetCartItemUI> cartItemUIDict = new(); // 存放商品與其 UI 控制器
    //-----------------------------------------------------------------//
    [Header("結帳功能設定")]
    private int totalPrice = 0; // 購物車總價
    [SerializeField] private TextMeshProUGUI moneyText; // 玩家持有金錢文字
    public AudioClip checkBillSound;
    private AudioSource audioSource;


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

    public void AddToCart(Item product)
    {
        if (cartData.ContainsKey(product))
        {
            // 若該購物車項目已存在，增加其購買數量
            cartData[product].qty++;
            cartItemUIDict[product].SetQuantity(cartData[product].qty);
        }
        else
        {
            // 若該購物車項目不存在，則新增一個
            CartItemData cartItemData = new(product);
            cartData[product] = cartItemData; // 記錄商品與其數量

            GameObject cartObject = Instantiate(cartItemPrefab, cartContent); // 生成購物車項目
            GameObject card = cartObject.transform.Find("Card").gameObject; // Wrapper + Card
            SetCartItemUI setCartItemUI = card.GetComponent<SetCartItemUI>();

            setCartItemUI.SetProduct(product, cartItemData.qty); // 傳遞商品資料與數量
            setCartItemUI.SetController(this); // 綁定個別購物車項目與整個購物車
            cartItemUIDict[product] = setCartItemUI; // 記錄商品與其 UI 控制器
        }

        UpdateTotalPrice();
    }

    public void ReduceQuantity(Item product)
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
            cartItemUIDict[product].SetQuantity(cartData[product].qty);
        }

        UpdateTotalPrice();
    }


    private void UpdateTotalPrice()
    {
        totalPrice = 0;
        foreach (var data in cartData.Values)
        {
            totalPrice += data.item.price * data.qty;
        }

        totalPriceText.text = $"${totalPrice}";
        totalPriceText.ForceMeshUpdate();
    }

    public void CheckBill() // 按下結帳按鈕以處理購物車訂單
    {
        if(totalPrice <= 0) return; // 購物車為空不處理

        // 將購買的商品交由 ResourceManager 管理（尚未與背包對接）
        ResourceManager resourceManager = WindowDataSetup.GetResourceManager();

        List<Item> itemsToAdd = new(); // 以清單進行統整
        foreach (var product in cartData.Values)
        {
            for (int i = 0; i < product.qty; i++) itemsToAdd.Add(product.item);
        }
        resourceManager.AddItem(itemsToAdd);

        // 處理金錢流向
        resourceManager.SpendMoney(totalPrice);
        MoneyUsage moneyUsage = moneyText.GetComponent<MoneyUsage>();
        moneyUsage.UpdateMoneyText(); // 反映到商店視窗的玩家金錢上

        // 播放結帳音效
        audioSource.PlayOneShot(checkBillSound);

        // 清空購物車
        foreach (Transform child in cartContent.transform)
        {
            Destroy(child.gameObject); // 刪掉購物車項目 UI
        }
        cartData.Clear();
        cartItemUIDict.Clear();

        UpdateTotalPrice();
    }
}
