using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedInstanceScript : MonoBehaviour
{
    public SoulSeed seedData;  // 參考 ScriptableObject
    [SerializeField] private int daysGrown = 0; // 已經成長的天數
    [SerializeField] private bool wateredToday = false;//今天澆水了沒
    [SerializeField] private int currentRewardPoint;
    //視覺管理
    public List<Sprite> growthSprites; // 成長過程的圖像(我沒有設計防呆，請記得目前只能塞三種)
    public SpriteRenderer spriteRenderer; // 用來顯示圖片的組件

    void Start()
    {
        if (growthSprites.Count > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = growthSprites[0]; // 初始顯示幼苗
        }
        currentRewardPoint = seedData.rewardPoint;
    }
    
    public void Grown(int days)//成長
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
    public void Water()//澆水
    {
        wateredToday = true;
        Debug.Log($"{seedData.seedName} 已澆水");
    }
    private void CheckIsWatered()//檢查是否澆水，若沒澆則獎勵變差
    {
        //根據澆水情況變動
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
    public void EndOfDay()//一天結束
    {
        Grown(1);
        CheckIsWatered();
        CheckIsDead();
    }
    public void Harvest()
    {
        Debug.Log($"{seedData.seedName} 成熟了！獎勵等級: {currentRewardPoint}");
        // 呼叫獎勵系統來抽選獎勵
        Destroy(gameObject);
    }
}
