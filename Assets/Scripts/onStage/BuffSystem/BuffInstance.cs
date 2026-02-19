using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffInstance : MonoBehaviour
{
    public BuffData sourceData;
    public float remainingTime;
    public float tickTimer; // 用於計算何時觸發 Tick 效果

    public BuffInstance(BuffData data)
    {
        sourceData = data;
        remainingTime = data.duration;
    }
}
