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
    //-----------------------------------------------------------------//
    [Tooltip("聊天間隔")]
    [SerializeField] private float firstLineDelay = 1.0f;
    [SerializeField] private float autoPlayInterval = 5.0f;
    private Coroutine continueCoroutine = null; // 記錄 Coroutine 以方便取消

    void Awake()
    {
        if (Instance == null) Instance = this; // 保持單一實例
        else Destroy(gameObject); // 刪除多餘實例
    }

    void Start()
    {
        chatBubbleManager = ChatBubbleManager.Instance;
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
        // 儲存舊用戶狀態
        if (currentUserRuntime != null)
            currentUserRuntime.SaveState();

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

            // 切換用戶時第一句延遲
            if (continueCoroutine != null) StopCoroutine(continueCoroutine);
            continueCoroutine = StartCoroutine(ContinueStoryCoroutine());
        }
        else
        {
            // 直接繼續故事
            if (continueCoroutine != null) StopCoroutine(continueCoroutine);
            continueCoroutine = StartCoroutine(ContinueStoryCoroutine());
        }

        waitingForChoice = false;
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
                string text = story.Continue().Trim();

                bool isPlayer = false;
                string speaker = "";

                foreach (var tag in story.currentTags)
                {
                    var parts = tag.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim().ToLower();
                        string value = parts[1].Trim().ToLower();

                        if (key == "speaker" && value == "player") isPlayer = true;
                        if (key == "speaker") speaker = value;
                    }
                }

                chatBubbleManager.AddBubble(text, isPlayer);
                currentUserRuntime.chatHistory.Add((text, isPlayer));

                // 自動播放下一句
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
                // 故事完全結束
                yield break;
            }
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
