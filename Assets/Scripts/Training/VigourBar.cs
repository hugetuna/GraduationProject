using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UI;

public class VigourBar : MonoBehaviour
{
    public TrainingUIData trainingUIData; // 取用當前訓練 UI 資料
    private IdolInstance[] idolInstance; // 存放偶像資料參考
    public IdolInstance characterInfo; // 該角色的數值資料
    private string characterName; // 該角色名稱
    private DragToLesson dragToLesson; // 取得該角色的 DragToLesson 參考

    public Slider vigourSlider; // 該角色的體力值 UI
    public Image lastVigourImage; // 在訓練區代表訓練前的體力條
    public bool isAbleToTrain = true; // 是否能進行訓練

    private Image image; // 取得該角色的圖片參考
    public Material grayMaterial; // 灰階材質


    void Start()
    {
        idolInstance = FindObjectsOfType<IdolInstance>(); // 獲取場景中所有具備 IdolInstance 的物件

        image = GetComponent<Image>(); // 取得該角色的圖片參考
        characterName = image.sprite.name; // 取得圖片來源名稱
        characterName = characterName.Replace("UI_character_", ""); // 去除名稱前綴
        IdolWho characterIndex = IdolWho.none;
        if (characterName == "Kuma") characterIndex = IdolWho.Kuma;
        else if (characterName == "Karo") characterIndex = IdolWho.Karo;
        else if (characterName == "Sirius") characterIndex = IdolWho.Sirius;

        for (int i = 0; i < idolInstance.Length; i++) // 確保不會超出陣列範圍
        {
            if (idolInstance[i].idolIndex == characterIndex) // 找到對應的偶像資料
            {
                characterInfo = idolInstance[i]; // 設定角色資訊
                break; // 找到後跳出迴圈
            }
        }

        dragToLesson = GetComponent<DragToLesson>();

        vigourSlider.maxValue = characterInfo.vigourMax; // 設定體力值 UI 的最大值
        vigourSlider.value = characterInfo.vigour; // 設定體力值 UI 的當前值（結算前不會做任何實質更改）
        //vigourSlider.value = vigourSlider.maxValue;

        // 設定消耗的體力背景（理論上只須設定一次即可）
        float ratio = (float)vigourSlider.value / vigourSlider.maxValue;

        RectTransform barRect = lastVigourImage.rectTransform;

        // 設定 anchor 範圍：從左邊開始（0 到 ratio）
        barRect.anchorMin = new Vector2(0f, 0f);
        barRect.anchorMax = new Vector2(ratio, 1f);

        // 確保 sizeDelta 為 0，才能正確由 anchor 控制大小
        barRect.offsetMin = Vector2.zero;
        barRect.offsetMax = Vector2.zero;
    }


    void Update()
    {
        // 在兩個區域間拖曳的體力條更新
        Image fillImage = vigourSlider.fillRect.GetComponent<Image>();
        if (characterInfo.vigour < trainingUIData.neededVigour)
        {
            isAbleToTrain = false; // 體力不足無法訓練
            image.material = grayMaterial; // 使用灰階材質
            fillImage.color = new Color32(240, 58, 106, 255); // 將不足以訓練的體力設成桃紅色
        }
        else
        {
            isAbleToTrain = true; // 有足夠體力進行訓練
            image.material = null;
            fillImage.color = new Color32(214, 189, 255, 255);
        }

        DropZoneType currentZoneType = dragToLesson.currentZoneType; // 取得當前拖放區域名稱
        if (currentZoneType == DropZoneType.Member)
        {
            vigourSlider.value = characterInfo.vigour;
        }
        else if (currentZoneType == DropZoneType.Trainee && isAbleToTrain)
        {
            vigourSlider.value = characterInfo.vigour - trainingUIData.neededVigour;
        }
    }
}
