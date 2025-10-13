using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;

/* 暫時掛在聊天室視窗的 Rightside 上 */
public class ChatController : MonoBehaviour
{
    private ChatBubbleManager chatBubbleManager;
    public TextAsset inkJSONAsset; // Ink 編譯出的 json 檔
    public List<Button> respondButtons; // 用來顯示回應選項的按鈕列表
    private Story story; // Ink 的故事實例
    private bool waitingForChoice = false; // 是否正在等待玩家選擇

    void Start()
    {
        chatBubbleManager = ChatBubbleManager.Instance;
        story = new Story(inkJSONAsset.text);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !waitingForChoice)
        {
            ContinueStory(); // 按下 Z 鍵時繼續對話
        }
    }

    void ContinueStory()
    {
        // 清空所有按鈕文字並關閉互動
        foreach (var button in respondButtons)
        {
            button.interactable = false;
            button.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }

        // 若故事尚未結束，就繼續顯示對話
        if (story.canContinue)
        {
            string text = story.Continue().Trim();

            bool isPlayer = false;
            string speaker = "";

            // 處理 tags
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

            chatBubbleManager.AddBubble(text, isPlayer);

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
        else
        {
            chatBubbleManager.AddBubble("END", false);
        }
    }

    public void ChooseOption(int index) // 選完選項的後續處理
    {
        if (!waitingForChoice) return;

        waitingForChoice = false;

        // 禁用所有選項避免重複點擊
        foreach (var button in respondButtons)
        {
            button.interactable = false;
        }

        story.ChooseChoiceIndex(index);
        ContinueStory();
    }
}
