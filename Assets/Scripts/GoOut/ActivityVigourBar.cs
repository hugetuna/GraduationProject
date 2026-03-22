using UnityEngine;
using UnityEngine.UI;

public class ActivityVigourBar : MonoBehaviour
{
    private IdolInstance characterInfo; // 該角色的數值資料
    private Activity activityData; // 商演資訊
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Image fillImage; // 體力值 UI 中的填充圖片
    [SerializeField] private Image lastFillImage; // 上一次體力值的填充圖片（用來表示體力變化）
    //-----------------------------------------------------------------//
    private Color32 normalColor = Color.white; // 體力正常時的顏色
    [SerializeField] private Color32 tiredColor = new Color32(93, 38, 38, 255); // 體力不足時的顏色
    [SerializeField] private GameObject tireEffect; // 體力不足時的特效物件

    public void Initialize(Activity activityData, IdolInstance idol)
    {
        this.activityData = activityData; // 更新目前商演資訊
        characterInfo = idol; // 使用傳入的角色資料

        UpdateVigourBar(); // 更新體力條顯示
    }

    public void UpdateVigourBar()
    {
        if (characterInfo == null || activityData == null) return; // 簡單的防呆檢查

        // 基礎數值計算
        float max = characterInfo.vigourMax;
        float current = characterInfo.vigour;
        float cost = activityData.vigourCost;
        float preview = current - cost;

        vigourSlider.maxValue = max; // 設定體力值 UI 的最大值

        vigourSlider.value = preview; // 更新體力值 UI 的當前值
        var ratio = Mathf.Clamp01(current / max); // 計算長度比例 (確保不低於 0)
        lastFillImage.fillAmount = ratio; // 顯示體力變化
        
        if(preview < 0)
        {
            fillImage.color = tiredColor; // 體力不足，改變顏色
            tireEffect.SetActive(true); // 顯示疲勞特效
        }
        else
        {
            fillImage.color = normalColor; // 體力足夠，恢復正常顏色
            tireEffect.SetActive(false); // 隱藏疲勞特效
        }
    }
}
