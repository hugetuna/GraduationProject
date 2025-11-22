using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitPlayerEnterAreaEvent : MonoBehaviour
{
    private System.Action onFinish;

    private Vector3 minPos;
    private Vector3 maxPos;

    private Transform leader;  // 取得隊長
    private TeamManager teamManager;
    public void StartEvent(Vector3 areaMin, Vector3 areaMax, System.Action finish)
    {
        minPos = areaMin;
        maxPos = areaMax;
        onFinish = finish;
        teamManager= FindAnyObjectByType<TeamManager>();
        // 開始檢查玩家位置
        enabled = true;
    }

    private void Update()
    {
        leader= teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<Transform>();
        if (leader == null) return;

        Vector3 p = leader.position;

        bool inside =
            p.x >= minPos.x && p.x <= maxPos.x &&
            p.y >= minPos.y && p.y <= maxPos.y &&
            p.z >= minPos.z && p.z <= maxPos.z;

        if (inside)
        {
            enabled = false; // 停止 Update 監聽
            onFinish?.Invoke(); // 讓 DayFlowController 進入下一事件
        }
    }
}
