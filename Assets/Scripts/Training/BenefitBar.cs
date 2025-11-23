using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BenefitBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI benefitText; // 顯示訓練前後的收益文字
    //-----------------------------------------------------------------//
    private IdolInstance characterInfo; // 該角色的數值資料
    private DragToLesson dragToLesson; // 取得該角色的 DragToLesson 參考（判斷當前拖曳區域）

    void Awake()
    {
        dragToLesson = GetComponent<DragToLesson>();
    }

    public void Initialize(string myName, TrainingUIData trainingUIData)
    {
        characterInfo = TeamDataUtility.IdolInstances[myName]; // 尋找對應的角色資料

        // CurrentDropZone 要等到第一次拖曳後才會設定好，這裡先顯示初始數值
        string trainingType = trainingUIData.trainingType.ToLower();
        benefitText.text = trainingType switch
        {
            "dance" => characterInfo.dance.ToString(),
            "vocal" => characterInfo.vocal.ToString(),
            "visual" => characterInfo.visual.ToString(),
            _ => "0"
        };
    }

    public void UpdateBenefitBar(TrainingUIData trainingUIData)
    {
        DropZoneType currentZoneType = dragToLesson.CurrentDropZone.zoneType; // 取得當前拖放區域
        string trainingType = trainingUIData.trainingType.ToLower(); // 取得訓練類型

        if (currentZoneType == DropZoneType.Member)
        {
            benefitText.text = trainingType switch
            {
                "dance" => characterInfo.dance.ToString(),
                "vocal" => characterInfo.vocal.ToString(),
                "visual" => characterInfo.visual.ToString(),
                _ => "0"
            };
        }
        else
        {
            // 理論上還要再計算 buff 效果，但目前還沒實作
            if (trainingType == "dance")
            {
                // 先照著 buffBoard 的內容寫死 10% 的加成
                int danceResult = trainingUIData.teacherName != "" ?
                    (int)(characterInfo.dance + trainingUIData.withTeacherBenefit * 0.1f) :
                    (int)(characterInfo.dance + trainingUIData.basicBenefit * 0.1f);
                benefitText.text = $"{characterInfo.dance}>>{danceResult}";
            }
            else if (trainingType == "vocal")
            {
                int vocalResult = trainingUIData.teacherName != "" ?
                    characterInfo.vocal + trainingUIData.withTeacherBenefit :
                    characterInfo.vocal + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.vocal}>>{vocalResult}";
            }
            else if (trainingType == "visual")
            {
                int visualResult = trainingUIData.teacherName != "" ?
                    characterInfo.visual + trainingUIData.withTeacherBenefit :
                    characterInfo.visual + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.visual}>>{visualResult}";
            }
        }
    }
}
