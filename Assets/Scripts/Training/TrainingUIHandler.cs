using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/* 掛在 TrainingManager 上 */
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
    [Header("介面所需資料")]
    [SerializeField] private TrainingUIData trainingUIData; // 訓練 UI 的資料 ScriptableObject
    [SerializeField] private TeamManager teamManager; // 透過 TeamManager 物件取得當前隊伍成員
    [SerializeField] private List<Sprite> characterSprites = new(); // 角色 UI 圖片
    private TeamData teamData; // 隊伍資料 ScriptableObject（從 trainingUIData 取得）
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟訓練 UI 的音效

    void Start()
    {
        DoorInteraction.OnDoorInteracted += ShowTrainingUI; // 訂閱並監聽與門互動事件
        closeButton.onClick.AddListener(CloseTrainingUI); // 設定關閉按鈕的監聽事件

        trainingUI.SetActive(false); // 預設關閉訓練 UI

        teamData = trainingUIData.teamData = ScriptableObject.CreateInstance<TeamData>();
        teamData.Initialize(teamManager, characterSprites); // 初始化隊伍資料
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 點擊非 UI 區域時關閉 UI
        {
            if(!IsCursorClickUIObject() && trainingUI.activeSelf) CloseTrainingUI();
        }

        if (trainingUI.activeSelf) // 根據訓練 UI 的開啟狀態，決定是否禁用角色移動
        {
            UIAndPlayerInput.DisableAllPlayerInputs(); // UI 已開啟 -> 禁用所有玩家移動
        }
        else
        {
            UIAndPlayerInput.EnableAllPlayerInputs(); // UI 已關閉 -> 啟用所有玩家移動
        }
    }

    void OnDestroy()
    {
        DoorInteraction.OnDoorInteracted -= ShowTrainingUI; // 取消訂閱與門互動事件
    }

    private void ShowTrainingUI()
    {
        Debug.Log("開啟訓練 UI");
        trainingUI.SetActive(true);
        AudioManager.Instance.PlaySFX(openSound);

        TypeText.text = trainingUIData.trainingType; // 設定訓練類型的 UI 文字內容
        if (trainingUIData.teacherName != null) // 設定老師的 UI 文字內容
        {
            // 暫時寫死為一星的 Amy 老師
            trainingUIData.teacherName = "Amy";
            TeacherText.text = $"老師：{trainingUIData.teacherName} 星1";
            BenefitText.text = $"基本收益：{trainingUIData.withTeacherBenefit}"; // 設定基本收益的 UI 文字內容
        }
        else
        {
            TeacherText.text = "老師：無";
            BenefitText.text = $"基本收益：{trainingUIData.basicBenefit}"; // 設定基本收益的 UI 文字內容
        }
        VigourText.text = $"耗費體力：{trainingUIData.neededVigour}"; // 設定耗費體力的 UI 文字內容

        List<Sprite> sprites = teamData.GetAllCharacterSprites();
        for (int i = 0; i < characterImages.Count; i++)
        {
            if (i < sprites.Count) characterImages[i].sprite = sprites[i];
            else characterImages[i].sprite = null; // 超出範圍的圖片插槽設為空，避免報錯
        }
    }

    private void CloseTrainingUI()
    {
        Debug.Log("關閉訓練 UI");
        trainingUI.SetActive(false);
        OnTrainingUIClosed?.Invoke(teamManager, trainingUIData); // 觸發訓練 UI 關閉事件
    }

    private bool IsCursorClickUIObject()
    {
        // 根據當前操作，設定滑鼠或觸控位置
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // RaycastAll 會從 eventData 中的滑鼠位置發射一條射線，檢測所有碰撞的 UI 元素
        // 符合條件的 UI 元素會被加到 raycastResults 清單中
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults.Count > 0;
    }
}
