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
    public static event Action<TrainingUIData> OnTrainingUIConfirmed; // 定義確定指派訓練成員事件
    //-----------------------------------------------------------------//
    [Header("訓練 UI 元素")]
    [SerializeField] private GameObject trainingUI; // 直接使用場景中的，不必另外生成
    //-----------------------------------------------------------------//
    [SerializeField] private Button panelBackground; // 點擊背景關閉 UI 的按鈕
    [SerializeField] private Button closeButton; // 關閉 UI 的叉叉按鈕
    [SerializeField] private TextMeshProUGUI TypeText;
    [SerializeField] private TextMeshProUGUI TeacherText;
    [SerializeField] private TextMeshProUGUI VigourText;
    [SerializeField] private List<Image> characterImages = new(); //  UI 上的（角色）圖片插槽
    [SerializeField] private Button confirmButton; // 確定指派按鈕
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟訓練 UI 的音效
    //-----------------------------------------------------------------//
    private TrainingUIData trainingUIData; // 訓練 UI 的資料 ScriptableObject
    private bool isInitialized = false; // 確保訓練 UI 只初始化一次
    //-----------------------------------------------------------------//
    [Header("跳轉提示 UI 元素")]
    [SerializeField] private GameObject hintPrefab; // 跳轉提示的 prefab

    void Start()
    {
        closeButton.onClick.AddListener(CloseTrainingUI); // 設定關閉按鈕的監聽事件
        panelBackground.onClick.AddListener(CloseTrainingUI); // 設定背景按鈕的監聽事件
        confirmButton.onClick.AddListener(ConfirmToAssign); // 設定指派按鈕的監聽事件

        trainingUI.SetActive(false); // 預設關閉訓練 UI
    }

    public void ShowTrainingUI(TrainingUIData data)
    {
        trainingUIData = data;

        Debug.Log("開啟訓練 UI");
        trainingUI.SetActive(true);
        AudioManager.Instance.PlaySFX(openSound);

        TypeText.text = trainingUIData.trainingType; // 設定訓練類型的 UI 文字內容
        if (trainingUIData.teacherName != null) // 設定老師的 UI 文字內容
        {
            TeacherText.text = $"老師：{trainingUIData.teacherName}"; // 暫時寫死老師的名稱
        }
        else
        {
            TeacherText.text = "老師：無";
        }
        VigourText.text = $"耗費體力：{trainingUIData.neededVigour}"; // 設定耗費體力的 UI 文字內容

        UpdateCharacterImagesAndPositions(); // 設定角色 UI 圖片及位置

        if (!isInitialized)
        {
            DragToLesson[] dragToLessons = trainingUI.GetComponentsInChildren<DragToLesson>();
            foreach (DragToLesson dtl in dragToLessons)
            {
                dtl.Initialize(trainingUIData); // 初始化底下每一個 DragToLesson 元件
            }

            var numbersController = trainingUI.GetComponent<NumbersController>();
            numbersController.InitializeSlots(trainingUIData); // 初始化數值顯示
            
            isInitialized = true;
        }
    }

    private void UpdateCharacterImagesAndPositions()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];

            // 為圖片插槽放置角色圖片
            if (i < TeamDataUtility.idolCount)
            {
                img.sprite = TrainingUIManager.Instance.GetCharacterSprite((IdolWho)i);
            }
            else
            {
                // 超出角色範圍就設為空，避免錯誤
                img.sprite = null;
                continue;
            }

            var idolInstance = TeamDataUtility.IdolDict[(IdolWho)i];
            var state = TrainingUIManager.Instance.GetIdolState(idolInstance.idolIndex);

            bool isActive = false;
            string type = trainingUIData.trainingType.ToLower();
            if (type == "dance")
            {
                isActive = state == IdolTrainingState.InTeam || state == IdolTrainingState.InDance;
            }
            else if (type == "vocal")
            {
                isActive = state == IdolTrainingState.InTeam || state == IdolTrainingState.InVocal;
            }
            else if (type == "visual")
            {
                isActive = state == IdolTrainingState.InTeam || state == IdolTrainingState.InVisual;
            }
            img.gameObject.SetActive(isActive);

            // 還原上次的位置
            Vector2 position = idolInstance.trainRecord.position;
            if (position != Vector2.zero) img.transform.localPosition = position;
        }
    }

    private void CloseTrainingUI()
    {
        Debug.Log("關閉訓練 UI");
        trainingUI.SetActive(false);
        OnTrainingUIClosed?.Invoke(); // 觸發訓練 UI 關閉事件
    }

    private void ConfirmToAssign()
    {
        if(TrainingUIManager.Instance.GetMembers().Count == 0)
        {
            // 若全員皆去訓練，觸發可通往電腦場景的 UI
            var hintObj = Instantiate(hintPrefab, trainingUI.transform.parent); // 在 TrainingUI 的父物件下生成提示 UI
            hintObj.transform.SetAsLastSibling(); // 確保提示 UI 在最上層
            hintObj.GetComponent<HintToggler>().SetTrainingUIData(trainingUIData); // 若確定前往電腦介面可先進行訓練結算
        }
        else
        {
            // 若無人去訓練，就什麼也不做（交由 TraineeAssignment 處理）
            // 有任何人去訓練，即可觸發指派訓練成員事件
            OnTrainingUIConfirmed?.Invoke(trainingUIData);
            CloseTrainingUI();
        }
    }
}
