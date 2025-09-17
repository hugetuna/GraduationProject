using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Product", menuName = "Computer/Product")]
public class Product : ScriptableObject
{
    public Item item;
    public int price; // 商品價格
    public float discount = 1.0f; // 打折倍率（可用來判斷該商品是否特價）
    public int stack; // 庫存數量
}

