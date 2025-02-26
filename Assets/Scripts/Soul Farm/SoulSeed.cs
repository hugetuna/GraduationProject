using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoulSeed", menuName = "Soul System/Soul Seed")]
public class SoulSeed : ScriptableObject
{
    public string seedName;  // 種子名稱
    public int growthDays;   // 成長周期 (1/3/5/6天)
    public int rewardPoint;  // 初始獎勵分數
    public int rewardPointMax;
    public int rewardPointMin;//依照種子類別分配最大最小值
    public int wateredMinus;//沒澆水會下降此值
    public int rewardPointGate;//區間用值
}

