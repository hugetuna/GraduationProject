using UnityEngine;
using UnityEngine.UI;

/* 掛在角色的 UI 圖片上 */
public class ActivityVigourBar : MonoBehaviour
{
    private IdolInstance characterInfo; // 該角色的數值資料
    private Activity activityData;
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Image fillImage; // 體力值 UI 中的填充圖片
    [SerializeField] private Image lastFillImage; // 上一次體力值的填充圖片（用來表示體力變化）
    //-----------------------------------------------------------------//
    private UIGrayEffect grayEffect; // 灰階效果參考
    private Vector2 teamPosition; // 角色在隊伍裡，體力條的位置（初始位置）
    [SerializeField] private Vector2 sendPosition; // 角色準備外出商演時，體力條的位置（拖曳後的位置）


    void Awake()
    {
        grayEffect = GetComponent<UIGrayEffect>();
    }

    public void Initialize(Activity activityData, IdolWho myIdolIndex)
    {
        this.activityData = activityData;
        teamPosition = vigourSlider.GetComponent<RectTransform>().anchoredPosition; // 記錄初始位置

        characterInfo = TeamDataUtility.IdolDict[myIdolIndex]; // 尋找對應的角色資料
        if (characterInfo == null)
        {
            Debug.LogError($"找不到角色資料：{myIdolIndex}");
            return;
        }

        UpdateVigourBar();
    }

    public void UpdateVigourBar(ActivityDropZoneType zoneType = ActivityDropZoneType.Member)
    {
        if (characterInfo == null || activityData == null) return;

        vigourSlider.maxValue = characterInfo.vigourMax; // 設定體力值 UI 的最大值

        if (zoneType == ActivityDropZoneType.Activity)
        {
            vigourSlider.GetComponent<RectTransform>().anchoredPosition = sendPosition; // 移動到外出商演位置

            vigourSlider.value = characterInfo.vigour - activityData.vigourCost;
            var ratio = (float)characterInfo.vigour / characterInfo.vigourMax;
            lastFillImage.rectTransform.anchorMax = new Vector2(ratio, 1);
            lastFillImage.rectTransform.offsetMax = Vector2.zero; // 確保圖片緊貼錨點
        }
        else
        {
            vigourSlider.GetComponent<RectTransform>().anchoredPosition = teamPosition; // 移回隊伍位置

            vigourSlider.value = characterInfo.vigour; // 更新體力值 UI 的當前值
            lastFillImage.rectTransform.anchorMax = new Vector2(0, 1);
            lastFillImage.rectTransform.offsetMax = Vector2.zero; // 確保圖片緊貼錨點

            if (characterInfo.vigour < activityData.vigourCost) // 體力不夠無法外出商演
            {
                grayEffect.SetGrayScale(true, false); // 使用灰階效果＆禁止拖曳
                fillImage.GetComponent<UIGrayEffect>().SetGrayScale(true, false);
            }
            else // 體力足夠可以外出商演
            {
                grayEffect.SetGrayScale(false, true); // 取消灰階效果＆允許拖曳
                fillImage.GetComponent<UIGrayEffect>().SetGrayScale(false, true);
            }
        }
    }
}
