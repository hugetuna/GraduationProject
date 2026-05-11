using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

/* 掛在預約視窗根部 */
public class TicketInfoUI : MonoBehaviour
{
    [Header("活動詳情的 UI 設定")]
    [SerializeField] private TextMeshProUGUI activityTitleText; // 活動名稱
    [SerializeField] private Image poster; // 活動圖片
    [SerializeField] private TextMeshProUGUI activityInfoText; // 活動描述
    [SerializeField] private Button joinButton; // 報名按鈕
    [SerializeField] private Button quitButton; // 取消報名按鈕
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject ticketContent; // 活動票券容器（目前只有一個）
    private List<Button> ticketButtons = new(); // 儲存所有活動票券按鈕
    private Dictionary<Button, Activity> ticketActivityDict = new(); // 儲存票券按鈕對應活動的字典
    private Dictionary<Button, bool> ticketStatDict = new(); // 儲存票券報名狀態的字典
    private Button selectedButton; // 當前選擇的票券按鈕
    private Activity selectedActivity = null; // 當前選擇的活動
    //-----------------------------------------------------------------//
    [SerializeField] private Vector2 originalPos = Vector2.zero; // 按鈕們的起始位置
    [SerializeField] private Vector2 offset = new(11.0f, 0); // 被按下的按鈕會往右移動的距離
    //-----------------------------------------------------------------//
    [Header("不同的票券外觀")] // 都是照 TicketColor enum 的順序擺放
    [SerializeField] private List<Sprite> ticketSprites = new();
    [SerializeField] private List<Sprite> selectedTicketSprites = new();
    [SerializeField] private List<GameObject> minTicketPrefabs = new();
    [SerializeField] private List<GameObject> DayContents; // 用於放置最小化活動圖示的容器（照日期順序）
    private Dictionary<Button, Button> ticketMinDict = new(); // 儲存票券按鈕對應的最小化圖示字典
    //-----------------------------------------------------------------//
    [Header("音效設定")]
    [SerializeField] private AudioClip joinSound;

    void Start()
    {
        joinButton.gameObject.SetActive(true);
        joinButton.onClick.AddListener(OnJoinActivity); // 註冊按鈕點擊事件
        joinButton.interactable = false; // 一開始不可點擊（畢竟什麼活動都沒選）

        quitButton.gameObject.SetActive(false);
        quitButton.onClick.AddListener(OnQuitActivity); // 註冊按鈕點擊事件
    }

    public void Initialize()
    {
        // 找出所有活動票券按鈕
        ticketButtons.Clear();
        Button[] buttons = ticketContent.GetComponentsInChildren<Button>();
        foreach (Button btn in buttons)
        {
            ticketButtons.Add(btn);
            btn.onClick.AddListener(() => UpdateTicketInfoUI(btn));
        }

        // 初始化前先清空字典，避免重複添加
        ticketActivityDict.Clear();
        ticketStatDict.Clear();
        ticketMinDict.Clear();

        // 初始化票券按鈕對應活動＆報名狀態的字典
        foreach (Button btn in ticketButtons)
        {
            Activity act = btn.GetComponent<SetTicketUI>().GetActivity();
            ticketActivityDict[btn] = act; // 一個票券按鈕對應一個活動

            bool isAppointed = GameManager.Instance.activitySaveData.IsActivityAppointed(act);

            if (isAppointed)
            {
                RestoreAppointedActivityUI(btn, act); // 回復已預約的活動票券 UI
            }
            else
            {
                ticketStatDict[btn] = false; // 預設未報名狀態
            }
        }

        // 初始化活動詳情 UI
        poster.sprite = null;
        activityTitleText.text = "";
        activityInfoText.text = "";

        activityTitleText.ForceMeshUpdate();
        activityInfoText.ForceMeshUpdate();
    }

    public void UpdateTicketInfoUI(Button clickedButton, Activity clickedMinTicket = null)
    {
        for (int i = 0; i < ticketButtons.Count; i++)
        {
            // 一般按鈕
            RectTransform rt = ticketButtons[i].GetComponent<RectTransform>();
            rt.localPosition = originalPos;
        }

        if (clickedButton == null && clickedMinTicket != null)
        {
            // 被按下的是最小化票券
            selectedActivity = clickedMinTicket;

            // 讓對應的票券按鈕呈現被選擇的狀態（唯一）
            foreach (Button btn in ticketButtons)
            {
                if (ticketActivityDict[btn] == clickedMinTicket)
                {
                    selectedButton = btn;
                    RectTransform clickedRt = btn.GetComponent<RectTransform>();
                    clickedRt.localPosition = originalPos + offset;
                    break;
                }
            }
        }
        else
        {
            // 被按下的按鈕（唯一）
            RectTransform clickedRt = clickedButton.GetComponent<RectTransform>();
            clickedRt.localPosition = originalPos + offset;

            selectedButton = clickedButton;
            selectedActivity = ticketActivityDict[clickedButton];
        }

        // 更新 UI 顯示
        activityTitleText.text = selectedActivity.activityName;
        activityInfoText.text = selectedActivity.description;
        poster.sprite = selectedActivity.activityImage;

        // 確保字型正確渲染
        activityTitleText.ForceMeshUpdate();
        activityInfoText.ForceMeshUpdate();

        // 報名按鈕處理
        if (ticketStatDict[selectedButton]) // 若選中已報名的活動
        {
            // 隱藏報名按鈕，顯示出藏在後面的取消報名按鈕
            joinButton.gameObject.SetActive(false);
            quitButton.gameObject.SetActive(true);
        }
        else // 若選中尚未報名的活動
        {
            joinButton.gameObject.SetActive(true);
            quitButton.gameObject.SetActive(false);
        }
        joinButton.interactable = true; // 選擇任意活動，報名按鈕始可點擊
    }

