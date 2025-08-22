using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProductData", menuName = "Computer/ProductData")]
public class ProductData : ScriptableObject
{
    public string productName;
    public Sprite productImage;
    public int productPrice;

    // 還不知道怎麼處理打折資訊...

}
