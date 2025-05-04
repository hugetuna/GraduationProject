using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<SoilSaveData> soilDataList = new List<SoilSaveData>();
    //單例物件生成
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    //土地專用儲存
    public void SaveSoilData(List<Soil> soils)
    {
        soilDataList.Clear();
        foreach (var soil in soils)
        {
            var data = new SoilSaveData
            {
                position = soil.transform.position,
                isPlantable = soil.isPlantable,
                isPlanting = soil.isPlanting
            };
            if (soil.seedOnThisSoil != null)
            {
                data.plantedSeedName = soil.seedOnThisSoil.seedData.seedName;
                data.isWatered = soil.seedOnThisSoil.getIsWateredToday();
                data.daysGrown = soil.seedOnThisSoil.GetDaysGrown();
                data.currentRewardPoint = soil.seedOnThisSoil.getRewardPoint();
            }
            soilDataList.Add(data);
        }
    }
}

