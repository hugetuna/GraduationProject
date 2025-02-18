using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] // 讓它可被 Unity 序列化（Inspector 可調整數值）
public class BondData//用以儲存羈絆值的類
{
    public IdolInstance idolA;  // 角色A
    public IdolInstance idolB;  // 角色B
    [Range(0, 100)] public int bondValue; // 羈絆值 (0~100)
}
