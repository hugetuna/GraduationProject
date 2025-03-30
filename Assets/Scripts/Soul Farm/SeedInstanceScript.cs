using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedInstanceScript : MonoBehaviour
{
    public SoulSeed seedData;  // �Ѧ� ScriptableObject
    [SerializeField] private int daysGrown = 0; // �w�g�������Ѽ�
    [SerializeField] private bool wateredToday = false;//���Ѽ���F�S
    [SerializeField] private int currentRewardPoint;
    //��ı�޲z
    public List<Sprite> growthSprites; // �����L�{���Ϲ�(�ڨS���]�p���b�A�аO�o�ثe�u���T��)
    public SpriteRenderer spriteRenderer; // �Ψ���ܹϤ����ե�

    void Start()
    {
        if (growthSprites.Count > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = growthSprites[0]; // ��l��ܥ��]
        }
        currentRewardPoint = seedData.rewardPoint;
    }
    
    public void Grown(int days)//����
    {
        daysGrown += days;
        SpriteChange();
    }
    public int GetDaysGrown()
    {
        return daysGrown;
    }
    private void SpriteChange()
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
    [ContextMenu("water")]
    public void Water()//���
    {
        wateredToday = true;
        Debug.Log($"{seedData.seedName} �w���");
    }
    private void CheckIsWatered()//�ˬd�O�_����A�Y�S��h���y�ܮt
    {
        //�ھڼ�����p�ܰ�
        if (wateredToday == false) { 
            currentRewardPoint = currentRewardPoint - seedData.wateredMinus;
        }
        wateredToday = false;
    }
    private void CheckIsDead()
    {
        if (currentRewardPoint < seedData.rewardPointMin)
        {
            Destroy(gameObject);
        }
    }
    [ContextMenu("end a day test")]
    public void EndOfDay()//�@�ѵ���
    {
        Grown(1);
        CheckIsWatered();
        CheckIsDead();
    }
    public void Harvest()
    {
        Debug.Log($"{seedData.seedName} �����F�I���y����: {currentRewardPoint}");
        // �I�s���y�t�Ψө����y
        Destroy(gameObject);
    }
}
