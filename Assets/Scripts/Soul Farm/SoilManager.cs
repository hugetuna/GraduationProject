using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    public GameObject soilPrefab;
    public GameObject[] seedPrefabs; // 所有種子 Prefab

    void Start()
    {
        foreach (var data in GameManager.Instance.soilDataList)
        {
            GameObject soilObj = Instantiate(soilPrefab, data.position, Quaternion.Euler(90f, 0f, 0f));
            Soil soil = soilObj.GetComponent<Soil>();
            //soil.isPlantable = data.isPlantable;
            //soil.isPlanting = data.isPlanting;
            soil.seedPrefabs = seedPrefabs;
            //如果於種植狀態且該土上的種子有名字
            if (data.isPlanting && !string.IsNullOrEmpty(data.plantedSeedName))
            {
                //遍歷傳入的矩陣，如果種子的等級和列表紀錄的相同，則回傳相應的索引值
                int index = soil.findSeedIndex(data.plantedSeedName);
                //在新生成的土上根據索引值重新種植物，並填入儲存於列表的資訊
                if (index >= 0)
                {
                    soil.TurnTheSoil();
                    soil.PlantSeed(index);
                    soil.seedOnThisSoil.Grown(data.daysGrown); // 因為 PlantSeed 已長一天
                    soil.seedOnThisSoil.setRewardPoint(data.currentRewardPoint);
                    if (data.isWatered)
                    {
                        soil.seedOnThisSoil.Water();
                    }
                }
            }
        }
    }
}

