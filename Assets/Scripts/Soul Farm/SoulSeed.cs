using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoulSeed", menuName = "Soul System/Soul Seed")]
public class SoulSeed : ScriptableObject
{
    public string seedName;  // �ؤl�W��
    public int growthDays;   // �����P�� (1/2/4/6��)
    public RewardLevel rewardLevel;  // ��l���y����
}

public enum RewardLevel
{
    dead,//���`
    Low,    // �C
    Medium, // ��
    High,   // ��
    Special // �S��
}
