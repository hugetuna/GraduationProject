using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PackUIHandler : MonoBehaviour
{
    public GameObject packUI; // 背包 UI
    public Button packButton; // 背包按鈕

    void Start()
    {
        packUI.SetActive(false); // 初始化背包 UI 狀態
        packButton.onClick.AddListener(OpenPackUI); // 設置按鈕點擊事件
    }

    void Update()
    {
        if (packUI.activeSelf && Input.GetMouseButtonDown(0)) // 檢查滑鼠左鍵是否被按下
        {
            if (!IsCursorClickUIObject()) // 點擊非 UI 區域時關閉 UI
            {
                Debug.Log("關閉背包 UI");
                packUI.SetActive(false);
            }
        }

        // 檢查背包 UI 是否開啟，並根據狀態啟用或禁用背包按鈕
        if (!packUI.activeSelf) packButton.interactable = true; // 啟用背包按鈕
        else packButton.interactable = false; // 禁用背包按鈕
    }

    private void OpenPackUI()
    {
        if (!packUI.activeSelf) packUI.SetActive(true); // 如果背包 UI 未開啟，則打開它
    }
    
    private bool IsCursorClickUIObject()
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
