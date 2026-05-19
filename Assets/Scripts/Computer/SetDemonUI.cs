using System.Collections;
using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;
using TMPro;

/* 掛在惡魔頁面 prefab 的根部 */
public class SetDemonUI : MonoBehaviour
{
    [Header("惡魔頁面內部元素")]
    [SerializeField] private Button talkButton;
    [SerializeField] private Button problemButton;
    [SerializeField] private Button sellButton; // 販賣按鈕
    [SerializeField] private GameObject sellUI; // 販賣頁面
    private bool isSellInitialized = false;
    [SerializeField] private Button dialogueObject;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject hintIcon;
    private bool hintShown = true;
    //-----------------------------------------------------------------//
    [Header("對話腳本")]
    [SerializeField] private TextAsset inkJSONAsset;
    private Story story;
    private string currentKnot = "default_text"; // 目前正在跑的 Ink knot
    private bool storyFinished = false;
    //-----------------------------------------------------------------//
    [Header("打字機效果設定")]
    [SerializeField] private float typingSpeed = 0.05f; // 每個字出現的速度
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullLineText = ""; // 暫存目前的完整文字內容
    [SerializeField] private GameObject indicator; // 漂浮三角形
    //-----------------------------------------------------------------//
    [Header("音效設定")]
    [SerializeField] private AudioClip openSellUISound; // 開啟販賣頁面的音效

    void Start()
    {       
        sellUI.SetActive(false); // 預設關閉販賣頁面
        hintIcon.SetActive(true); // 預設顯示提示圖示
        indicator.SetActive(false); // 預設關閉三角形

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
        InitAndStartStory(currentKnot);
    }

    private void OnProblemButtonClick()
    {
        if (currentKnot == "problem" && !storyFinished) return;

        currentKnot = "problem";
        InitAndStartStory(currentKnot);
    }

    private void OnSellButtonClick()
    {
        // 打開販賣頁面
        sellUI.SetActive(true);
        AudioManager.Instance.PlaySFX(openSellUISound, 0.5f);

        if(!isSellInitialized)
        {
            sellUI.GetComponent<SetSellUI>().Initialize();
            isSellInitialized = true;
        }
        
        // 原先的對話
        // if (currentKnot == "sell" && !storyFinished) return;

        // currentKnot = "sell";
        // InitAndStartStory(currentKnot);
    }

    // 將重複的故事初始化整理成函式
    private void InitAndStartStory(string knotName)
    {
        story = new Story(inkJSONAsset.text);
        story.ChoosePathString(knotName);
        storyFinished = false;

        indicator.SetActive(false); // 新對話開始時先收起三角形

        // 切換 Knot 時要確保停止上一個正在跑的打字機
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        ShowNextLine();
    }

    private void OnDialogueBgClick()
    {
        // 如果正在打字，點擊背景則立即輸出完整文字
        if (isTyping)
        {
            FinishTypewriterInstantly();
        }
        else
        {
            ShowNextLine();
        }
    }

    private void ShowNextLine()
    {
        if (story == null) return;

        if (story.canContinue)
        {
            fullLineText = story.Continue();

            indicator.SetActive(false); // 打字期間要把三角形隱藏起來
            
            // 啟動打字機效果
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeTextRoutine(fullLineText));
        }
        else
        {
            storyFinished = true;
            dialogueText.maxVisibleCharacters = fullLineText.Length; // 結束時確保文字完全顯示
            indicator.SetActive(false); // 結束時也要收起三角形
            // dialogueText.text = story.currentText; // 維持最後一句
        }
    }
    private IEnumerator TypeTextRoutine(string text) // 打字機協程
    {
        isTyping = true;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        // 強制更新網格以確保排版正確，避免文字在打字過程中換行跳動
        dialogueText.ForceMeshUpdate();

        int totalVisibleCharacters = text.Length;
        int counter = 0;

        while (counter <= totalVisibleCharacters)
        {
            dialogueText.maxVisibleCharacters = counter;
            counter++;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;

        // 字全部打完後檢查接著是否還有話要說，有的話才顯示三角形提醒玩家
        if (story != null && story.canContinue)
        {
            indicator.SetActive(true);
        }
    }

    private void FinishTypewriterInstantly() // 即時停止打字機效果（並顯示所有文字）
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogueText.maxVisibleCharacters = fullLineText.Length;
        isTyping = false;

        // 玩家手動點擊讓打字全部顯示，此時一樣檢查後面有沒有字，有就顯示三角形
        if (indicator != null && story != null && story.canContinue)
        {
            indicator.SetActive(true);
        }
    }

    // private string GetInkLine(string knotName)
    // {
    //     var tempStory = new Story(inkJSONAsset.text);
    //     tempStory.ChoosePathString(knotName);
    //     return tempStory.Continue();
    // }

    private void ResetDialogue()
    {
        // 重置時也要停止打字機＆三角形
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        isTyping = false;
        indicator.SetActive(false);

        currentKnot = "default_text";
        story = new Story(inkJSONAsset.text);
        story.ChoosePathString(currentKnot);
        storyFinished = false;

        hintIcon.SetActive(true);
        hintShown = true;
        
        // 預設內容也改用打字機顯示
        // fullLineText = GetInkLine(currentKnot);
        // typingCoroutine = StartCoroutine(TypeTextRoutine(fullLineText));
        ShowNextLine();
    }
}