using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScheduleManager : MonoBehaviour
{
    public DayManager dayManager; // 取得 DayManager 的參考
    public static event Action OnChangeDay; // 定義切換天數事件

    public TeamUIData teamUIData; // 在此用來取得訓練成員清單
    private List<string> teamTrainees;
    public TrainingUIData trainingUIData;

    public static bool isSettled = false; // 是否已經結算過訓練
    public static List<GameObject> disappearCharacters = new(); // 訓練結算後隱藏的角色物件
    public TeamManager teamManager; // 透過 teamManager 取得當前隊長

    public GameObject computerMenu;
    public Button settleTrainingButton; // 訓練結算按鈕
    public Button changeDayButton; // 切換天數按鈕


    void Start()
    {
        ComputerInteraction.OnComputerInteracted += ShowMenu; // 訂閱並監聽與電腦互動事件
        teamTrainees = teamUIData.teamTrainees;
        computerMenu.SetActive(false); // 初始時隱藏電腦選單
        settleTrainingButton.onClick.AddListener(SettleTraining); // 訓練結算按鈕點擊事件
        changeDayButton.onClick.AddListener(ChangeDay); // 切換天數按鈕點擊事件
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 檢查滑鼠左鍵是否被按下
        {
            if (!IsCursorClickUIObject()) // 點擊非 UI 區域時關閉 UI
            {
                //Debug.Log("關閉電腦選單 UI");
                computerMenu.SetActive(false);
            }
        }
    }

    void OnDestroy()
    {
        ComputerInteraction.OnComputerInteracted -= ShowMenu; // 取消訂閱與電腦互動事件
    }

    private void ShowMenu() // 顯示電腦選單的函式
    {
        computerMenu.SetActive(true); // 顯示電腦選單
    }

    //[ContextMenu("SettleTraining")]
    public void SettleTraining() // 結算訓練收益的函式
    {
        if (!isSettled)
        {
            Debug.Log("進行訓練結算");

            foreach (string trainee in teamTrainees) // 迭代訓練成員
            {
                var character = GameObject.Find($"Character_{trainee}"); // 尋找場景上對應的角色物件
                if (character != null) // 確保角色物件存在
                {
                    IdolInstance idolInstance = character.GetComponent<IdolInstance>(); // 取得角色的 IdolInstance 組件
                    idolInstance.vigour -= trainingUIData.neededVigour; // 減少體力值
                    if (trainingUIData.trainingType == "Dance") // 如果是舞蹈訓練
                    {
                        // 增加該角色的舞蹈數值並乘上對應的訓練效果 
                        if (trainingUIData.isWithTeacher)
                        {
                            idolInstance.dance += trainingUIData.withTeacherBenefit;
                        }
                        else
                        {
                            idolInstance.dance += trainingUIData.basicBenefit;
                        }
                    }
                    var playerControlMainWorld = character.GetComponent<PlayerControlMainWorld>();
                    if (teamManager.teamMembers.IndexOf(playerControlMainWorld) == teamManager.currentLeaderIndex)
                    {
                        // 如果該角色剛好是隊長，就先將隊長切換成下一位（不然隊長消失後隊伍會動不了）
                        teamManager.SwitchLeader(1);
                    }
                    disappearCharacters.Add(character);
                    character.SetActive(false); // 隱藏並停用角色物件（等同於從隊伍中消失）
                }
            }
            computerMenu.SetActive(false);
            isSettled = true; // 設定為已結算
        }
        else
        {
            Debug.Log("今日訓練已結算，請前往下一天");
        }


    }

    //[ContextMenu("ChangeDay")]
    public void ChangeDay() // 用來更動日期的函式
    {
        if (isSettled) // 如果已經結算過訓練
        {
            dayManager.ChangeDay();
            OnChangeDay?.Invoke(); // 觸發切換天數事件
            computerMenu.SetActive(false);
            isSettled = false; // 重置結算狀態
        }
        else
        {
            Debug.Log("請先結算今日訓練後再切換天數");
        }
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
