using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimalFarmSaveData
{
    public FarmLV farmLV;
    public bool isActivated;//是否已經開啟這個農場的使用權
    public int maxSeedAmount;//最大種植數量
    public int foodBarn;//食物欄位
    public int foodBarnMax;//食物欄位上限
}

