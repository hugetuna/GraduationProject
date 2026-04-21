using UnityEngine;
using UnityEngine.UI;

/* 掛在角色 UI 上 */
public class TrainingVigourBar : MonoBehaviour
{
    private IdolInstance characterInfo; // 該角色的數值資料
    private TrainingUIData trainingUIData;
    private DropZoneType currentZoneType;
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
    [SerializeField] private Vector2 sendPosition; // 角色準備外出商演時，體力條的位置（拖曳後的位置）

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
        float cost = trainingUIData.neededVigour;

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

        ApplyGrayEffect(zoneType); // 根據體力狀態更新灰階效果
    }

    private void ApplyGrayEffect(DropZoneType zoneType)
    {
        if (grayEffect == null || characterInfo == null || trainingUIData == null) return;

        // 檢查體力是否足夠
        bool isTooTired = characterInfo.vigour < trainingUIData.neededVigour;
        // Debug.Log($"對 {characterInfo.idolIndex} 使用灰階效果: 體力={characterInfo.vigour}, 耗體={trainingUIData.vigourCost}");

        if (zoneType == DropZoneType.Member)
        {
            // 在 Member 區才需要根據體力變灰
            grayEffect.SetGrayScale(isTooTired);
            fillGrayEffect.SetGrayScale(isTooTired);
            drag.enabled = !isTooTired; // 體力不足時禁用拖曳功能
        }
        else // 訓練區（先不管 None）
        {
            // 取消灰階（不論體力狀態）
            grayEffect.SetGrayScale(false);
            fillGrayEffect.SetGrayScale(false);
            drag.enabled = true; // 始終允許拖曳功能
        }
    }
}
