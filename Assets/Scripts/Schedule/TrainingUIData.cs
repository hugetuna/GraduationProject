using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingUIData", menuName = "Schedule/TrainingUIData")]
public class TrainingUIData : ScriptableObject
{
    /* 訓練 UI 的基本介紹部分 */
    public string trainingType = ""; // 訓練類型
    public bool isWithTeacher = false; // 是否有老師協助訓練，預設為無
    public string teacherName = "無"; // 老師名稱，預設為無
    public int neededVigour = 20; // 訓練所需的體力值，預設為 20
    public int basicBenefit = 5; // 訓練的基本收益，預設為 5（找不到設定資料QQ）

    /* 根據角色狀態有所浮動的部分 */
    // 角色訓練前與訓練後的體力＆能力值 -> 讀取角色的資料再加以視覺化
    // 角色當前的 buff 與 debuff -> 尚未實作，可以先隨意寫

    public void Reset() // 恢復預設值
    {
        trainingType = "";
        isWithTeacher = false;
        teacherName = "無";
        neededVigour = 20;
        basicBenefit = 5;
    }
}
