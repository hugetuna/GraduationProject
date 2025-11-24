using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

/* 掛在惡魔頁面 prefab 的根部 */
public class InternalDemonUI : MonoBehaviour
{
    [Header("惡魔頁面內部元素")]
    [SerializeField] private Button talkButton; // 對話按鈕
    [SerializeField] private Button problemButton; // 提問按鈕
    [SerializeField] private Button dialogueObject; // 對話框本身（點擊以推進對話）
    [SerializeField] private TextMeshProUGUI dialogueText; // 惡魔頁面的對話框文字
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject hintIcon; // 提示按鈕是否被點擊過
    private bool hintShown = true; // 記錄提示圖示是否顯示中
    private bool firstTalkClicked = false; // 記錄是否第一次點對話按鈕
    //-----------------------------------------------------------------//
    // [SerializeField] private Button sellButton; // 可開啟販賣頁面的按鈕
    // [SerializeField] private GameObject sellUI; // 販賣頁面
    private Button closeSellButton; // 關閉販賣頁面的按鈕

    // 退出惡魔頁面的按鈕寫在 DemonUIHandler 腳本（讓惡魔頁面的開關能統一管理）
    //-----------------------------------------------------------------//
    [Header("對話腳本")]
    [SerializeField] private TextAsset inkJSONAsset;
    private Story story;

    void Start()
    {
        talkButton.onClick.AddListener(OnTalkButtonClick);
        dialogueObject.onClick.AddListener(OnDialogueBgClick);
        problemButton.onClick.AddListener(OnProblemButtonClick);

        // sellButton.onClick.AddListener(() =>
        // {
        //     sellUI.SetActive(true); // 開啟販賣頁面
        // });

        // closeSellButton = sellUI.transform.Find("Close").GetComponent<Button>();
        // closeSellButton.onClick.AddListener(() =>
        // {
        //     sellUI.SetActive(false); // 關閉販賣頁面
        // });

        dialogueText.text = GetInkLine("talk_0"); // 初始對話框文字
        hintIcon.SetActive(true); // 預設提示圖示為顯示狀態
        // sellUI.SetActive(false); // 預設隱藏販賣頁面
    }

    void OnEnable()
    {
        // 每次開啟惡魔頁面時重置對話狀態
        ResetDialogue();
    }

    private void OnTalkButtonClick()
    {
        // 第一次點對話按鈕時隱藏 hintIcon
        if (hintShown)
        {
            hintIcon.SetActive(false);
            hintShown = false;
        }

        // 第一次點按鈕，從 talk_1 開始
        if (!firstTalkClicked)
        {
            story.ChoosePathString("talk_1");
            firstTalkClicked = true;
            ShowNextLine();
        }
        else
        {
            // 對話結束後循環
            if (!story.canContinue)
            {
                story = new Story(inkJSONAsset.text); // 重置 Ink
                story.ChoosePathString("talk_1");
            }
            ShowNextLine();
        }
    }

    private void OnDialogueBgClick()
    {
        if(dialogueText.text == GetInkLine("talk_0")) return; // 若是待機文字則不處理
        ShowNextLine();
    }

    private void OnProblemButtonClick()
    {
        story.ChoosePathString("problem");
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (story.canContinue)
        {
            dialogueText.text = story.Continue();
        }
        else
        {
            dialogueText.text = "不聊了，快去工作吧"; // 對話結束
        }
    }

    private string GetInkLine(string knotName)
    {
        var tempStory = new Story(inkJSONAsset.text);
        tempStory.ChoosePathString(knotName);
        return tempStory.Continue();
    }

    private void ResetDialogue()
    {
        story = new Story(inkJSONAsset.text);
        firstTalkClicked = false;
        hintIcon.SetActive(true);
        hintShown = true;
        dialogueText.text = GetInkLine("talk_0"); // 待機文字
    }
}
