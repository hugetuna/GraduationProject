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
    //-----------------------------------------------------------------//
    public GameObject productPrefab; // 用於生成商品項目的預製件
    public List<Transform> productContent = new(); // 用於放置生成的商品物件的容器

    void Start()
    {
        // 從無處獲取商品清單
        foreach (Product product in productList) // 按清單生成初始的商品項目
        {
            // 生成商品並分類...然而現在還沒有分類
            GameObject productObject = Instantiate(productPrefab, productContent[0]); // "Wrapper" + Card
            if (productObject == null)
            {
                Debug.Log("商品卡片生成失敗！");
                continue;
            }

            GameObject card = productObject.transform.Find("Card").gameObject; // Wrapper + "Card"
            // 設定商品卡片的 UI 資料
            SetProductUI setProductUI = card.GetComponent<SetProductUI>();
            setProductUI.Initialize(product);
        }
    }
}
