using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

/* 掛在聊天室視窗的 Rightside 聊天室上（Singleton）*/
public class ChatRoomManager : MonoBehaviour
{
    public static ChatRoomManager Instance; // 唯一實例
    //-----------------------------------------------------------------//
    [Tooltip("用來顯示回應選項的按鈕列表")]
    [SerializeField] private List<Button> respondButtons;
    //-----------------------------------------------------------------//
    private ChatBubbleManager chatBubbleManager;
    private UserRuntime currentUserRuntime = null; // 目前正在聊天的用戶（動態資料）
    private bool waitingForChoice = false; // 是否正在等待玩家選擇

    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    void Start()
    {
        chatBubbleManager = ChatBubbleManager.Instance;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !waitingForChoice)
        {
            if (currentUserRuntime == null)
            {
                Debug.LogWarning("缺少聊天對象，無法繼續對話");
                return;
            }
            ContinueStory(); // 按下 Z 鍵時繼續對話
        }
    }

    public void StartChatting(UserRuntime userRuntime)
    {
        // 儲存舊用戶的狀態
        if (currentUserRuntime != null)
        {
            currentUserRuntime.SaveState();
        }

        // 確認是否切換了不同用戶
        bool isSwitchingUser = currentUserRuntime == null ||
                               currentUserRuntime.user != userRuntime.user;

        // 更新目前用戶
        currentUserRuntime = userRuntime;
        currentUserRuntime.Initialize();

        // 若是不同用戶才重建 UI
        if (isSwitchingUser)
        {
            chatBubbleManager.ClearAllBubbles();
            chatBubbleManager.RebuildFromHistory(currentUserRuntime.chatHistory);
        }

        waitingForChoice = false;
    }

    private void ContinueStory()
    {
        // 清空所有按鈕文字並關閉互動
        foreach (var button in respondButtons)
        {
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        // 若故事尚未結束，就繼續顯示對話
        Story story = currentUserRuntime.story;
        if (story.canContinue)
        {
            string text = story.Continue().Trim();

            bool isPlayer = false;
            string speaker = "";

            // 處理 Ink 內的 tags
            foreach (var tag in story.currentTags)
            {
                var parts = tag.Split(':'); // tag 格式為「key: value」
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim().ToLower();
                    string value = parts[1].Trim().ToLower();

                    if (key == "speaker" && value == "player") isPlayer = true;
                    if (key == "speaker") speaker = value;
                }
            }

            // 顯示並儲存當前對話
            chatBubbleManager.AddBubble(text, isPlayer);
            currentUserRuntime.chatHistory.Add((text, isPlayer));

            // 自動播放：再次呼叫 ContinueStory() 直到遇到選項或故事結束
            // if (!waitingForChoice) Invoke(nameof(ContinueStory), 0.1f);
        }
        // 若遇到選項則啟用按鈕
        else if (story.currentChoices.Count > 0)
        {
            waitingForChoice = true;

            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                var choice = story.currentChoices[i];
                var button = respondButtons[i];

                button.interactable = true;
                button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text.Trim();

                button.onClick.RemoveAllListeners();
                int choiceIndex = i;
                button.onClick.AddListener(() => ChooseOption(choiceIndex));
            }
        }
        // 故事完全結束
        // else
        // {
        //     chatBubbleManager.AddBubble("END", false);
        //     currentUserRuntime.chatHistory.Add(("END", false));
        // }
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
        ContinueStory();
    }
}
