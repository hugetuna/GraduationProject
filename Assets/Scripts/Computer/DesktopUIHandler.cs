using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* 掛在 UIManager 等物件上，不得掛在 UI（視窗）本身 */
public class DesktopUIHandler : MonoBehaviour
{
    [Tooltip("桌面 UI —— 與電腦互動可開啟電腦介面")]
    public GameObject desktopUI;
    //-----------------------------------------------------------------//
    [Tooltip("用來取得當前隊伍成員，並在介面開啟時停用角色控制")]
    public TeamManager teamManager; // 透過 TeamManager 物件取得當前隊伍成員
    private List<PlayerControlMainWorld> teamMembers = new(); // 記錄取得的隊伍成員
    private List<PlayerInput> playerInputs = new(); // 玩家輸入系統

    void Awake()
    {
        // 初始為透視投影
        Camera.main.orthographic = false;
    }

    void Start()
    {
        desktopUI.SetActive(false); // 初始隱藏桌面 UI
        ComputerInteraction.OnComputerInteracted += ShowDesktopUI; // 訂閱並監聽與電腦互動事件

        teamMembers = teamManager.teamMembers; // 獲取當前隊伍成員
        foreach (PlayerControlMainWorld member in teamMembers)
        {
            if (member.TryGetComponent<PlayerInput>(out var playerInput))
            {
                playerInputs.Add(playerInput); // 收集所有玩家的輸入系統
            }
        }
    }

    void OnDestroy()
    {
        ComputerInteraction.OnComputerInteracted -= ShowDesktopUI; // 取消訂閱與電腦互動事件
    }

    void Update()
    {
        if (desktopUI.activeSelf)
        {
            foreach (PlayerInput input in playerInputs)
            {
                input.enabled = false; // 禁用所有玩家的輸入系統
            }
        }
        else
        {
            foreach (PlayerInput input in playerInputs)
            {
                input.enabled = true; // 啟用所有玩家的輸入系統
            }
        }
    }

    private void ShowDesktopUI()
    {
        desktopUI.SetActive(true); // 顯示桌面 UI
        Camera cam = GetComponent<Camera>();

        // 切換成平行投影
        Camera.main.orthographic = true;
    }
}
