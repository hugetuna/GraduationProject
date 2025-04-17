using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrainingUIHandler : MonoBehaviour
{
    public GameObject traningUI;
    public GameObject teamManager; // 預計使用 TeamManager 取得當前隊伍成員
    private GameObject traningUIInstance = null; // 記錄被生成的訓練 UI

    void Start()
    {
        DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 訂閱並監聽事件
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 檢查滑鼠左鍵是否被按下
        {
            if (!IsCursorClickUIObject() && traningUIInstance != null) // 點擊非 UI 區域時關閉 UI
            {
                Debug.Log("關閉訓練 UI");
                traningUIInstance.SetActive(false); 
            }
        }

        if(traningUIInstance != null && traningUIInstance.activeSelf){
            // 若訓練 UI 被開啟，就按照目前隊伍的狀態來更新 UI
        }
    }

    void OnDestroy()
    {
        DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 取消訂閱事件
    }

    private void ShowTrainingUI(){
        Debug.Log("開啟訓練 UI");
        if(traningUIInstance == null){
            traningUIInstance = Instantiate(traningUI); // （每日結算時再進行銷毀）
        }
        else {
            traningUIInstance.SetActive(true); // 如果 UI 已經存在，則顯示它
        }
        // Time.timeScale = 0; // 禁用玩家控制（以暫停遊戲的方式）
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
