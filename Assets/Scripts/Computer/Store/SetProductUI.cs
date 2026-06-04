using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/* 掛在商店視窗的商品卡片根部（不看 Wrapper）*/
public class SetProductUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("商品卡片的 UI 設定")]
    private ProductRuntime productRuntime;
    [SerializeField] private TextMeshProUGUI productNameText; // 商品名稱文字
    [SerializeField] private TextMeshProUGUI productPriceText; // 商品價格文字
    [SerializeField] private TextMeshProUGUI oldProductPriceText; // 商品原價文字（若有特價）
    [SerializeField] private Image productImage; // 商品圖片
    [SerializeField] private TextMeshProUGUI stackText; // 庫存數量文字
    [SerializeField] private Button cartButton; // 加入購物車的按鈕
    //-----------------------------------------------------------------//
    [Header("價格文字設定")]
    [SerializeField] private Color32 normalPriceColor = new(147, 99, 230, 255); // 正常價格顏色
    [SerializeField] private Color32 discountPriceColor = new(255, 75, 75, 255); // 特價價格
    [SerializeField] private GameObject discountDash; // 特價時的原價刪除線
    private float minDashLength = 19; // 原價刪除線的最小長度
    private float maxDashLength = 23; // 原價刪除線的最大長度
    //-----------------------------------------------------------------//
    [Header("Hover 資訊")]
    [SerializeField] private GameObject hoverObj; // Hover 時顯示的資訊物件
    [SerializeField] private TextMeshProUGUI hoverNameText; // Hover 時顯示的商品名稱文字
    [SerializeField] private TextMeshProUGUI hoverEffectText; // Hover 時顯示的商品描述文字
    [SerializeField] private TextMeshProUGUI hoverNumText; // Hover 時顯示的商品持有數量文字
    //-----------------------------------------------------------------//
    private CartController cartController;

    void Start()
    {
        CartController.OnPurchaseSuccess += UpdateStackText; // 訂閱結帳事件
    }

    void OnDestroy()
    {
        CartController.OnPurchaseSuccess -= UpdateStackText; // 取消訂閱結帳事件
    }

    public void Initialize(ProductRuntime newProduct)
    {
        // 設定商品資料
        productRuntime = newProduct;
        Product product = productRuntime.product;

        // 設定購物車控制器
        cartController = GetComponentInParent<CartController>();

        // 設定 UI 顯示
        productImage.sprite = product.item.icon;
        productNameText.text = product.item.itemName;

        // 設定 hover 資訊
        hoverNameText.text = product.item.itemName;
        hoverEffectText.text = product.item.description;
        int ownedQty = ResourceManager.Instance.GetItemCount(product.item);
        hoverNumText.text = $"已持有數量: {ownedQty}";

        // 初始庫存數量設為最大庫存數 -> 已內建於 ProductRuntime 類別
        UpdateStackText();

        // 根據是否特價來調整價格顯示
        float discount = productRuntime.currentDiscount;
        if (discount == 1.0f) // 原價
        {
            productPriceText.text = $"${product.price}";
            productPriceText.color = normalPriceColor;
            oldProductPriceText.gameObject.SetActive(false);
            discountDash.SetActive(false);
        }
        else // 特價
        {
            productPriceText.text = $"${(int)(product.price * discount)}";
            productPriceText.color = discountPriceColor;
            oldProductPriceText.text = $"${product.price}";
            oldProductPriceText.gameObject.SetActive(true);
            discountDash.SetActive(true);

            // 調整刪除線長度
            if (oldProductPriceText.text.Length <= 3) // 價格字數少時
            {
                discountDash.GetComponent<RectTransform>().sizeDelta = new Vector2(minDashLength, discountDash.GetComponent<RectTransform>().sizeDelta.y);
            }
            else // 價格字數多時
            {
                discountDash.GetComponent<RectTransform>().sizeDelta = new Vector2(maxDashLength, discountDash.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        // 確保字型正確渲染
        productNameText.ForceMeshUpdate();
        productPriceText.ForceMeshUpdate();
        stackText.ForceMeshUpdate();
        hoverNameText.ForceMeshUpdate();
        hoverEffectText.ForceMeshUpdate();
        hoverNumText.ForceMeshUpdate();

        // 為加入購物車的按鈕註冊事件
        cartButton.onClick.RemoveAllListeners(); // 確保不會重複註冊
        cartButton.onClick.AddListener(() =>
        {
            cartController.AddToCart(productRuntime);
        });
    }

    public void UpdateStackText()
    {
        stackText.text = $"庫存 {productRuntime.currentStack}";
        stackText.ForceMeshUpdate();

        if (productRuntime.currentStack == 0) cartButton.interactable = false;
        else cartButton.interactable = true;

        // 順便更新 Hover 的持有數量資訊
        int ownedQty = ResourceManager.Instance.GetItemCount(productRuntime.product.item);
        hoverNumText.text = $"已持有數量: {ownedQty}";
        hoverNumText.ForceMeshUpdate();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverObj.SetActive(false);
    }
}
