using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedInstanceScript_Animal : MonoBehaviour
{
    public SoulSeed seedData;  // 參考 ScriptableObject
    [SerializeField] private int daysGrown = 0; // 已經成長的天數
    [SerializeField] private bool wateredToday = false;//今天澆水了沒
    [SerializeField] private int currentRewardPoint;
    //視覺管理
    public List<GameObject> growthStages; // 成長過程的圖像(我沒有設計防呆，請記得目前只能塞三種)
    public SpriteRenderer spriteRenderer; // 用來顯示圖片的組件
    [Header("種子實例的移動計時器")]
    public float moveTimer = 0f; // 移動計時器
    public float moveInterval = 5f; // 移動間隔時間
    public float moveSpeed = 10f; // 移動速度
    public Vector3 moveDirection=new Vector3(0,0,0); // 移動持續時間
    public bool isMoving = false; // 是否正在移動
    public OrderSet orderSeter;
    void Start()
    {
        VisualUpdate();
        currentRewardPoint = seedData.rewardPoint;
        //開始時運動一次
        moveTimer=moveInterval-0.2f; // 讓它一開始就能選擇方向移動
    }
    void Update()
    {
        orderSeter.UpdateSortingOrder();
        moveTimer += Time.deltaTime;
        if (moveTimer >= moveInterval)
        {
            if (isMoving==false) // 如果目前沒有正在移動，則選擇一個新的隨機方向
            {
                moveDirection = ChooseRandomDirection();
                isMoving = true;
            }
            Move(moveDirection, moveSpeed);
        }
        if (moveTimer >= moveInterval + 1f) // 移動持續1秒
        {
            moveTimer = 0f; // 重置計時器
            moveInterval=Random.Range(3f, 6f); // 隨機下一次移動的間隔時間
            isMoving = false;
        }
    }
    //漫遊邏輯
    //當碰撞到牆壁時，立刻重新計算一個隨機方向
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            // 撞到牆了，立刻重新計算一個隨機方向
            moveDirection = ChooseRandomDirection();
        }
    }
    //生成一個隨機方向
    public Vector3 ChooseRandomDirection()
    {
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        return randomDirection;
    }
    //移動
    public void Move(Vector3 direction, float speed)
    {
        if (direction.x > 0)
        {
            this.gameObject.transform.localScale = new Vector3(-1, 1, 1); // 向右移動，保持正常大小
        }
        else if (direction.x < 0)
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1); // 向左移動，翻轉圖片
        }
        transform.Translate(direction * speed * Time.deltaTime);
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
    public bool GetIsWateredToday()
    {
        return wateredToday;
    }
    private void CheckIsWatered()//檢查是否澆水，若沒澆則獎勵變差
    {
        //根據澆水情況變動
        if (wateredToday == false) { 
            currentRewardPoint -=seedData.wateredMinus;
        }
        wateredToday = false;
    }
    public void CheckIsDead()
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
    public int GetRewardPoint()
    {
        return currentRewardPoint;
    }
    public void SetRewardPoint(int value)
    {
        currentRewardPoint=value;
    }
    public int Harvest()
    {
        Debug.Log($"{seedData.seedName} 成熟了！獎勵等級: {currentRewardPoint}");
        // 呼叫獎勵系統來抽選獎勵
        return currentRewardPoint;
    }
}
