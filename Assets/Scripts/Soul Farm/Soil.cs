using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Soil : MonoBehaviour, IInteractable
{
    public bool isPlantable = false; // 是否可以種植
    public bool isPlanting = false;//是否在種植中
    public Transform seedSpawnPoint; // 種子的生成位置
    public float yOffSet=1.5f;
    public GameObject[] seedPrefabs; // 儲存不同種類的種子預製體
    public SeedInstanceScript seedOnThisSoil;//儲存一個被種植的種子的副本
    public OrderSet orderSeter;
    //根據名子找到public GameObject[] seedPrefabs;的預製件
    public int findSeedIndex(string name)
    {
        int i = 0;
        foreach (var seedPrefab in seedPrefabs)
        {
            if (name == seedPrefab.GetComponent<SeedInstanceScript>().seedData.name)
            {
                return i;
            }
            i++;
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
    //種一個種子
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
        // 在指定位置生成種子
        Vector3 realSpawnPoint = seedSpawnPoint.position + new Vector3(0, yOffSet, 0);
        Quaternion rotation = Quaternion.Euler(45f, 0f, 0f);
        seedOnThisSoil =Instantiate(seedPrefabs[seedIndex], realSpawnPoint, rotation).GetComponent<SeedInstanceScript>();
        seedOnThisSoil.transform.position += new Vector3(0, 0, 0.5f);
        seedOnThisSoil.GetComponent<SortingGroup>().sortingOrder= Mathf.RoundToInt(-transform.position.z * 100)+10;
        //改大小
        //seedOnThisSoil.transform.localScale = seedOnThisSoil.transform.localScale * 0.5f;
        isPlantable = false; // 標記這塊土地已經被種植
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
        else if (isPlanting == true && isPlantable == false && seedOnThisSoil.GetDaysGrown() == seedOnThisSoil.seedData.growthDays)
        {
            seedOnThisSoil.GetComponent<SeedInstanceScript>().Harvest();
            isPlanting = false;
            isPlantable = false;
        }
    } 
}
