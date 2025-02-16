using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedInstanceScript : MonoBehaviour
{
    public SoulSeed seedData;  // 參考 ScriptableObject
    private int daysGrown = 0; // 已經成長的天數
    private bool wateredToday = false;
    private RewardLevel currentRewardLevel;
    //視覺管理
    public List<Sprite> growthSprites; // 成長過程的圖像(我沒有設計防呆，請記得目前只能塞三種)
    public SpriteRenderer spriteRenderer; // 用來顯示圖片的組件

    void Start()
    {
        if (growthSprites.Count > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = growthSprites[0]; // 初始顯示幼苗
        }
        currentRewardLevel = seedData.rewardLevel;//設置初始獎勵等級
    }
    
    public void Grown(int days)//成長
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
    public void Water()//澆水
    {
        wateredToday = true;
        Debug.Log($"{seedData.seedName} 已澆水");
    }
    private void CheckIsWatered()//檢查是否澆水，若沒澆則獎勵變差
    {
        if (!wateredToday)
        {
            RewardChange(-1);
        }
    }
    public void RewardChange(int amount)//改變獎勵等級
    {
        if(currentRewardLevel>0)
        {
            currentRewardLevel += amount;
        }
    }
    [ContextMenu("end a day test")]
    public void EndOfDay()//一天結束
    {
        Grown(1);
        CheckIsWatered();
    }
    private void Harvest()
    {
        Debug.Log($"{seedData.seedName} 成熟了！獎勵等級: {currentRewardLevel}");
        // 呼叫獎勵系統來抽選獎勵
        Destroy(gameObject);
    }
}
