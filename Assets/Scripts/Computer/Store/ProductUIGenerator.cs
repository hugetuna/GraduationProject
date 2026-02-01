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
        // 替商品清單產生動態資料
        foreach (Product product in productList)
        {
            ProductRuntime newProductRuntime = new ProductRuntime(product);
            inventoryList.Add(newProductRuntime);
        }

        // 從無處獲取商品清單
        foreach (ProductRuntime productRuntime in inventoryList) // 按清單生成初始的商品項目
        {
            // 生成商品並分類...然而現在只有兩個分類（消耗品 vs. 裝備）
            GameObject productObject = null;
            var itemType = productRuntime.product.item.itemType;
            if(itemType == ItemType.Consumable)
            {
                productObject = Instantiate(productPrefab, productContent[0]); // "Wrapper" + Card
            }
            else if(itemType == ItemType.Equipment)
            {
                productObject = Instantiate(productPrefab, productContent[1]); // "Wrapper" + Card
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
