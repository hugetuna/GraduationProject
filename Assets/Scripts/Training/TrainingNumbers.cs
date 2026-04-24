using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

/* 掛在角色 UI 上 */
public class TrainingNumbers : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI 元素")]
    [SerializeField] private TextMeshProUGUI nameText; // 角色名稱
    [SerializeField] private TextMeshProUGUI performanceText; // 角色演技數值
    [SerializeField] private TextMeshProUGUI danceText; // 角色舞蹈數值
    [SerializeField] private TextMeshProUGUI vocalText; // 角色歌唱數值
    [SerializeField] private TextMeshProUGUI visualText; // 角色表現力數值
    [SerializeField] private List<BuffBar> buffBars; // 角色的增益效果顯示欄位
    //-----------------------------------------------------------------//
    [Header("訓練前後的數值標示")]
    Color beforeColor = Color.white; // 訓練前數值的顏色
    [SerializeField] private Color afterColor; // 訓練後數值的顏色
    //-----------------------------------------------------------------//
    private IdolWho idolIndex; // 角色索引
    private IdolInstance characterInfo; // 角色資料
    private TrainingUIData trainingUIdata; // 當前的訓練資訊
    private bool isWithTeacher; // 是否有老師加成

    public void Initialize(IdolWho idolIndex, TrainingUIData data, bool isWithTeacher)
    {
        this.idolIndex = idolIndex;
        characterInfo = TeamDataUtility.IdolDict[idolIndex]; // 尋找對應的角色資料
        trainingUIdata = data;
        this.isWithTeacher = isWithTeacher;
    }

    public void OnPointerEnter(PointerEventData eventData) // 滑鼠移入時顯示角色資料
    {
        var type = trainingUIdata.trainingType;
        var benefit = isWithTeacher ? trainingUIdata.withTeacherBenefit : trainingUIdata.basicBenefit;
        var trainRecord = characterInfo.trainRecord;

        nameText.text = TeamDataUtility.GetIdolNameTW(characterInfo.idolIndex);
        performanceText.text = $"{characterInfo.performance}";

        if (trainRecord.droppedZoneType == DropZoneType.Member)
        {
            danceText.text = $"{characterInfo.dance}"; danceText.color = beforeColor; 
             vocalText.text = $"{characterInfo.vocal}"; vocalText.color = beforeColor; 
             visualText.text = $"{characterInfo.visual}"; visualText.color = beforeColor; 
        }
        else
        {
            if(type == TrainingType.Dance)
            {
                danceText.text = $"{characterInfo.dance + benefit}"; danceText.color = afterColor; 
                vocalText.text = $"{characterInfo.vocal}"; vocalText.color = beforeColor; 
                visualText.text = $"{characterInfo.visual}"; visualText.color = beforeColor; 
            }
            else if(type == TrainingType.Vocal)
            {
                danceText.text = $"{characterInfo.dance}"; danceText.color = beforeColor; 
                vocalText.text = $"{characterInfo.vocal + benefit}"; vocalText.color = afterColor; 
                visualText.text = $"{characterInfo.visual}"; visualText.color = beforeColor; 
            }
            else if(type == TrainingType.Visual)
            {
                danceText.text = $"{characterInfo.dance}"; danceText.color = beforeColor; 
                vocalText.text = $"{characterInfo.vocal}"; vocalText.color = beforeColor; 
                visualText.text = $"{characterInfo.visual + benefit}"; visualText.color = afterColor; 
            }
        }

        var buffNames = ItemEffectUtility.GetTrainingEffectDisplayNames(idolIndex, type);
        for (int i = 0; i < buffBars.Count; i++) // 以 buffBar 物件的數量為上限
        {
            if (i < buffNames.Count)
            {
                buffBars[i].gameObject.SetActive(true);
                buffBars[i].UpdateBuffBar(buffNames[i]); // 更新 buffBar 上的文字（如果有圖示的話可以一起更新）
            }
            else
            {
                buffBars[i].gameObject.SetActive(false); // 多出來的隱藏起來
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) // 滑鼠移開時清除資料顯示
    {
        nameText.text = "";
        performanceText.text = "";
        danceText.text = "";
        vocalText.text = "";
        visualText.text = "";
        foreach (var buffBar in buffBars)
        {
            buffBar.gameObject.SetActive(false);
        }
    }
}
