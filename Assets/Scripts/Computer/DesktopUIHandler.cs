using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;

/* 掛在 UIManager 等物件上，不得掛在 UI（視窗）本身 */
public class DesktopUIHandler : MonoBehaviour
{
    [Header("電腦桌面與其底下 UI")]
    public GameObject desktopUI;
    [SerializeField] private Button powerButton;
    // [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject settleUI; // 按下 powerButton 後跳出結算畫面
    [SerializeField] private GameObject demonPet; // 惡魔桌寵（第一天不顯示）
    //-----------------------------------------------------------------//
    [Header("角色控制")]
    public TeamManager teamManager; // 透過 TeamManager 物件取得當前隊伍成員
    private List<PlayerControlMainWorld> teamMembers = new(); // 記錄取得的隊伍成員
    // private List<PlayerInput> playerInputs = new(); // 玩家輸入系統
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip turnOnSound;
    //-----------------------------------------------------------------//
    [Header("受影響的物件")]
    [SerializeField] private List<GameObject> sceneObjects; // 避免場景物件和電腦介面彼此穿模
    // public static event Action OnDesktopUIClosed; // 關閉桌面 UI 事件

    void Awake()
    {
        // 初始為透視投影
        Camera.main.orthographic = false;
    }

    void Start()
    {
        desktopUI.SetActive(false); // 初始隱藏桌面 UI
        settleUI.SetActive(false); // 初始隱藏結算畫面

        ComputerInteraction.OnComputerInteracted += ShowDesktopUI; // 訂閱並監聽與電腦互動事件
        powerButton.onClick.AddListener(TurnOffComputer); // 設置關機按鈕點擊事件
    }

    void OnDestroy()
    {
        ComputerInteraction.OnComputerInteracted -= ShowDesktopUI; // 取消訂閱與電腦互動事件
    }

    private void ShowDesktopUI()
    {
        UIAndPlayerInput.DisableAllPlayerInputs(); // 禁用所有玩家的輸入系統
        foreach (GameObject obj in sceneObjects)
        {
            obj.SetActive(false); // 隱藏場景物件避免穿模（進結算換天後就會恢復了）
        }

        desktopUI.SetActive(true); // 顯示桌面 UI
        AudioManager.Instance.PlaySFX(turnOnSound); // 播放開機音效
        
        Camera.main.orthographic = true; // 切換成平行投影

        if(DayManager.Instance.totalDays == 1) demonPet.SetActive(false); // 第一天不顯示惡魔桌寵

        teamMembers = teamManager.teamMembers; // 獲取當前隊伍成員
        foreach (PlayerControlMainWorld member in teamMembers)
        {
            member.transform.position=new Vector3(0,0,0); // 將角色移出畫面避免遮擋 UI
            member.gameObject.SetActive(false); // 隱藏角色避免遮擋 UI
        }
    }

    private void TurnOffComputer()
    {
        UIAndPlayerInput.EnableAllPlayerInputs(); // 啟用所有玩家的輸入系統

        // startMenu.SetActive(false); // 關閉開始選單
        desktopUI.SetActive(false); // 關閉電腦桌面 UI

        settleUI.SetActive(true); // 開啟結算畫面
        settleUI.GetComponent<SetSettleUI>().ShowTodayBenefits(); // 設定裡面的 UI

        // OnDesktopUIClosed?.Invoke(); // 觸發關閉桌面 UI 事件

        // 切換成透視投影
        Camera.main.orthographic = false;

        // teamMembers = teamManager.teamMembers; // 獲取當前隊伍成員
        // foreach (PlayerControlMainWorld member in teamMembers)
        // {
        //     member.gameObject.SetActive(true); // 正常顯示角色
        // }
    }

    [ContextMenu("Test-Appoint Teacher")] // 加快測試流程用
    public void AppointTestTeacher()
    {
        var teacher = new TeacherInfo("Michael", TrainingType.Dance);
        GameManager.Instance.SaveTeacherData(teacher); // 同步更新存檔
        Debug.Log($"預約了測試用老師：{teacher.teacherName}，訓練類型：{teacher.trainingType}");
    }
}
