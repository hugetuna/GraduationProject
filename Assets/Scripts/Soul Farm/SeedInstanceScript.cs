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
    public List<GameObject> growthStages; // 成長過程的圖像(我沒有設計防呆，請記得目前只能塞三種)
    public SpriteRenderer spriteRenderer; // 用來顯示圖片的組件

    void Start()
    {
        VisualUpdate();
        currentRewardPoint = seedData.rewardPoint;
    }
    
    public void Grown(int days)//成長
    {
        daysGrown += days;
        VisualUpdate();
    }
    public int GetDaysGrown()
    {
        return daysGrown;
    }
    private void VisualUpdate()
    {
        for (int i = 0; i < growthStages.Count; i++)
        {
            growthStages[i].SetActive(false); // 先關掉所有
        }
        if (daysGrown == 0)
        {
            growthStages[0].SetActive(true); // 顯示幼苗階段
        }
        else if (daysGrown >= seedData.growthDays)
        {
            growthStages[2].SetActive(true); // 顯示成熟階段
        }
        else
        {
            growthStages[1].SetActive(true); // 顯示中期階段
        }
    }

    [ContextMenu("water")]
    public void Water()//澆水
    {
        wateredToday = true;
        Debug.Log($"{seedData.seedName} 已澆水");
    }
    public bool getIsWateredToday()
    {
        return wateredToday;
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
    public int getRewardPoint()
    {
        return currentRewardPoint;
    }
    public void setRewardPoint(int value)
    {
        currentRewardPoint=value;
    }
    public void Harvest()
    {
        Debug.Log($"{seedData.seedName} 成熟了！獎勵等級: {currentRewardPoint}");
        // 呼叫獎勵系統來抽選獎勵
        Destroy(gameObject);
    }
}
