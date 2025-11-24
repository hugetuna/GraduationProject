using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

/* 掛在惡魔頁面 prefab 的根部 */
public class InternalDemonUI : MonoBehaviour
{
    [Header("惡魔頁面內部元素")]
    [SerializeField] private Button talkButton;
    [SerializeField] private Button problemButton;
    [SerializeField] private Button sellButton; // 新增販賣按鈕
    [SerializeField] private Button dialogueObject;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject hintIcon;
    private bool hintShown = true;

    [Header("對話腳本")]
    [SerializeField] private TextAsset inkJSONAsset;
    private Story story;

    private string currentKnot = "default_text"; // 目前正在跑的 Ink knot
    private bool storyFinished = false;

    void Start()
    {
        talkButton.onClick.AddListener(OnTalkButtonClick);
        problemButton.onClick.AddListener(OnProblemButtonClick);
        sellButton.onClick.AddListener(OnSellButtonClick);
        dialogueObject.onClick.AddListener(OnDialogueBgClick);

        ResetDialogue();
    }

    void OnEnable()
    {
        ResetDialogue();
    }

    private void OnTalkButtonClick()
    {
        if (hintShown)
        {
            hintIcon.SetActive(false);
            hintShown = false;
        }

        // 對話未結束不可中斷
        if (currentKnot == "talk" && !storyFinished) return;

        currentKnot = "talk";
        story = new Story(inkJSONAsset.text);
        story.ChoosePathString(currentKnot);
        storyFinished = false;
        ShowNextLine();
    }

    private void OnProblemButtonClick()
    {
        if (currentKnot == "problem" && !storyFinished) return;

        currentKnot = "problem";
        story = new Story(inkJSONAsset.text);
        story.ChoosePathString(currentKnot);
        storyFinished = false;
        ShowNextLine();
    }

    private void OnSellButtonClick()
    {
        if (currentKnot == "sell" && !storyFinished) return;

        currentKnot = "sell";
        story = new Story(inkJSONAsset.text);
        story.ChoosePathString(currentKnot);
        storyFinished = false;
        ShowNextLine();
    }

    private void OnDialogueBgClick()
    {
        ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (story == null) return;

        if (story.canContinue)
        {
            dialogueText.text = story.Continue();
        }
        else
        {
            storyFinished = true;
            dialogueText.text = story.currentText; // 維持最後一句
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
        currentKnot = "default_text";
        story = new Story(inkJSONAsset.text);
        story.ChoosePathString(currentKnot);
        storyFinished = false;
        hintIcon.SetActive(true);
        hintShown = true;
        dialogueText.text = GetInkLine(currentKnot);
    }
}