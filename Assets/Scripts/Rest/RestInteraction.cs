using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestInteraction : MonoBehaviour
{
    // public string InteractionKey => "Rest"; // 這個字串用來指定動畫 key
    //-----------------------------------------------------------------//
    private float holdTimer = 0f;
    public float holdThreshold = 3f; // 長按觸發的時間（秒）
    public TeamManager teamManager;
    private bool isPlayerInsideCollider = false;
    private bool darkRestTriggered = false;

    void Update()
    {
        if (isPlayerInsideCollider) return;

        // 按住空白鍵開始計時
        if (Input.GetKey(KeyCode.Space))
        {
            if (!darkRestTriggered)
            {
                holdTimer += Time.deltaTime;

                // 長按滿特定秒數觸發
                if (holdTimer >= holdThreshold)
                {
                    darkRestTriggered = true;
                    TriggerDarkRest();
                }
            }
        }

        // 放開空白鍵立刻重置狀態
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ResetDarkRestStatus();
        }
    }

    private void TriggerDarkRest()
    {
        Debug.Log("觸發還沒做好的休息功能");
        
        var teamMembers = teamManager.teamMembers;
        var currentLeaderIndex = teamManager.currentLeaderIndex;
        teamMembers[currentLeaderIndex].GetComponent<IdolInstance>().recoverVigour(50);
    }

    private void ResetDarkRestStatus()
    {
        holdTimer = 0f;
        darkRestTriggered = false;
    }

    private void OnTriggerEnter(Collider other) // 碰撞偵測
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideCollider = false;
            ResetDarkRestStatus(); // 玩家離開範圍立刻重置，防止累積時間
        }
    }
}
