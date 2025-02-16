using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoulSeed", menuName = "Soul System/Soul Seed")]
public class SoulSeed : ScriptableObject
{
    public string seedName;  // 種子名稱
    public int growthDays;   // 成長周期 (1/2/4/6天)
    public RewardLevel rewardLevel;  // 初始獎勵等級
}

public enum RewardLevel
{
    dead,//死亡
    Low,    // 低
    Medium, // 中
    High,   // 高
    Special // 特殊
}
