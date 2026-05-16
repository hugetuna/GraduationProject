using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/* 掛在 TrainingManager 底下，分別控制三種不同的 UI */
public class TrainingUIHandler : MonoBehaviour
{
    public static event Action OnTrainingUIClosed; // 定義訓練 UI 關閉事件
    public static event Action<TrainingUIData, bool> OnTrainingUIConfirmed; // 定義確定指派訓練成員事件
    //-----------------------------------------------------------------//
    [Header("訓練 UI 元素")]
    [SerializeField] private GameObject trainingUI; // 直接使用場景中的，不必另外生成
    //-----------------------------------------------------------------//
    // [SerializeField] private Button panelBackground; // 點擊背景關閉 UI 的按鈕
    [SerializeField] private Button closeButton; // 關閉 UI 的叉叉按鈕
    [SerializeField] private TextMeshProUGUI TypeText;
    [SerializeField] private TextMeshProUGUI TeacherText;
    [SerializeField] private TextMeshProUGUI VigourText;
    [SerializeField] private TextMeshProUGUI BenefitText;
    [SerializeField] private List<Image> characterImages = new(); //  UI 上的（角色）圖片插槽
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟訓練 UI 的音效
    [SerializeField] private AudioClip assignSound; // 按下指派按鈕的音效
    //-----------------------------------------------------------------//
    private TrainingUIData trainingUIData; // 訓練 UI 的資料 ScriptableObject
    // private bool isInitialized = false;
    private string todayTeacherName = "";
    //-----------------------------------------------------------------//
    [Header("跳轉提示 UI 元素")]
    [SerializeField] private GameObject hintPrefab; // 跳轉提示的 prefab

    void Start()
    {
        closeButton.onClick.AddListener(ConfirmToAssign); // 設定關閉按鈕的監聽事件
        // panelBackground.onClick.AddListener(ConfirmToAssign); // 設定背景按鈕的監聽事件
        // confirmButton.onClick.AddListener(ConfirmToAssign); // 設定指派按鈕的監聽事件

        trainingUI.SetActive(false); // 預設關閉訓練 UI
    }

    public void ShowTrainingUI(TrainingUIData data)
    {
        trainingUIData = data;
        trainingUI.SetActive(true);

        Debug.Log("開啟訓練 UI");
        AudioManager.Instance.PlaySFX(openSound);

        //-----------------------------------------------------------------//

        TypeText.text = trainingUIData.trainingType.ToString(); // 設定訓練類型的 UI 文字內容
        FindTodayTeacherAndExplanation(); // // 根據有無老師，設定老師、耗費體力與數值收益的 UI 文字內容

        UpdateCharacterImagesAndPositions(); // 設定角色 UI 圖片及位置

        RefreshDragSystem(); // 初始化或刷新拖曳系統，確保每次開啟 UI 都能正確顯示拖曳功能

        // if (!isInitialized)
        // {
        //     InitializeDragSystem();
        //     isInitialized = true;
        // }

        //-----------------------------------------------------------------//

        CheckUnableState(); // 檢查是否有無法訓練的角色，並套用灰階效果
    }

    private void FindTodayTeacherAndExplanation()
    {
        // 從預約資料中讀取今天的老師名稱（[老師名字] vs. 無）
        var teacherSaveData = GameManager.Instance.teacherSaveData;
        var trainingType = trainingUIData.trainingType;
        if (todayTeacherName == "") todayTeacherName = teacherSaveData.GetTeacherNameByType(trainingType);
        
        if (todayTeacherName != "無") // 今天有老師
        {
            teacherSaveData.SetTeacherLessonCompleted(trainingType); // 標記老師為已使用，避免隔天重複預約
            VigourText.text = $"耗費體力：{trainingUIData.neededVigour - 5}"; 
            BenefitText.text = $"數值收益：{trainingUIData.withTeacherBenefit}";
        }
        else // 今天沒老師
        {
            VigourText.text = $"耗費體力：{trainingUIData.neededVigour}"; 
            BenefitText.text = $"數值收益：{trainingUIData.basicBenefit}";
        }

        TeacherText.text = $"老師：{todayTeacherName}";
        Debug.Log($"今天的老師：{todayTeacherName}");
    }

