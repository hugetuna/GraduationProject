using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedInstanceScript : MonoBehaviour
{
    public SoulSeed seedData;  // �Ѧ� ScriptableObject
    private int daysGrown = 0; // �w�g�������Ѽ�
    private bool wateredToday = false;
    private RewardLevel currentRewardLevel;
    //��ı�޲z
    public List<Sprite> growthSprites; // �����L�{���Ϲ�(�ڨS���]�p���b�A�аO�o�ثe�u���T��)
    public SpriteRenderer spriteRenderer; // �Ψ���ܹϤ����ե�

    void Start()
    {
        if (growthSprites.Count > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = growthSprites[0]; // ��l��ܥ��]
        }
        currentRewardLevel = seedData.rewardLevel;//�]�m��l���y����
    }
    
    public void Grown(int days)//����
    {
        daysGrown += days;
        spriteChange();
    }
    private void spriteChange()
    {
        if (daysGrown == 0)
        {
            spriteRenderer.sprite = growthSprites[0];
        }
        else if (daysGrown == seedData.growthDays)
        {
            spriteRenderer.sprite = growthSprites[2];
        }
        else
        {
            spriteRenderer.sprite = growthSprites[1];
        }
    }
    public void Water()//���
    {
        wateredToday = true;
        Debug.Log($"{seedData.seedName} �w���");
    }
    private void CheckIsWatered()//�ˬd�O�_����A�Y�S��h���y�ܮt
    {
        if (!wateredToday)
        {
            RewardChange(-1);
        }
    }
    public void RewardChange(int amount)//���ܼ��y����
    {
        if(currentRewardLevel>0)
        {
            currentRewardLevel += amount;
        }
    }
    [ContextMenu("end a day test")]
    public void EndOfDay()//�@�ѵ���
    {
        Grown(1);
        CheckIsWatered();
    }
    private void Harvest()
    {
        Debug.Log($"{seedData.seedName} �����F�I���y����: {currentRewardLevel}");
        // �I�s���y�t�Ψө����y
        Destroy(gameObject);
    }
}
