using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Soil : MonoBehaviour, IInteractable
{
    public bool isPlantable = false; // 是否可以種植
    public bool isPlanting = false;//是否在種植中
    public Transform seedSpawnPoint; // 種子的生成位置
    public float yOffSet=0f;
    public GameObject[] seedPrefabs; // 儲存不同種類的種子預製體
    public SeedInstanceScript seedOnThisSoil;//儲存一個被種植的種子的副本
    public OrderSet orderSeter;
    //與manager連結以獲取數據
    public TeamManager teamManager;
    public ResourceManager resourceManager;
    public SoilManager soilManager;
    private void Start()
    {
        teamManager=FindObjectOfType<TeamManager>();
        resourceManager = FindObjectOfType<ResourceManager>();
        soilManager = FindObjectOfType<SoilManager>();
    }
    //根據名子找到public GameObject[] seedPrefabs;的預製件
    public int FindSeedIndex(string name)
    {
        string extendName = name + "Basic";//延長名字以符合列表
        for (int i = 0;i<seedPrefabs.Length;i++)
        {
            
            if (extendName == seedPrefabs[i].GetComponent<SeedInstanceScript>().seedData.name)
            {
                return i;
            }
        }
        return 0;
    }
    //翻土
    public void TurnTheSoil()
    {
        if(isPlanting == false && isPlantable == false)
        {
            isPlantable = true;
            Debug.Log("翻土!");
        }
        else
        {
            Debug.Log("此地塊已翻過土或正在種植");
        }
    }
    //種植個種子
    public void PlantSeed(int seedIndex)
    {
        if (!isPlantable)
        {
            Debug.Log("這塊土地還不能種植！");
            return;
        }
        if (seedIndex < 0 || seedIndex >= seedPrefabs.Length)
        {
            Debug.Log("無效的種子種類！");
            return;
        }

        // 正確的生成位置（僅加 y 偏移，不動 z）
        //Vector3 realSpawnPoint = seedSpawnPoint.position + new Vector3(0, yOffSet, 0);
        Quaternion rotation = Quaternion.Euler(45f, 0f, 0f);

        // 生成並初始化種子
        seedOnThisSoil = Instantiate(seedPrefabs[seedIndex], seedSpawnPoint.position, rotation).GetComponent<SeedInstanceScript>();

        // 移除父物件避免不必要的繼承偏移
        seedOnThisSoil.transform.SetParent(null);

        // 設定圖層順序，z 值保持不變
        seedOnThisSoil.GetComponent<SortingGroup>().sortingOrder = Mathf.RoundToInt(-transform.position.z * 100) + 10;

        // 完成種植狀態
        isPlantable = false;
        isPlanting = true;
    }

    void IInteractable.Interact(int toolType) // 互動行為
    {
        //TODO:追加工具反映
        //未處於種植狀態且不可種植->翻土(使用鋤頭tool==1)
        if(isPlanting == false && isPlantable == false && toolType==1)
        {
            TurnTheSoil();
        }
        //未處於種植狀態且可種植->種植
        else if (isPlanting == false && isPlantable == true)
        {
            PlantSeed(1); 
        }
        //種植中，但成長未滿->澆水
        else if (isPlanting == true && isPlantable == false && seedOnThisSoil.GetDaysGrown()< seedOnThisSoil.seedData.growthDays)
        {
            seedOnThisSoil.GetComponent<SeedInstanceScript>().Water();
        }
        //種植中，但成長以滿->收割
        else if (isPlanting == true && isPlantable == false && seedOnThisSoil.GetDaysGrown() >= seedOnThisSoil.seedData.growthDays)
        {
            //從teammanager抓隊長，把種出來的粉絲填入收割者，然後再塞進道具庫
            IdolInstance leader=teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
            int seedRewardPoint = seedOnThisSoil.GetComponent<SeedInstanceScript>().Harvest();
            //最終值算法(暫定)->種植值+魅力-80~種植值+魅力+30
            int finalSeedRewardPoint = Random.Range(seedRewardPoint - 80 + leader.charm, seedRewardPoint + 30 + leader.charm);
            Debug.Log(finalSeedRewardPoint);
            resourceManager.AddItem(soilManager.RollFansItem(finalSeedRewardPoint, leader.idolIndex));
            isPlanting = false;
            isPlantable = false;
            Destroy(seedOnThisSoil.gameObject);
        }
    } 
}
