using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

/* 掛在聊天室視窗的 Rightside 聊天室上 */
public class ChatRoomController : MonoBehaviour
{
    [Header("回應選項按鈕列表")]
    [SerializeField] private List<Button> respondButtons;
    //-----------------------------------------------------------------//
    private ChatBubbleManager chatBubbleManager;
    private UserRuntime currentUserRuntime = null; // 目前正在聊天的用戶（動態資料）
    private UserRuntime lastEnabledUserRuntime = null; // 「上一次視窗開啟時」正在聊天的用戶
    private bool waitingForChoice = false; // 是否正在等待玩家選擇
    //-----------------------------------------------------------------//
    [Header("聊天間隔")]
    [SerializeField] private float firstLineDelay = 0.5f;
    private float autoPlayInterval;
    private Coroutine continueCoroutine = null; // 記錄 Coroutine 以方便取消

    void Start()
    {
        chatBubbleManager = ChatBubbleManager.Instance;
    }

    void OnEnable() // 僅限同檔案同天同場景的視窗開關
    {
      if(lastEnabledUserRuntime != null)
        {
            // 重新啟用聊天室時，如果有正在聊天的用戶，則繼續顯示對話內容
            StartChatting(lastEnabledUserRuntime);
        }
    }

    void OnDisable() // 僅限同檔案同天同場景的視窗開關
    {
        // 停止所有對話協程
        if (continueCoroutine != null) StopCoroutine(continueCoroutine);
        continueCoroutine = null;

        lastEnabledUserRuntime = currentUserRuntime; // 記錄當前用戶狀態
        currentUserRuntime = null; // 清空目前用戶引用，以便下次視窗開啟時重新設定狀態
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Z) && !waitingForChoice)
    //     {
    //         if (currentUserRuntime == null)
    //         {
    //             Debug.LogWarning("缺少聊天對象，無法繼續對話");
    //             return;
    //         }
    //         ContinueStory(); // 按下 Z 鍵時繼續對話
    //     }
    // }

    public void StartChatting(UserRuntime userRuntime)
    {
        // 檢查是否點擊了「同一個」正在對話的人
        if (currentUserRuntime == userRuntime)
        {
            // 如果是，只須確保已讀，不用重新啟動協程
            if (userRuntime.isUnread)
            {
                userRuntime.isUnread = false;
                SetUserUI.InvokeUserStatusChanged(userRuntime);
            }
            return;
        }

        // 處理「舊用戶」狀態
        if (currentUserRuntime != null)
        {
            // 為選取狀態 UI 預留位置
            SetUserUI.InvokeUserStatusChanged(currentUserRuntime);
        }

        // 設定「新用戶」狀態
        userRuntime.isUnread = false;
        currentUserRuntime = userRuntime;
        SetUserUI.InvokeUserStatusChanged(currentUserRuntime);

        // 為新用戶重建 UI 與歷史紀錄
        chatBubbleManager.ClearAllBubbles();
        chatBubbleManager.RebuildFromHistory(currentUserRuntime.chatHistory);

        // 啟動新的對話協程
        if (continueCoroutine != null) StopCoroutine(continueCoroutine);
        continueCoroutine = StartCoroutine(ContinueStoryCoroutine());

        waitingForChoice = false;


        //     // 設定點擊用戶的已讀狀態（無論有沒有實際切換）
        //     userRuntime.isUnread = false;
        //     SetUserUI.InvokeUserStatusChanged(userRuntime);

        //     // 處理舊用戶的狀態存檔
        //     if (currentUserRuntime != null && currentUserRuntime != userRuntime)
        //     {
        //         SetUserUI.InvokeUserStatusChanged(currentUserRuntime);
        //     }

        //     // 確認是否切換用戶並更新目前引用
        //     bool isSwitchingUser = currentUserRuntime == null || currentUserRuntime.user != userRuntime.user;
        //     currentUserRuntime = userRuntime;

        //     // 若用戶切換，重建對話 UI 與播放故事
        //     if (isSwitchingUser)
        //     {
        //         chatBubbleManager.ClearAllBubbles();
        //         chatBubbleManager.RebuildFromHistory(currentUserRuntime.chatHistory);
        //     }

        //     // 啟動或重啟對話協程
        //     if (continueCoroutine != null) StopCoroutine(continueCoroutine);
        //     continueCoroutine = StartCoroutine(ContinueStoryCoroutine());

        //     waitingForChoice = false;
    }

    private IEnumerator ContinueStoryCoroutine()
    {
        Story story = currentUserRuntime.story; // 取得對話物件

        // 剛切換到特定頁面的第一句延遲
        if (firstLineDelay > 0f)
            yield return new WaitForSeconds(firstLineDelay);

        while (true)
        {
            // 清空所有按鈕文字並關閉互動
            foreach (var button in respondButtons)
            {
                button.interactable = false;
                button.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

            // 若故事尚未結束，就顯示對話
            if (story.canContinue)
            {
                ShowAndRecordStory(story); // 顯示對話內容、處理標籤並儲存對話歷史

                // 自動播放下一句
                autoPlayInterval = Random.Range(1.0f, 3.0f);
                yield return new WaitForSeconds(autoPlayInterval);
            }
            else if (story.currentChoices.Count > 0)
            {
                // 遇到選項就停止 Coroutine
                waitingForChoice = true;

                for (int i = 0; i < story.currentChoices.Count; i++)
                {
                    var choice = story.currentChoices[i];
                    var button = respondButtons[i];

                    button.interactable = true;
                    button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text.Trim();

                    button.onClick.RemoveAllListeners();
                    int choiceIndex = i;
                    button.onClick.AddListener(() =>
                    {
                        ChooseOption(choiceIndex);
                        // 選完後自動繼續故事
                        if (continueCoroutine != null) StopCoroutine(continueCoroutine);
                        continueCoroutine = StartCoroutine(ContinueStoryCoroutine());
                    });
                }

                yield break; // 停止 Coroutine 等待玩家選擇
            }
            else
            {
                // 故事完全結束，停止整個 Coroutine
                // chatBubbleManager.AddBubble("＊此次對話已結束＊", false);
                yield break;
            }
        }
    }

    private void ShowAndRecordStory(Story story)
    {
        string text = story.Continue().Trim();

        bool isPlayer = false;

        string teacherName = "";
        TrainingType teacherType = TrainingType.None;

        foreach (var tag in story.currentTags) // 處理標籤
        {
            var parts = tag.Split(':');
            if (parts.Length == 2)
            {
                string key = parts[0].Trim(); // 開頭一律小寫
                string value = parts[1].Trim(); // 開頭一律大寫

                if (key == "speaker") isPlayer = value == "Player";

                if (key == "appointTrName") teacherName = value;
                if (key == "appointTrType")
                {
                    teacherType = value switch
                    {
                        "Dance" => TrainingType.Dance,
                        "Vocal" => TrainingType.Vocal,
                        "Visual" => TrainingType.Visual,
                        _ => TrainingType.None
                    };
                }
            }
        }

        // 通知 UI 更新狀態
        currentUserRuntime.lastMessageText = text;
        currentUserRuntime.lastMessageDay = DayManager.Instance.date;
        SetUserUI.InvokeUserStatusChanged(currentUserRuntime);

        // 新增對話泡泡，並隨時記錄對話歷史
        chatBubbleManager.AddBubble(text, isPlayer);
        currentUserRuntime.AddToChatHistory(text, isPlayer);
        currentUserRuntime.SaveState();

        // 處理預約訓練老師
        if (teacherName != "" && teacherType != TrainingType.None)
        {
            var teacher = new TeacherInfo(teacherName, teacherType);
            GameManager.Instance.SaveTeacherData(teacher); // 同步更新存檔
            Debug.Log($"預約了老師：{teacherName}，訓練類型：{teacherType}");
        }
    }

    private void ChooseOption(int index) // 選完選項的後續處理
    {
        if (!waitingForChoice) return;

        waitingForChoice = false;

        // 禁用所有選項避免重複點擊
        foreach (var button in respondButtons)
        {
            button.interactable = false;
        }

        currentUserRuntime.story.ChooseChoiceIndex(index);
    }
}
