using UnityEngine;
using UnityEngine.UI;

public class VigourBar : MonoBehaviour
{
    private IdolInstance characterInfo; // 該角色的數值資料
    private Image characterImage; // 取得該角色的圖片參考
    //-----------------------------------------------------------------//
    [Header("體力 UI")]
    [SerializeField] private Slider vigourSlider; // 該角色的體力值 UI
    [SerializeField] private Image fillImage; // 體力條填充圖片
    [SerializeField] private Material grayMaterial; // 灰階材質
    // private bool isAbleToTrain = true; // 是否能進行訓練

    void Awake()
    {
        characterImage = GetComponent<Image>();
    }

    public void Initialize(TrainingUIData trainingUIData, IdolWho myIdolIndex)
    {
        characterInfo = TeamDataUtility.IdolDict[myIdolIndex]; // 尋找對應的角色資料
        if(characterInfo == null)
        {
            Debug.LogError($"找不到角色資料：{myIdolIndex}");
            return;
        }
        
        vigourSlider.maxValue = characterInfo.vigourMax; // 設定體力值 UI 的最大值
        vigourSlider.value = characterInfo.vigour; // 設定體力值 UI 的當前值（結算前不會做任何實質更改？）

        var state = characterInfo.trainRecord.state;
        if (state == IdolTrainingState.InTeam) // 只有在隊伍中才須做體力判定
        {
            if (characterInfo.vigour < trainingUIData.neededVigour)
            {
                // isAbleToTrain = false; // 體力不足無法訓練
                characterImage.GetComponent<CanvasGroup>().interactable = false;
                characterImage.material = grayMaterial; // 使用灰階材質
                fillImage.color = new Color32(240, 58, 106, 255); // 將不足以訓練的體力設成桃紅色
            }
            else
            {
                // isAbleToTrain = true; // 有足夠體力進行訓練
                characterImage.GetComponent<CanvasGroup>().interactable = true;
                characterImage.material = null;
                fillImage.color = new Color32(214, 189, 255, 255);
            }
        }
    }
}
