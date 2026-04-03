using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在 UIManager 底下 */
public class ActivityAssignment : MonoBehaviour
{
    private Activity todayActivity; // 從 SetActivityUI 的事件接收的商演資訊
    //-----------------------------------------------------------------//
    [Header("商演提示 UI")]
    [SerializeField] private GameObject hintObject; // 全員商演的提示物件
    [SerializeField] private Button hintNoBtn; // 提示的 "否" 按鈕
    [SerializeField] private Button hintYesBtn; // 提示的 "是" 按鈕
    //-----------------------------------------------------------------//
    [Header("隊伍管理")]
    [SerializeField] private TeamManager teamManager; // 用來標記忙碌角色（跨場景同步）

    void Start()
    {
        SetActivityUI.OnActivityConfirmed += ShowHintUI;
        hintNoBtn.onClick.AddListener(CloseHintUI);
        hintYesBtn.onClick.AddListener(AssignToActivity);
        hintObject.SetActive(false);
    }

    void OnDestroy()
    {
        SetActivityUI.OnActivityConfirmed -= ShowHintUI;
    }

    public void ShowHintUI(Activity todayActivity)
    {
        hintObject.SetActive(true); // 先跳出提示，告知玩家必須全員商演的狀況
        this.todayActivity = todayActivity; // 儲存商演資訊以供後續指派使用
    }

    public void CloseHintUI()
    {
        hintObject.SetActive(false);
    }
    
    public void AssignToActivity() 
    {
        // 必須全員一起去商演，若有人體力不足預計會減少收益
        Debug.Log("全員商演 Go");

        var idolList = TeamDataUtility.IdolInstanceList;
        foreach (var idol in idolList)
        {
            // 記下角色的商演參加紀錄（結算時再處理耗體與收益）
            idol.activityRecord.SetActivityRecord(todayActivity);

            // 召回所有成員，無論她們原本在做什麼
            var control = idol.GetComponent<PlayerControlMainWorld>();
            teamManager.RemoveBusyMember(control);
            idol.gameObject.SetActive(true);
            idol.isAvailable = true;

            // 前往 live 小遊戲
        }
    }
}