    private void UpdateCharacterImagesAndPositions()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];
            var idolInstance = TeamDataUtility.IdolDict.ElementAt(i).Value;

            // 為圖片插槽放置角色圖片
            if (i < TeamDataUtility.idolCount)
            {
                img.sprite = idolInstance.idolUISprites.spriteQ;
            }
            else
            {
                // 超出角色範圍就設為空，避免錯誤
                img.sprite = null;
                continue;
            }

            var state = TrainingUIManager.Instance.GetIdolState(idolInstance.idolIndex);

            // 根據角色的訓練狀態決定圖片是否顯示（在訓練中或在隊伍中的角色才顯示）
            bool isActive = false;
            bool isInTeamScope = idolInstance.trainRecord.IsInTeamScope();
            TrainingType type = trainingUIData.trainingType;
            if (idolInstance.CanShowInTheAction(AvailableAction.Train))
            {
                isActive = type switch
                {
                    TrainingType.Dance => isInTeamScope || state == IdolTrainingState.InDance,
                    TrainingType.Vocal => isInTeamScope || state == IdolTrainingState.InVocal,
                    TrainingType.Visual => isInTeamScope || state == IdolTrainingState.InVisual,
                    _ => false
                };
            }
            img.gameObject.SetActive(isActive);

            // 還原上次的位置
            Vector2 position = idolInstance.trainRecord.position;
            if (position != Vector2.zero) img.transform.localPosition = position;
        }
    }

    private void CheckUnableState()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];
            var idolInstance = TeamDataUtility.IdolInstanceList[i];
            var state = TrainingUIManager.Instance.GetIdolState(idolInstance.idolIndex);

            if (state == IdolTrainingState.Unable) // 處理當天無法訓練的角色
            {
                Debug.Log($"角色 {idolInstance.idolIndex} 無法參與訓練，已設為灰階");
                var grayEffect = img.GetComponent<UIGrayEffect>();
                grayEffect.SetGrayScale(true, false); // 使用灰階效果＆禁止拖曳
            }
        }
    }

    private void CloseTrainingUI()
    {
        Debug.Log("關閉訓練 UI");
        trainingUI.SetActive(false);

        OnTrainingUIClosed?.Invoke(); // 觸發訓練 UI 關閉事件
    }

    private void RefreshDragSystem()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            var img = characterImages[i];

            DragToLesson dtl = img.GetComponentInChildren<DragToLesson>();
            TrainingVigourBar vb = img.GetComponentInChildren<TrainingVigourBar>();
            TrainingNumbers tn = img.GetComponentInChildren<TrainingNumbers>();

            var idol = TeamDataUtility.IdolInstanceList[i];
            dtl.Initialize(idol.idolIndex); // 初始化每個角色的拖曳功能
            vb.Initialize(trainingUIData, idol.idolIndex); // 初始化每個角色的體力條
            tn.Initialize(idol.idolIndex, trainingUIData, todayTeacherName != "無"); // 初始化每個角色的數值顯示
        }
    }

    private void ConfirmToAssign()
    {
        if (TrainingUIManager.Instance.GetMembers().Count == 0)
        {
            // 若全員皆去訓練，觸發可通往電腦場景的 UI
            var hintObj = Instantiate(hintPrefab, trainingUI.transform.parent); // 在 TrainingUI 的父物件下生成提示 UI
            hintObj.transform.SetAsLastSibling(); // 確保提示 UI 在最上層
            hintObj.GetComponent<GoToComputerHint>().SetTrainingUIData(trainingUIData); // 若確定前往電腦介面可先進行訓練結算
        }
        else
        {
            // 若無人去訓練，就什麼也不做（交由 TraineeAssignment 處理）
            // 有任何人去訓練，即可觸發指派訓練成員事件
            if (assignSound != null) AudioManager.Instance.PlaySFX(assignSound);
            OnTrainingUIConfirmed?.Invoke(trainingUIData, false);
            CloseTrainingUI();
        }
    }
}
