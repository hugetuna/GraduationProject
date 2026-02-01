using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Product", menuName = "Computer/Product")]
public class Product : ScriptableObject
{
    public Item item;
    public int price; // 商品價格
    public int maxStack; // 最大庫存數量
}

