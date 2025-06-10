using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<SoilSaveData> soilDataList = new List<SoilSaveData>();
    public List<IdolSaveData> idolDataList = new List<IdolSaveData>();
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
    //偶像專用儲存
    public void SaveIdolData(List<IdolInstance> idols)
    {
        idolDataList.Clear();
        foreach (var idol in idols)
        {
            var data = new IdolSaveData
            {
                idolIndex=idol.idolIndex,
                vocal = idol.vocal,
                dance = idol.dance,
                visual = idol.visual,
                voTrainingBonus = idol.voTrainingBonus,
                daTrainingBonus = idol.daTrainingBonus,
                viTrainingBonus = idol.viTrainingBonus,
                charm = idol.charm,
                charmInCount=idol.charmInCount,
                performance=idol.performance,
                vigour= idol.vigour,
                vigourMax=idol.vigourMax,
                fans= idol.fans,
                bondWithP= idol.bondWithP,
                BHaveSetUp=true
            };
            idolDataList.Add(data);
        }
    }
}

