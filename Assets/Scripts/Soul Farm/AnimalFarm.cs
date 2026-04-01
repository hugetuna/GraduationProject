using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum FarmLV
{
    Low,
    Medium,
    High
}
public class AnimalFarm : MonoBehaviour, IInteractable
{
    public string InteractionKey => "TGrow"; // 這個字串用來指定動畫 key

    public FarmLV farmLV;
    public Transform seedSpawnPoint; // 種子的生成位置
    public GameObject[] seedPrefabs; // 儲存不同種類的種子預製體
    
    public OrderSet orderSeter;
    [Header("種田數值紀錄")]
    public List<SeedInstanceScript> seedsOnThisSoil;//紀錄所有被種植的種子
    public int maxSeedAmount=3;//最大種植數量
    public int foodBarn=0;//食物欄位
    public int foodBarnMax=0;//食物欄位上限
    [Header("介面")]
    public Canvas farmCanvas;//互動按鈕介面
    //public Button plantSeedButton;
    //public Button addFoodBarnButton;
    //public Button harvestSeedButton;
    [Header("Manager")]
    public TeamManager teamManager;
    public ResourceManager resourceManager;
    public SoilManager soilManager;
    [Header("種田相關音效")]
    public AudioClip audio_PlantSeed;
    public AudioClip audio_WaterSeed;
    public AudioClip audio_HarvestSeed;
    private void Start()
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        resourceManager = FindAnyObjectByType<ResourceManager>();
        soilManager = FindAnyObjectByType<SoilManager>();
    }
    //種植個種子
    public void PlantSeed()
    {
        if (seedsOnThisSoil.Count < maxSeedAmount)
        {
            //生成種子
            Quaternion rotation = Quaternion.Euler(45f, 0f, 0f);//設定種子生成的旋轉角度
            GameObject newSeed = Instantiate(seedPrefabs[(int)farmLV], seedSpawnPoint.position, rotation);
            seedsOnThisSoil.Add(newSeed.GetComponent<SeedInstanceScript>());
            //消耗體力
            IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
            leader.costVigour(leader.plantVigourCost);
        }
        else
        {
            Debug.Log("此農場已達最大種植數量");
        }
    }
    //補充食物欄位
    public void AddFoodBarn(int amount)
    {
        if (foodBarn + amount <= foodBarnMax)
        {
            foodBarn += amount;
            //消耗體力
            IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
            leader.costVigour(leader.waterVigourCost);
        }
        else
        {
            foodBarn = foodBarnMax;
            Debug.Log("食物欄位已滿");
        }
    }
    //種子消耗食物欄位(每天結束時)
    public void SeedsConsumeBarn()
    {
        if (foodBarn > 0)
        {
            foreach (SeedInstanceScript seed in seedsOnThisSoil)
            {
                if (!seed.GetIsWateredToday())
                {
                    seed.Water();
                    foodBarn--;
                }
            }
        }
        else
        {
            Debug.Log("食物欄位沒有食物了");
        }
    }
    //Harvest種子
    public void HarvestSeed()
    {
        foreach (SeedInstanceScript seed in seedsOnThisSoil)
        {
            if (seed.GetDaysGrown()>=seed.seedData.growthDays)
            {
                IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
                //從teammanager抓隊長，把種出來的粉絲填入收割者，然後再塞進道具庫
                int seedRewardPoint = seed.GetComponent<SeedInstanceScript>().Harvest();
                //最終值算法(暫定)->種植值+魅力-80~種植值+魅力+30
                int finalSeedRewardPoint = Random.Range(seedRewardPoint - 80 + leader.charm, seedRewardPoint + 30 + leader.charm);
                Debug.Log(finalSeedRewardPoint);
                FansItem newFan = soilManager.RollFansItem(finalSeedRewardPoint, leader.idolIndex);
                resourceManager.AddItem(newFan);
                //播放收割音效
                AudioManager.Instance.PlaySFX(audio_HarvestSeed);
                //刪除種子物件
                Destroy(seed.gameObject);
                break; // 只收割一個種子
            }
        }
        Debug.Log("沒有可以收割的種子");
    }
    void IInteractable.Interact(int toolType) // 互動行為
    {
        
    }
    
}
