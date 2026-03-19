using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Baito", menuName = "Go Out/Baito")]
public class Baito : ScriptableObject
{
    public string baitoName; // 打工類型
    public string description; // 打工描述
    public int vigourCost; // 打工耗體
    public int MoneyGain; // 打工的金錢收益
    
    // 額外收益先不寫
}
