using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    public GameObject soilPrefab;
    public GameObject[] seedPrefabs; // 所有種子 Prefab
    //粉絲轉蛋獎池
    public List<HarvestGachaPool> harvestGachaPools;
    //所有農場的列表
    public List<AnimalFarm> animalFarms;
    void Start()
    {
        SetupFarm();
        SetupAnimal();
    }
    public void SetupFarm()
    {
        foreach (var farm in GameManager.Instance.FarmsDataList)
        {
            if (animalFarms[(int)farm.farmLV] != null)
            {
                animalFarms[(int)farm.farmLV].isActivated = farm.isActivated;
                //animalFarms[(int)farm.farmLV].gameObject.SetActive(farm.isActivated);
                animalFarms[(int)farm.farmLV].maxSeedAmount = farm.maxSeedAmount;
                animalFarms[(int)farm.farmLV].foodBarn = farm.foodBarn;
                animalFarms[(int)farm.farmLV].foodBarnMax = farm.foodBarnMax;
            }
        }
    }
    public void SetupAnimal()
    {
        foreach (var animal in GameManager.Instance.animalDataList)
        {
            SeedInstanceScript_Animal plantedAnimal = animalFarms[(int)animal.farmLV].PlantSeed();
            if (animal.isWatered && plantedAnimal != null)
            {
                plantedAnimal.Water();
            }
            if (plantedAnimal != null)
            {
                plantedAnimal.SetRewardPoint(animal.currentRewardPoint);
                for (int i = 0; i < animal.daysGrown; i++)
                {
                    plantedAnimal.Grown(1);
                }
            }
            plantedAnimal?.CheckIsDead();
        }
    }
    public FansItem RollFansItem(int rewardPoint, IdolWho harvester)
    {
        foreach(var pool in harvestGachaPools)
        {
            if (rewardPoint >= pool.rewardPointLimitMin && rewardPoint <= pool.rewardPointLimitMax)
            {
                if (pool.gachaPool.Count == 0)
                {
                    Debug.LogError("轉蛋池裡沒東西");
                    return null;
                }
                int randomIndex = Random.Range(0, pool.gachaPool.Count);
                //使用 Clone 建立副本
                FansItem original = pool.gachaPool[randomIndex];
                FansItem copy = (FansItem)original.Clone();
                copy.SetHarvester(harvester);
                return copy;
            }
        }
        Debug.LogError("沒有轉出");
        return null;
    }
}

