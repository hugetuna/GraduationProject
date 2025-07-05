using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BenefitBar : MonoBehaviour
{
    private TrainingUIData trainingUIData; // 取用當前訓練 UI 資料
    public VigourBar vigourBar; // 取用 VigourBar 的參考
    private IdolInstance characterInfo; // 該角色的數值資料（從 VigourBar 取得）
    public Text benefitText; // 顯示訓練前後的收益文字
    private DragToLesson dragToLesson; // 取得該角色的 DragToLesson 參考

    void Start()
    {
        benefitText.text = "";
        dragToLesson = GetComponent<DragToLesson>();
        trainingUIData = vigourBar.trainingUIData;
    }


    void Update()
    {
        characterInfo = vigourBar.characterInfo;

        DropZoneType currentZoneType = dragToLesson.currentZoneType; // 取得當前拖放區域名稱
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
                int danceResult = trainingUIData.isWithTeacher ?
                    characterInfo.dance + trainingUIData.withTeacherBenefit:
                    characterInfo.dance + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.dance}→{danceResult}";
            }
            else if (trainingUIData.trainingType.ToLower() == "vocal")
            {
                int vocalResult = trainingUIData.isWithTeacher ?
                    characterInfo.vocal + trainingUIData.withTeacherBenefit:
                    characterInfo.vocal + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.vocal}→{vocalResult}";
            }
            else if (trainingUIData.trainingType.ToLower() == "visual")
            {
                int visualResult = trainingUIData.isWithTeacher ?
                    characterInfo.visual + trainingUIData.withTeacherBenefit:
                    characterInfo.visual + trainingUIData.basicBenefit;
                benefitText.text = $"{characterInfo.visual}→{visualResult}";
            }
        }
    }
}
