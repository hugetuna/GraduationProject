using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : MonoBehaviour, IInteractable
{
    public bool isPlantable = false; // 是否可以種植
    public bool isPlanting = false;//是否在種植中
    public Transform seedSpawnPoint; // 種子的生成位置
    public GameObject[] seedPrefabs; // 儲存不同種類的種子預製體
    public GameObject seedOnThisSoil;//儲存一個被種植的種子的副本
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
        seedOnThisSoil=Instantiate(seedPrefabs[seedIndex], seedSpawnPoint.position, Quaternion.identity);
        isPlantable = false; // 標記這塊土地已經被種植
        isPlanting = true;
    }
    void IInteractable.Interact() // 互動行為
    {
        if(isPlanting == false && isPlantable == false)
        {
            TurnTheSoil();
        }
        else if(isPlanting == false && isPlantable == true)
        {
            PlantSeed(1);
        }
        
    } 
}
