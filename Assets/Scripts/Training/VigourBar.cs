using UnityEngine;
using UnityEngine.UI;

/* 掛在每個角色圖片 UI 上 */
public class VigourBar : MonoBehaviour
{
    private IdolInstance characterInfo; // 該角色的數值資料
    private TrainingUIData trainingUIData;
    //-----------------------------------------------------------------//
    private Image characterImage; // 取得該角色的圖片參考
    private CanvasGroup canvasGroup;
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Material grayMaterial; // 灰階材質
    // private bool isAbleToTrain = true; // 是否能進行訓練

    void Awake()
    {
        characterImage = GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(TrainingUIData trainingUIData, IdolWho myIdolIndex)
    {
        this.trainingUIData = trainingUIData;

        characterInfo = TeamDataUtility.IdolDict[myIdolIndex]; // 尋找對應的角色資料
        if (characterInfo == null)
        {
            Debug.LogError($"找不到角色資料：{myIdolIndex}");
            return;
        }

        UpdateVigourBar();
    }

    public void UpdateVigourBar()
    {
        if (characterInfo == null || trainingUIData == null) return;

        vigourSlider.maxValue = characterInfo.vigourMax; // 設定體力值 UI 的最大值
        vigourSlider.value = characterInfo.vigour; // 更新體力值 UI 的當前值

        var state = characterInfo.trainRecord.state;
        if (state == IdolTrainingState.InTeam) // 只有在隊伍中才須做體力判定
        {
            if (characterInfo.vigour < trainingUIData.neededVigour)
            {
                // isAbleToTrain = false; // 體力不足無法訓練
                canvasGroup.interactable = false;
                characterImage.material = grayMaterial; // 使用灰階材質
                // fillImage.color = new Color32(240, 58, 106, 255); // 將不足以訓練的體力設成桃紅色
            }
            else
            {
                // isAbleToTrain = true; // 有足夠體力進行訓練
                canvasGroup.interactable = true;
                characterImage.material = null;
                // fillImage.color = new Color32(255, 255, 255, 255); // 預設白色（不影響圖片顯示）
            }
        }
    }
}
