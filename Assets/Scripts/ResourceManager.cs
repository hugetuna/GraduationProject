using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceManager : MonoBehaviour
{
    public int Money;
    public float MoneyBonus=1f;//賺錢倍率
    public BondData bondAB;//列表中，A與B的羈絆值，以下同
    public BondData bondBC;
    public BondData bondCA;
    public List<IdolInstance> idolsPicked;//選進隊伍的三名偶像
    public List<Item> items;
    //每天結束時必須重製資源暫時狀態
    public void resetTemporaryEffect()
    {
        MoneyBonus = 1f;
    }
}
