using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIAndPlayerInput : MonoBehaviour
{
    public static List<PlayerInput> playerInputs = new(); // 玩家輸入系統

    void Start()
    {
        // 收集所有玩家的輸入系統
        PlayerInput[] inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        playerInputs.AddRange(inputs);
    }

    public static void EnableAllPlayerInputs()
    {
        foreach (PlayerInput input in playerInputs)
        {
            input.enabled = true; // 啟用所有玩家的輸入系統
        }
    }

    public static void DisableAllPlayerInputs()
    {
        foreach (PlayerInput input in playerInputs)
        {
            input.enabled = false; // 禁用所有玩家的輸入系統
        }
    }
}
