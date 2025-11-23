using UnityEngine;
using UnityEngine.UI;

public class VigourBar : MonoBehaviour
{
    private IdolInstance characterInfo; // 該角色的數值資料
    // public IdolInstance CharacterInfo
    // {
    //     get { return characterInfo; }
    // }
    private Image characterImage; // 取得該角色的圖片參考
    private DragToLesson dragToLesson; // 取得該角色的 DragToLesson 參考（判斷當前拖曳區域）
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Image lastVigourFill; // 在訓練區代表訓練前的體力條
    private Image fillImage; // 表示訓練後的體力變化
    private bool isAbleToTrain = true; // 是否能進行訓練
    [SerializeField] private Material grayMaterial; // 灰階材質

    void Awake()
    {
        characterImage = GetComponent<Image>();
        dragToLesson = GetComponent<DragToLesson>();
        fillImage = vigourSlider.fillRect.GetComponent<Image>();
    }

    public void Initialize(string myName)
    {
        if(characterImage.sprite != null)
        {
            characterInfo = TeamDataUtility.IdolInstances[myName]; // 尋找對應的角色資料
        }

        vigourSlider.maxValue = characterInfo.vigourMax; // 設定體力值 UI 的最大值
        vigourSlider.value = characterInfo.vigour; // 設定體力值 UI 的當前值（結算前不會做任何實質更改）

        // 設定消耗的體力背景（理論上只須設定一次即可）
        float ratio = (float)vigourSlider.value / vigourSlider.maxValue;

        RectTransform barRect = lastVigourFill.rectTransform;

        // 設定 anchor 範圍：從左邊開始（0 到 ratio）
        barRect.anchorMin = new Vector2(0f, 0f);
        barRect.anchorMax = new Vector2(ratio, 1f);

        // 確保 sizeDelta 為 0，才能正確由 anchor 控制大小
        barRect.offsetMin = Vector2.zero;
        barRect.offsetMax = Vector2.zero;
    }

    public void UpdateVigourBar(TrainingUIData trainingUIData)
    {
        // 在兩個區域間拖曳的體力條更新
        if (characterInfo.vigour < trainingUIData.neededVigour)
        {
            isAbleToTrain = false; // 體力不足無法訓練
            characterImage.GetComponent<CanvasGroup>().interactable = false;
            characterImage.material = grayMaterial; // 使用灰階材質
            fillImage.color = new Color32(240, 58, 106, 255); // 將不足以訓練的體力設成桃紅色
        }
        else
        {
            isAbleToTrain = true; // 有足夠體力進行訓練
            characterImage.GetComponent<CanvasGroup>().interactable = true;
            characterImage.material = null;
            fillImage.color = new Color32(214, 189, 255, 255);
        }

        DropZoneType currentZoneType = dragToLesson.CurrentDropZone.zoneType; // 取得當前拖放區域名稱
        if (currentZoneType == DropZoneType.Member)
        {
            vigourSlider.value = characterInfo.vigour;
        }
        else if (currentZoneType != DropZoneType.Member && isAbleToTrain)
        {
            vigourSlider.value = characterInfo.vigour - trainingUIData.neededVigour;
        }
    }
}
