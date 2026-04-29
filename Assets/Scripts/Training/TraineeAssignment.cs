using UnityEngine;

/* 掛在 TrainingManager 上，負責指派訓練成員並備份資料 */
public class TraineeAssignment : MonoBehaviour
{
    [SerializeField] private TeamManager teamManager;

    void Start()
    {
        TrainingUIHandler.OnTrainingUIConfirmed += AssignTrainees;
        GoToComputerHint.OnGoToComputer += AssignAllToTrain;
    }

    void OnDestroy()
    {
        TrainingUIHandler.OnTrainingUIConfirmed -= AssignTrainees;
        GoToComputerHint.OnGoToComputer -= AssignAllToTrain;
    }

    public void AssignAllToTrain(TrainingUIData data)
    {
        AssignTrainees(data, true);
    }

    public void AssignTrainees(TrainingUIData data, bool areAllToTrain) // 當任意訓練 UI 按下確定按鈕時呼叫
    {
        // 取得該訓練 UI 類型
        string currentTrainingType = data.trainingType.ToString();

        // 遍歷所有角色來檢查狀態
        foreach (var idol in TeamDataUtility.IdolObjectList)
        {
            IdolInstance idolData = idol.GetComponent<IdolInstance>();

            // 如果角色根本不應該出現在訓練介面，就直接跳過（比如說她正好在打工）
            if(!idolData.CanShowInTheAction(AvailableAction.Train)) continue; 
            
            /* 取得角色的訓練資料 */
            IdolWho idolEnum = idolData.idolIndex;
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
                if (areAllToTrain) continue; // 全員訓練不隱藏角色
                teamManager.AddBusyMember(idolControl);
                idol.SetActive(false);

                // 更新跨場景狀態
                UpdateIdolTrainRecord(idolEnum, isActive: false);
                idolData.currentAction = AvailableAction.Train;
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
                UpdateIdolTrainRecord(idolEnum, isActive: true);
                idolData.currentAction = AvailableAction.Free;
            }
            // 情況三：這個角色在其他 UI 的訓練區（例如角色在 Vocal，而玩家正處於 Dance）
            else
            {
                if (areAllToTrain) // 全員訓練不隱藏角色
                {
                    teamManager.RemoveBusyMember(idolControl);
                    idol.SetActive(true);
                    UpdateIdolTrainRecord(idolEnum, isActive: true);
                    idolData.currentAction = AvailableAction.Train; // 但還是要登記為訓練中
                }

                continue; // 不做其他處理（維持原狀）
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
        UpdateIdolTrainRecord(
            idol.idolIndex,
            vigourCost: data.neededVigour,
            danceExp: finalDanceExp,
            vocalExp: finalVocalExp,
            visualExp: finalVisualExp
        );
    }

    // 重置訓練數值 (當角色不在訓練區)
    private void ClearTrainingStats(IdolInstance idol)
    {
        UpdateIdolTrainRecord(
            idol.idolIndex,
            vigourCost: 0,
            danceExp: 0,
            vocalExp: 0,
            visualExp: 0
        );
    }

    // 在指派訓練成員的同時備份狀態變化
    public static void UpdateIdolTrainRecord(
        IdolWho name, // 第一項引數必填
        IdolTrainingState? state = null,
        Vector2? position = null,
        DropZoneType? droppedZoneType = null,
        int? droppedZoneIndex = null,
        int? vigourCost = null,
        int? danceExp = null,
        int? vocalExp = null,
        int? visualExp = null,
        bool? isActive = null
    )
    {
        var idol = TeamDataUtility.IdolDict[name];
        var trainRecord = idol.trainRecord;

        if (state != null) trainRecord.state = state.Value;
        if (position != null) trainRecord.position = position.Value;
        if (droppedZoneType != null) trainRecord.droppedZoneType = droppedZoneType.Value;
        if (droppedZoneIndex != null) trainRecord.droppedZoneIndex = droppedZoneIndex.Value;
        if (vigourCost != null) trainRecord.vigourCost = vigourCost.Value;
        if (danceExp != null) trainRecord.danceExp = danceExp.Value;
        if (vocalExp != null) trainRecord.vocalExp = vocalExp.Value;
        if (visualExp != null) trainRecord.visualExp = visualExp.Value;

        if (isActive != null) idol.isAvailable = isActive.Value;
    }
}