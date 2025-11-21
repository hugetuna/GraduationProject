using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/* 掛在 TrainingManager 底下，分別控制三種不同的 UI */
public class TrainingUIHandler : MonoBehaviour
{
    public static event Action<TeamManager, TrainingUIData> OnTrainingUIClosed; // 定義訓練 UI 關閉事件
    //-----------------------------------------------------------------//
    [Header("訓練 UI 元素")]
    [SerializeField] private GameObject trainingUI; // 直接使用場景中的，不必另外生成
    //-----------------------------------------------------------------//
    [SerializeField] private Button closeButton; // 關閉 UI 的叉叉按鈕
    [SerializeField] private TextMeshProUGUI TypeText;
    [SerializeField] private TextMeshProUGUI TeacherText;
    [SerializeField] private TextMeshProUGUI VigourText;
    [SerializeField] private TextMeshProUGUI BenefitText;
    [SerializeField] private List<Image> characterImages = new(); //  UI 上的（角色）圖片插槽
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟訓練 UI 的音效
    //-----------------------------------------------------------------//
    private TrainingUIData trainingUIData; // 訓練 UI 的資料 ScriptableObject
    private TeamManager teamManager; // 透過 TeamManager 物件取得當前隊伍成員
    private bool isInitialized = false; // 確保訓練 UI 只初始化一次

    void Start()
    {
        // DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 訂閱並監聽與門互動事件
        closeButton.onClick.AddListener(CloseTrainingUI); // 設定關閉按鈕的監聽事件

        trainingUI.SetActive(false); // 預設關閉訓練 UI
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 點擊非 UI 區域時關閉 UI
        {
            if (!UIAndPlayerInput.IsCursorClickUIObject() && trainingUI.activeSelf)
            {
                CloseTrainingUI();
            }
        }
    }

    // void OnDestroy()
    // {
    //     DoorInteraction.OnDoorInteracted -= ShowTrainingUI; // 取消訂閱與門互動事件
    // }

    public void ShowTrainingUI(TrainingUIData data, TeamManager tm, List<Sprite> characterSprites)
    {
        trainingUIData = data;
        teamManager = tm;

        Debug.Log("開啟訓練 UI");
        trainingUI.SetActive(true);
        AudioManager.Instance.PlaySFX(openSound);

        TypeText.text = trainingUIData.trainingType; // 設定訓練類型的 UI 文字內容
        if (trainingUIData.teacherName != null) // 設定老師的 UI 文字內容
        {
            // 暫時寫死老師的名稱和星等
            TeacherText.text = $"老師：{trainingUIData.teacherName} 星1";
            BenefitText.text = $"基本收益：{trainingUIData.withTeacherBenefit}"; // 設定基本收益的 UI 文字內容
        }
        else
        {
            TeacherText.text = "老師：無";
            BenefitText.text = $"基本收益：{trainingUIData.basicBenefit}"; // 設定基本收益的 UI 文字內容
        }
        VigourText.text = $"耗費體力：{trainingUIData.neededVigour}"; // 設定耗費體力的 UI 文字內容

        for (int i = 0; i < characterImages.Count; i++)
        {
            if (i < characterSprites.Count) characterImages[i].sprite = characterSprites[i];
            else characterImages[i].sprite = null; // 超出範圍的圖片插槽設為空，避免報錯（當圖片少於插槽）
        }

        // 檢查角色是否在隊伍裡，把不在的（UI 物件）隱藏起來
        CheckCharactersInTeam();

        if (!isInitialized)
        {
            DragToLesson[] dragToLessons = trainingUI.GetComponentsInChildren<DragToLesson>();
            foreach (DragToLesson dtl in dragToLessons)
            {
                dtl.Initialize(trainingUIData); // 傳遞 TrainingUIData 給底下的每個 DragToLesson 元件
            }
            isInitialized = true;
        }
    }

    private void CheckCharactersInTeam()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];
            if (img.sprite == null) continue;

            string name = TeamDataUtility.CleanNameOfCharacterUI(img.sprite.name);
            var state = TrainingUIManager.Instance.GetIdolState(name);

            Debug.Log($"[Check] {name} state = {state}");

            if(trainingUIData.trainingType.ToLower() == "dance")
            {
                if(state != IdolTrainingState.InTeam && state != IdolTrainingState.InDance)
                {
                    img.gameObject.SetActive(false);
                }
                else
                {
                    img.gameObject.SetActive(true);
                }
            }
            else if(trainingUIData.trainingType.ToLower() == "vocal")
            {
                if(state != IdolTrainingState.InTeam && state != IdolTrainingState.InVocal)
                {
                    img.gameObject.SetActive(false);
                }
                else
                {
                    img.gameObject.SetActive(true);
                }
            }
            else if(trainingUIData.trainingType.ToLower() == "visual")
            {
                if(state != IdolTrainingState.InTeam && state != IdolTrainingState.InVisual)
                {
                    img.gameObject.SetActive(false);
                }
                else
                {
                    img.gameObject.SetActive(true);
                }
            }
        }
    }

    private void CloseTrainingUI()
    {
        Debug.Log("關閉訓練 UI");
        trainingUI.SetActive(false);
        OnTrainingUIClosed?.Invoke(teamManager, trainingUIData); // 觸發訓練 UI 關閉事件
    }
}
