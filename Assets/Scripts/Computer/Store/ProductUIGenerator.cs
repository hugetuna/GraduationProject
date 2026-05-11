using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 放在商店視窗根部以生成不同分類 ScrollView 中的商品們 */
public class ProductUIGenerator : MonoBehaviour
{
    [Header("商品資料")]
    // 目前沒有任何地方能獲取商品清單之類的
    public List<Product> productList = new(); // 儲存商品資訊的清單（道具包含在商品資料裡）
    private List<ProductRuntime> inventoryList = new(); // 商店的商品實體清單（包含動態資料）
    //-----------------------------------------------------------------//
    public GameObject productPrefab; // 用於生成商品項目的預製件
    public List<Transform> productContent = new(); // 用於放置生成的商品物件的容器

    void Start()
    {
        // 從無處獲取商品清單
        // 替商品清單產生動態資料
        // 如果 GameManager 裡已經有資料，就用它的
        if (GameManager.Instance.productSaveData.products != null &&
            GameManager.Instance.productSaveData.products.Count > 0)
        {
            // Debug.Log("使用已儲存的商品資料");
            inventoryList = GameManager.Instance.productSaveData.products;
        }
        else
        {
            // Debug.Log("初始化商品資料");
            inventoryList.Clear();
            foreach (Product product in productList)
            {
                ProductRuntime newProductRuntime = new(product);
                inventoryList.Add(newProductRuntime);

                // 立即同步回 GameManager
                GameManager.Instance.SaveProductData(newProductRuntime);
            }
        }

        //

        // 按清單生成初始的商品項目
        foreach (ProductRuntime productRuntime in inventoryList)
        {
            // 生成商品並分類...目前有「體力補品、訓練增益、好運配件、服裝裝備」等大致分類
            GameObject productObject = null;
            string itemName = productRuntime.product.item.itemName;
            var itemType = productRuntime.product.item.itemType;
            if (itemName.Contains("體力"))
            {
                productObject = Instantiate(productPrefab, productContent[0]); // "Wrapper" + Card
            }
            else if (itemName.Contains("舞蹈") || itemName.Contains("表現") || itemName.Contains("歌唱"))
            {
                productObject = Instantiate(productPrefab, productContent[1]); // "Wrapper" + Card
            }
            else if (itemName.Contains("御守") || itemName.Contains("香水"))
            {
                productObject = Instantiate(productPrefab, productContent[2]); // "Wrapper" + Card
            }
            else if (itemType == ItemType.Equipment)
            {
                productObject = Instantiate(productPrefab, productContent[3]); // "Wrapper" + Card
            }

            if (productObject == null)
            {
                Debug.Log("商品卡片生成失敗！");
                continue;
            }

            GameObject card = productObject.transform.Find("Card").gameObject; // Wrapper + "Card"
            // 設定商品卡片的 UI 資料
            SetProductUI setProductUI = card.GetComponent<SetProductUI>();
            setProductUI.Initialize(productRuntime);
        }
    }
}

[System.Serializable]
public class ProductSaveData
{
    public List<ProductRuntime> products = new();
}
