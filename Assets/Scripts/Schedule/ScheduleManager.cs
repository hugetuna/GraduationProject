using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScheduleManager : MonoBehaviour
{
    /* date 變數和 changeDate 函式跟 DayManager.cs 的寫法一樣 */
    public static int date = 1; // 靜態變數，保存遊戲中的日期

    public static event Action OnChangeDay; // 定義切換天數事件
    public TeamUIData teamUIData; // 在此用來取得訓練成員清單
    private List<string> teamTrainees;
    public bool isWithTeacher = false; // 是否有老師協助訓練，預設為無
    public string trainingType = "Dance"; // 訓練類型，預設為舞蹈訓練
    public float danceTrainingEffect = 1.0f; // 額外訓練效果（之後會再從 IdolInstance 讀進來）
    private bool isSettled = false; // 是否已經結算過訓練


    void Start() // 可以在開始遊戲時初始化，或每次遊戲結束時進行保存
    {
        date = 1; // 初始化日期，若需要可以改成從保存中讀取
        Debug.Log($"今天是第 {date} 天");

        teamTrainees = teamUIData.teamTrainees;
    }

    [ContextMenu("SettleTraining")]
    public void SettleTraining() // 結算訓練收益的函式（目前須手動操作）
    {
        if (!isSettled)
        {
            Debug.Log("進行訓練結算");

            foreach (string trainee in teamTrainees) // 迭代訓練成員
            {
                var character = GameObject.Find($"Character_{trainee}"); // 尋找場景上對應的角色物件
                if (character != null)
                { // 確保角色物件存在
                    IdolInstance idolInstance = character.GetComponent<IdolInstance>(); // 取得角色的 IdolInstance 組件
                    idolInstance.vigour -= 20; // 減少體力值
                    if (trainingType == "Dance") // 如果是舞蹈訓練
                    {
                        // 增加該角色的舞蹈數值並乘上對應的訓練效果 
                        if (isWithTeacher)
                        {
                            idolInstance.dance += (int)(100 * danceTrainingEffect);
                        }
                        else
                        {
                            idolInstance.dance += (int)(60 * danceTrainingEffect);
                        }
                    }

                }
            }
            isSettled = true; // 設定為已結算
        }
        else
        {
            Debug.Log("今日訓練已結算，請前往下一天");
        }


    }

    [ContextMenu("ChangeDay")]
    public void ChangeDay() // 用來更動日期的函式（目前須手動操作）
    {
        if (isSettled) // 如果已經結算過訓練
        {
            OnChangeDay?.Invoke(); // 觸發切換天數事件
            date++;
            Debug.Log($"今天是第 {date} 天");
            isSettled = false; // 重置結算狀態
        }
        else
        {
            Debug.Log("請先結算今日訓練後再切換天數");
        }
    }

}
