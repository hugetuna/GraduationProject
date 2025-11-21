using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/* 管理 UI 和玩家輸入相關的函式，掛在 UIManager 上 */
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

    public static bool IsCursorClickUIObject() // 檢查掛腳本的物件是否被滑鼠點擊
    {
        // 根據當前操作，設定滑鼠或觸控位置
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // RaycastAll 會從 eventData 中的滑鼠位置發射一條射線，檢測所有碰撞的 UI 元素
        // 符合條件的 UI 元素會被加到 raycastResults 清單中
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults.Count > 0;
    }
}
