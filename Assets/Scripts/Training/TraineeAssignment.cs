using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* 掛在 TrainingManager 上，負責指派訓練成員並備份資料 */
public class TraineeAssignment : MonoBehaviour
{
    [SerializeField] private TeamManager teamManager;

    void Start()
    {
        TrainingUIHandler.OnTrainingUIConfirmed += AssignTrainees;
        TrainingHintToggler.OnGoToComputer += AssignTrainees;
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIConfirmed -= AssignTrainees;
        TrainingHintToggler.OnGoToComputer -= AssignTrainees;
    }

    public void AssignTrainees(TrainingUIData data) // 當任意訓練 UI 按下確定按鈕時呼叫
    {
        // 取得該訓練 UI 類型
        string currentTrainingType = data.trainingType.ToString();

        // 遍歷所有角色來檢查狀態
        foreach (var idol in TeamDataUtility.IdolObjectList)
        {
            // 取得角色的訓練資料
            string idolName = TeamDataUtility.CleanNameOfCharacterObject(idol.name);
            IdolWho idolEnum = TeamDataUtility.GetIdolEnum(idolName);
            IdolInstance idolData = TeamDataUtility.IdolDict[idolEnum];
            var trainRecord = idolData.trainRecord;

            // 取得角色的控制腳本
            PlayerControlMainWorld idolControl = idol.GetComponent<PlayerControlMainWorld>();

            // 判斷該角色目前被分配在哪個區域 (由拖曳系統決定)
            string idolZoneStr = trainRecord.droppedZoneType.ToString();

            // 情況一：這個角色被分配到「當前正開啟的訓練 UI」
            if (idolZoneStr == currentTrainingType)
            {
                // 執行數值計算
                CalculateAndSetTrainingStats(idolData, data);

                // 標記為忙碌並隱藏角色物件
                teamManager.AddBusyMember(idolControl);
                idol.SetActive(false);

                // 更新跨場景狀態
                UpdateTrainRecord(idolEnum, isActive: false);
            }
            // 情況二：這個角色在「成員區」(可能是剛被移出訓練，或者原本就沒事)
            else if (trainRecord.droppedZoneType == DropZoneType.Member)
            {
                // 重置訓練數值計算
                ClearTrainingStats(idolData);

                // 確保角色在場景中是顯示的
                teamManager.RemoveBusyMember(idolControl);
                idol.SetActive(true);

                // 更新跨場景狀態
                UpdateTrainRecord(idolEnum, isActive: true);
            }
            // 情況三：這個角色在其他 UI 的訓練區（例如角色在 Vocal，而玩家正處於 Dance）
            else
            {
                // 不做任何處理（維持原狀）
                continue;
            }
        }
    }
    
    // 計算並寫入訓練數值 (體力與經驗)
    private void CalculateAndSetTrainingStats(IdolInstance idol, TrainingUIData data)
    {
        // 根據是否有老師來決定收益類型
        var teacherName = GameManager.Instance.teacherSaveData.GetTeacherNameByType(data.trainingType);
        int benefit = teacherName != "無" ? data.withTeacherBenefit : data.basicBenefit;

        int finalDanceExp = 0;
        int finalVocalExp = 0;
        int finalVisualExp = 0;

        // 根據訓練類型計算加成
        switch (data.trainingType)
        {
            case TrainingType.Dance:
                finalDanceExp = (int)(benefit * idol.daTrainingBonus);
                break;
            case TrainingType.Vocal:
                finalVocalExp = (int)(benefit * idol.voTrainingBonus);
                break;
            case TrainingType.Visual:
                finalVisualExp = (int)(benefit * idol.viTrainingBonus);
                break;
        }

        // 寫入 Record (這裡只是紀錄「將會」發生什麼事，還沒真正扣體力)
        UpdateTrainRecord(idol.idolIndex,
            vigourCost: data.neededVigour,
            danceExp: finalDanceExp,
            vocalExp: finalVocalExp,
            visualExp: finalVisualExp
        );
    }

    // 重置訓練數值 (當角色不在訓練區)
    private void ClearTrainingStats(IdolInstance idol)
    {
        UpdateTrainRecord(idol.idolIndex,
            vigourCost: 0,
            danceExp: 0,
            vocalExp: 0,
            visualExp: 0
        );
    }

    // 在指派訓練成員的同時備份狀態變化
    public static void UpdateTrainRecord(IdolWho name, // 第一項引述必填
                                         IdolTrainingState state = IdolTrainingState.None,
                                         Vector2? position = null,
                                         DropZoneType droppedZoneType = DropZoneType.None,
                                         int? droppedZoneIndex = null,
                                         int? vigourCost = null,
                                         int? danceExp = null,
                                         int? vocalExp = null,
                                         int? visualExp = null,
                                         bool? isActive = null)
    {
        var idol = TeamDataUtility.IdolDict[name];
        var trainRecord = idol.trainRecord;

        if (state != IdolTrainingState.None) trainRecord.state = state;
        if (position != null) trainRecord.position = position.Value;
        if (droppedZoneType != DropZoneType.None) trainRecord.droppedZoneType = droppedZoneType;
        if (droppedZoneIndex != null) trainRecord.droppedZoneIndex = droppedZoneIndex.Value;
        if (vigourCost != null) trainRecord.vigourCost = vigourCost.Value;
        if (danceExp != null) trainRecord.danceExp = danceExp.Value;
        if (vocalExp != null) trainRecord.vocalExp = vocalExp.Value;
        if (visualExp != null) trainRecord.visualExp = visualExp.Value;
        if (isActive != null) trainRecord.isActive = isActive.Value;
    }
}