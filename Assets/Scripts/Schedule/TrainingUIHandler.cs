using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingUIHandler : MonoBehaviour
{
    public GameObject traningUI;
    // public PlayerControlMainWorld playerController; // 玩家控制腳本
    // public List<PlayerControlMainWorld> teamMembers; // 隊伍成員列表

    void Start()
    {
        DoorInteraction.onDoorInteracted += ShowTrainingUI; // 訂閱並監聽事件
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        DoorInteraction.onDoorInteracted += ShowTrainingUI; // 取消訂閱事件
    }

    void ShowTrainingUI(){
        Debug.Log("開啟訓練 UI");
        Instantiate(traningUI); // （每日結算時再進行銷毀）
        // playerController.enabled = false; // 禁用玩家控制
        
        // // 取得 TeamManager 腳本
        // TeamManager teamManager = FindObjectOfType<TeamManager>();
        // // 複製 TeamManager 腳本的成員列表
        // teamMembers = new List<PlayerControlMainWorld>(teamManager.teamMembers);
    }
}
