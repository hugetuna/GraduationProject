using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoilSaveData
{
    public Vector3 position;
    public bool isPlantable;
    public bool isPlanting;
    public bool isWatered;
    public string plantedSeedName;
    public int daysGrown;
    public int currentRewardPoint;
}

