using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HarvestGachaPool", menuName = "Soul System/HarvestGachaPool")]
public class HarvestGachaPool : ScriptableObject
{
    public int rewardPointLimitMin;
    public int rewardPointLimitMax;
    public List<FansItem> gachaPool;
}
