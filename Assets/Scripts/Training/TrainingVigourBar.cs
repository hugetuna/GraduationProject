using UnityEngine;
using UnityEngine.UI;
using System;

/* 掛在角色 UI 上 */
public class TrainingVigourBar : MonoBehaviour
{
    public static event Action OnTrainingTutorialDone; // 定義訓練室新手教學完成事件（新手教學用）
    //-----------------------------------------------------------------//
    private IdolInstance characterInfo; // 該角色的數值資料
    private TrainingUIData trainingUIData;
    private DropZoneType currentZoneType;
    private bool isWithTeacher;
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Image fillImage; // 體力值 UI 中的填充圖片
    [SerializeField] private Image lastFillImage; // 上一次體力值的填充圖片（用來表示體力變化）
    //-----------------------------------------------------------------//
    private DragToLesson drag; // 拖曳元件參考
    private UIGrayEffect grayEffect; // 灰階效果參考
    private UIGrayEffect fillGrayEffect; // 體力條圖片的灰階效果參考
    private RectTransform vigourRect; // 體力條的 RectTransform 參考
    [SerializeField] private Vector2 teamPosition; // 角色在隊伍裡，體力條的位置（初始位置）
    [SerializeField] private Vector2 sendPosition; // 角色準備訓練時，體力條的位置（拖曳後的位置）
    [SerializeField] private Color32 tiredColor = new Color32(244, 112, 112, 255); // 體力不足時的顏色

    void Awake()
    {
        drag = GetComponent<DragToLesson>();
        grayEffect = GetComponent<UIGrayEffect>();
        fillGrayEffect = fillImage.GetComponent<UIGrayEffect>();
        vigourRect = vigourSlider.GetComponent<RectTransform>();
    }

    public void Initialize(TrainingUIData trainingUIData, IdolWho myIdolIndex)
    {
        this.trainingUIData = trainingUIData;
        characterInfo = TeamDataUtility.IdolDict[myIdolIndex]; // 尋找對應的角色資料
        currentZoneType = characterInfo.trainRecord.droppedZoneType; // 根據角色的打工紀錄設定區域類型
        isWithTeacher = GameManager.Instance.teacherSaveData.IsWithTeacherToday(trainingUIData.trainingType);

        UpdateVigourBar(currentZoneType); // 初始化體力條顯示
    }

    public void UpdateVigourBar(DropZoneType zoneType)
    {
        if (zoneType == DropZoneType.None)
        {
            if (currentZoneType == DropZoneType.None)
            {
                Debug.LogWarning("無法更新體力條：缺少當前區域類型資訊");
                return;
            }
            zoneType = currentZoneType; // 如果沒有提供新的區域類型，使用當前的類型
        }

        currentZoneType = zoneType; // 更新位置
        if (characterInfo == null || trainingUIData == null) return; // 簡單的防呆檢查

        // 基礎數值計算
        float max = characterInfo.vigourMax;
        float current = characterInfo.vigour;
        float cost = isWithTeacher ? trainingUIData.neededVigour - 5 : trainingUIData.neededVigour;

        vigourSlider.maxValue = max; // 設定體力值 UI 的最大值

        if (zoneType == DropZoneType.Member)
        {
            vigourRect.anchoredPosition = teamPosition; // 移回隊伍位置

            vigourSlider.value = current; // 更新體力值 UI 的當前值
            lastFillImage.fillAmount = 0; // 不須顯示體力變化
        }
        else // 訓練區（先不管 None）
        {
            vigourRect.anchoredPosition = sendPosition; // 移動到訓練位置

            vigourSlider.value = current - cost; // 更新體力值 UI 的當前值
            var ratio = Mathf.Clamp01(current / max); // 計算長度比例 (確保不低於 0)
            lastFillImage.fillAmount = ratio; // 顯示體力變化
        }

        ApplyColorEffect(zoneType); // 根據體力狀態更新顏色效果
    }

    private void ApplyColorEffect(DropZoneType zoneType)
    {
        if (grayEffect == null || characterInfo == null || trainingUIData == null) return;

        // 檢查體力是否足夠
        float cost = isWithTeacher ? trainingUIData.neededVigour - 5 : trainingUIData.neededVigour;
        bool isTooTired = characterInfo.vigour < cost;
        // Debug.Log($"對 {characterInfo.idolIndex} 使用灰階效果: 體力={characterInfo.vigour}, 耗體={cost}");

        // 檢查今天是否為訓練室的新手教學（因為會影響到體力判定所以放在這個腳本處理）
        bool firstDay = false;
        var currentEvent = DayManager.Instance.dayEventManager.currentEvent;
        if (DayManager.Instance.chapter == 0 && DayManager.Instance.date == 1 && currentEvent != null && currentEvent.TriggerTimeIndex >= 6)
        {
            if (currentZoneType != DropZoneType.Member)
            {
                // 把角色放進訓練室後就不允許再拖曳了
                // Debug.Log("已將指定角色放入訓練室，鎖定拖曳");
                drag.enabled = false;
                firstDay = true;
                OnTrainingTutorialDone?.Invoke();
            }
        }

        if (zoneType == DropZoneType.Member)
        {
            // 在 Member 區才需要根據體力變色
            grayEffect.SetGrayScale(isTooTired);
            // fillGrayEffect.SetGrayScale(isTooTired);
            fillImage.color = isTooTired ? tiredColor : Color.white; // 體力不足時改變顏色，否則恢復正常顏色
            if (!firstDay) drag.enabled = !isTooTired; // 體力不足時禁用拖曳功能
        }
        else // 訓練區（先不管 None）
        {
            // 取消變色（不論體力狀態）
            grayEffect.SetGrayScale(false);
            // fillGrayEffect.SetGrayScale(false);
            fillImage.color = Color.white;
            if (!firstDay) drag.enabled = true; // 始終允許拖曳功能
        }
    }
}
