using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoulSeed", menuName = "Soul System/Soul Seed")]
public class SoulSeed : ScriptableObject
{
    public string seedName;  // �ؤl�W��
    public int growthDays;   // �����P�� (1/3/5/6��)
    public int rewardPoint;  // ��l���y����
    public int rewardPointMax;
    public int rewardPointMin;//�̷Ӻؤl���O���t�̤j�̤p��
    public int wateredMinus;//�S����|�U������
    public int rewardPointGate;//�϶��έ�
}

