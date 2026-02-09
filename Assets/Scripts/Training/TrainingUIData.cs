using UnityEngine;

public enum TrainingType { None = -1, Dance = 0, Vocal = 1, Visual = 2 }

[CreateAssetMenu(fileName = "TrainingUIData", menuName = "Training/TrainingUIData")]

public class TrainingUIData : ScriptableObject
{
    /* 訓練 UI 的基本介紹部分 */
    public TrainingType trainingType = TrainingType.None; // 訓練類型
    public string teacherName = ""; // 老師名稱，預設為無
    public int neededVigour = 20; // 訓練所需的體力值，預設為 20
    public int basicBenefit = 60; // 訓練的基本收益，預設為 60
    public int withTeacherBenefit = 100; // 有老師協助訓練的收益，預設為 100
    //-----------------------------------------------------------------//
    /* 根據角色狀態有所浮動的部分 */
    // 角色訓練前與訓練後的體力＆能力值 -> 讀取角色的資料再加以視覺化 v
    // 角色當前的 buff 與 debuff -> 尚未實作，可以先隨意寫


// #if UNITY_EDITOR // 僅在編輯器模式下使用
//     public void Reset() // 恢復預設值
//     {
//         trainingType = "";
//         teacherName = "";
//         neededVigour = 20;
//         basicBenefit = 60;
//         withTeacherBenefit = 100;
//         teamData = null;
//     }
// #endif
}
