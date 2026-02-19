using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public IdolOnStage bindIdol;
    public List<BuffInstance> activeBuffs = new List<BuffInstance>();
    public BuffData testBuff; // 用於測試的 Buff 資料
    void Update()
    {
        // 從最後一個索引開始往回跑 (activeBuffs.Count - 1)
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            BuffInstance buff = activeBuffs[i];

            buff.remainingTime -= Time.deltaTime;

            if (buff.remainingTime <= 0)
            {
                // 時間到了，移除它
                EndBuff(buff);
            }
            else
            {
                // 更新 Tick 計時器
                buff.tickTimer -= Time.deltaTime;
                if (buff.tickTimer <= 0)
                {
                    // 記得讓 OnTick 也能接收偶像資料，這樣才能產生效果！
                    buff.sourceData.OnTick();
                    buff.tickTimer = 1f;
                }
            }
        }
    }
    public void AddBuff(BuffData data)
    {
        BuffInstance newBuff = new BuffInstance(data);
        activeBuffs.Add(newBuff);
        // Apply the buff's effects to the idol here
        newBuff.sourceData.OnApply(bindIdol);
        Debug.Log($"Applied buff: {data.buffName}");
    }
    public void EndBuff(BuffInstance instance) {
        activeBuffs.Remove(instance);
        // Remove the buff's effects from the idol here
        instance.sourceData.OnEnd(bindIdol);
        Debug.Log($"Buff ended: {instance.sourceData.buffName}");
    }
    [ContextMenu("BuffTest")]
    public void TestApplyBuff() {
        AddBuff(testBuff);
    }
}
