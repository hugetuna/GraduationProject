using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/* 管理 UI 和玩家輸入相關的函式，不用特別掛在誰身上 */
public class UIAndPlayerInput : MonoBehaviour
{
    public static List<PlayerInput> playerInputs = new(); // 玩家輸入系統

    void Start()
    {
        // 收集所有玩家的輸入系統
        PlayerInput[] inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        playerInputs.AddRange(inputs);
    }

    public static void RefreshPlayerInputs() // 換場景後重新抓取
    {
        playerInputs.Clear();

        PlayerInput[] inputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        playerInputs.AddRange(inputs);
    }

    public static void EnableAllPlayerInputs()
    {
        if (playerInputs.Exists(input => input == null)) RefreshPlayerInputs();

        foreach (PlayerInput input in playerInputs)
        {
            input.enabled = true; // 啟用所有玩家的輸入系統
        }
    }

    public static void DisableAllPlayerInputs()
    {
        if (playerInputs.Exists(input => input == null)) RefreshPlayerInputs();

        foreach (PlayerInput input in playerInputs)
        {
            input.enabled = false; // 禁用所有玩家的輸入系統
        }
    }

    // public static bool IsCursorClickUIObject(GameObject target) // 檢查特定物件是否被滑鼠點擊
    // {
    //     if (target == null) return false;

    //     // 根據當前操作，設定滑鼠或觸控位置
    //     PointerEventData eventData = new(EventSystem.current)
    //     {
    //         position = Input.mousePosition
    //     };

    //     // RaycastAll 會從 eventData 中的滑鼠位置發射一條射線，檢測所有碰撞的 UI 元素
    //     // 符合條件的 UI 元素會被加到 raycastResults 清單中
    //     var raycastResults = new List<RaycastResult>();
    //     EventSystem.current.RaycastAll(eventData, raycastResults);

    //     // 遍歷所有射線碰到的 UI，看看有沒有我們要的那個物件
    //     foreach (var result in raycastResults)
    //     {
    //         if (result.gameObject.transform == target.transform ||
    //             result.gameObject.transform.IsChildOf(target.transform))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
}