    public void OnJoinActivity()
    {
        if (joinSound != null) AudioManager.Instance.PlaySFX(joinSound); // 播放音效
        ticketStatDict[selectedButton] = true; // 標記該票券為已報名

        // 改變當下被選擇的票券外觀
        TicketColor theme = selectedButton.GetComponent<SetTicketUI>().GetTicketColor();
        selectedButton.GetComponent<Image>().sprite = selectedTicketSprites[(int)theme];

        // 生成最小化活動圖示到對應日期上
        int daydiff = Math.Abs(selectedActivity.day - DayManager.Instance.date);
        GameObject minTicket = Instantiate(minTicketPrefabs[(int)theme], DayContents[daydiff].transform);

        // 設定最小化活動圖示的內容
        minTicket.GetComponentInChildren<SetMinTicketUI>().Initialize(selectedActivity);

        // 登記到字典中
        ticketMinDict[selectedButton] = minTicket.GetComponentInChildren<Button>();

        // 隱藏報名按鈕，顯示出藏在後面的取消報名按鈕
        joinButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(true);

        // 跨場景的資料儲存
        GameManager.Instance.SaveActivityData(selectedActivity);
    }

    public void OnQuitActivity()
    {
        if (joinSound != null) AudioManager.Instance.PlaySFX(joinSound, setPitch: 0.8f); // 播放音效
        ticketStatDict[selectedButton] = false; // 標記該票券為未報名

        // 將當下被選擇的票券外觀，回復成原本的樣子
        TicketColor theme = selectedButton.GetComponent<SetTicketUI>().GetTicketColor();
        selectedButton.GetComponent<Image>().sprite = ticketSprites[(int)theme];

        // 移除最小化活動圖示（別忘了 Wrapper + Button）
        Transform minTicketContent = ticketMinDict[selectedButton].transform.parent;
        Destroy(minTicketContent.gameObject);
        ticketMinDict.Remove(selectedButton);

        // 取消報名後，維持上一次選擇的 UI 狀態
        // 只有 joinButton 和 quitButton 需要更新
        joinButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(false);

        // 跨場景的資料儲存
        GameManager.Instance.activitySaveData.RemoveActivity(selectedActivity);

        // // 取消報名後，重置活動詳情＆票券 UI
        // for (int i = 0; i < ticketButtons.Count; i++)
        // {
        //     // 一般按鈕
        //     RectTransform rt = ticketButtons[i].GetComponent<RectTransform>();
        //     rt.localPosition = originalPos;
        // }
        // poster.sprite = null;
        // activityTitleText.text = "";
        // activityInfoText.text = "";
        // activityTitleText.ForceMeshUpdate();
        // activityInfoText.ForceMeshUpdate();

        // // joinButton 顯示出來，並且不可點擊直到重新選擇任何活動
        // joinButton.gameObject.SetActive(true);
        // quitButton.gameObject.SetActive(false);
        // joinButton.interactable = false;
    }

    private void RestoreAppointedActivityUI(Button btn, Activity act)
    {
        ticketStatDict[btn] = true;

        // 1. 恢復票券按鈕的摺痕外觀
        TicketColor theme = btn.GetComponent<SetTicketUI>().GetTicketColor();
        btn.GetComponent<Image>().sprite = selectedTicketSprites[(int)theme];

        // 2. 生成最小化活動圖示到日曆
        int daydiff = act.day - DayManager.Instance.date;
        if (daydiff >= 0 && daydiff <= 5)
        {
            GameObject minTicket = Instantiate(minTicketPrefabs[(int)theme], DayContents[daydiff].transform);
            minTicket.GetComponentInChildren<SetMinTicketUI>().Initialize(act);

            // 登記到字典，這樣之後如果要取消報名才找得到物件
            ticketMinDict[btn] = minTicket.GetComponentInChildren<Button>();

            // 最小化票券有自行設定點擊事件，不用另外撰寫
        }
    }
}
