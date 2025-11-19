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
        characterInfo = System.Array.Find(
            TeamDataUtility.IdolInstances, obj 
            => obj.name.Contains(myName)
        ); // 尋找對應的角色資料

        UpdateBenefitBar(trainingUIData);
    }
    
    public void UpdateBenefitBar(TrainingUIData trainingUIData)
    {
        DropZoneType currentZoneType = dragToLesson.CurrentZoneType; // 取得當前拖放區域
        if (currentZoneType == DropZoneType.Member)
        {
            if (trainingUIData.trainingType.ToLower() == "dance")
            {
                benefitText.text = characterInfo.dance.ToString();
            }
            else if (trainingUIData.trainingType.ToLower() == "vocal")
            {
                benefitText.text = characterInfo.vocal.ToString();
            }
            else if (trainingUIData.trainingType.ToLower() == "visual")
            {
                benefitText.text = characterInfo.visual.ToString();
            }
        }
        else if (currentZoneType == DropZoneType.Trainee)
        {
            if (trainingUIData.trainingType.ToLower() == "dance")
            {
                int danceResult = trainingUIData.teacherName != "" ?
                    characterInfo.dance + trainingUIData.withTeacherBenefit:
                    characterInfo.dance + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.dance}>>{danceResult}";
            }
            else if (trainingUIData.trainingType.ToLower() == "vocal")
            {
                int vocalResult = trainingUIData.teacherName != "" ?
                    characterInfo.vocal + trainingUIData.withTeacherBenefit:
                    characterInfo.vocal + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.vocal}>>{vocalResult}";
            }
            else if (trainingUIData.trainingType.ToLower() == "visual")
            {
                int visualResult = trainingUIData.teacherName != "" ?
                    characterInfo.visual + trainingUIData.withTeacherBenefit:
                    characterInfo.visual + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.visual}>>{visualResult}";
            }
        }
    }
}
