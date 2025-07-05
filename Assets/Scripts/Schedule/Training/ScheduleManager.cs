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
    public TrainingUIData trainingUIData;

    public static bool isSettled = false; // 是否已經結算過訓練
    public static List<GameObject> disappearCharacters = new(); // 訓練結算後隱藏的角色物件
    public TeamManager teamManager; // 透過 teamManager 取得當前隊長


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
