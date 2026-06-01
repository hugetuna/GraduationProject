using System;
using UnityEngine;
using UnityEngine.UI;

public class RestVigourBar : MonoBehaviour
{
    // public static event Action OnTrainingTutorialDone; // 定義訓練室新手教學完成事件（新手教學用）
    //-----------------------------------------------------------------//
    private IdolInstance characterInfo; // 該角色的數值資料
    private RestDropZoneType currentZoneType;
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Image fillImage; // 體力值 UI 中的填充圖片（同時表示體力變化）
    //-----------------------------------------------------------------//
    // private DragToRest drag; // 拖曳元件參考
    private RectTransform vigourRect; // 體力條的 RectTransform 參考
    [SerializeField] private Vector2 teamPosition; // 角色在隊伍裡，體力條的位置（初始位置）
    [SerializeField] private Vector2 sendPosition; // 角色準備休息時，體力條的位置（拖曳後的位置）

    void Awake()
    {
        // drag = GetComponent<DragToRest>();
        vigourRect = vigourSlider.GetComponent<RectTransform>();
    }

    public void Initialize(IdolWho myIdolIndex)
    {
        characterInfo = TeamDataUtility.IdolDict[myIdolIndex]; // 尋找對應的角色資料
        currentZoneType = characterInfo.restRecord.zoneType; // 根據角色的休息紀錄設定區域類型

        UpdateVigourBar(currentZoneType); // 初始化體力條顯示
    }

    public void UpdateVigourBar(RestDropZoneType zoneType)
    {
        if (zoneType == RestDropZoneType.None)
        {
            if (currentZoneType == RestDropZoneType.None)
            {
                Debug.LogWarning("無法更新體力條：缺少當前區域類型資訊");
                return;
            }
            zoneType = currentZoneType; // 如果沒有提供新的區域類型，使用當前的類型
        }

        currentZoneType = zoneType; // 更新位置
        if (characterInfo == null) return; // 簡單的防呆檢查

        // 基礎數值計算
        float max = characterInfo.vigourMax;
        float current = characterInfo.vigour;
        float add = characterInfo.restRecord.vigourEarned;

        vigourSlider.maxValue = max; // 設定體力值 UI 的最大值

        if (zoneType == RestDropZoneType.Member) // 成員區
        {
            vigourRect.anchoredPosition = teamPosition; // 移回隊伍位置

            vigourSlider.value = current; // 更新體力值 UI 的當前值（無體力變化）
            
        }
        else // 休息區（先不管 None）
        {
            vigourRect.anchoredPosition = sendPosition; // 移動到訓練位置

            vigourSlider.value = current + add; // 更新體力值 UI 的當前值（有體力變化）
        }

        // ApplyColorEffect(zoneType); // 根據體力狀態更新顏色效果 => 休息室似乎不用
    }
}
