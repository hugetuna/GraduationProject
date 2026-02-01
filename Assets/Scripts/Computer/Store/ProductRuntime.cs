using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductRuntime
{
    public Product product; // 商品的靜態資料
    public int currentStack; // 當前的庫存（會變動的數字放這裡）
    public float currentDiscount = 1.0f; // 當前打折倍率（可用來判斷該商品是否特價）

    public ProductRuntime(Product product) // 建構元
    {
        this.product = product;
        currentStack = product.maxStack;
    }
}
