using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkJSONAsset;
    //對話類型，true為對話場景，false為主場景
    public bool dialogueType = true;
    private Story story;
    [Header("主世界專用調控")]
    public TeamManager teamManager;
    public GameObject MainCanvas;
    [Header("文本與按鈕等UI元件")]
    public GameObject dialogueCanvas;
    public GameObject backGroundCanvas;
    public TextMeshProUGUI dialogueText;
    public Transform dialogueChoices;
    public GameObject ChoiceButtomPrefab;
    public Button skipButton;
    [Header("應應tag改變演示")]
    public List<CharacterDialogueProfile> characterDialogueProfiles;
    public TachieManager tachieManager;
    public TextMeshProUGUI speakerName;
    public Image speakerImage;
    public Sprite EmptyImg;
    public List<BGMFile> bgmFiles;
    public List<sfxFile> sfxFiles;
    public BackGroundSetter backGroundSetter;
    [Header("Log相關")]
    public GameObject LogCanvas;
    public GameObject LogBlock;
    public List<GameObject> LogBlocks;
    public Transform LogContent;
    public ScrollRect scrollRect;
    [Header("打字機效果用")]
    public float typingSpeed = 0.05f;    // 每個字的間隔時間
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    //對話結束時呼叫的函式
    [Header("對話結束時呼叫的場景")]
    public string onDialogueEndScene;
    [Header("為了EventManager")]
    public System.Action onDialogueFinish = null;
    [Header("更改玩家操作")]
    public PlayerInput playerInput;
    //安全切換 Map
    private void SwitchActionMap(string mapName)
    {
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(mapName);
            Debug.Log($"Action Map 切換至: {mapName}");
        }
        // 在切換玩家小人的Action Map 前，檢查是否有任何農場的互動UI正在顯示
        AnimalFarm[] chackIsFarming = FindObjectsByType<AnimalFarm>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);
        foreach (AnimalFarm farm in chackIsFarming)
        {
            if (farm.farmCanvas.gameObject.activeInHierarchy)
            {
                return; // 如果有任何一個農場的互動UI正在顯示，就不切換Action Map
            }
        }
        if (teamManager != null)
        {
            PlayerInput captain = teamManager?.teamMembers[
            teamManager.currentLeaderIndex].GetComponent<PlayerInput>();
            if (captain != null)
            {
                captain.SwitchCurrentActionMap(mapName);
            }
        }
    }
    //單例物件生成
    public static DialogueManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        OnSceneLoaded();
    }
    public void OnSceneLoaded()
    {
        //TrySetVariable<string>("playerName", "郭家豪");
        //Debug.Log("DialogueManager偵測場景載入");
        if (dialogueType == true)
        {
            backGroundCanvas.SetActive(true);
            dialogueCanvas.SetActive(true);
            DialogueStart();
        }
        else {
            dialogueCanvas.SetActive(false);
            backGroundCanvas.SetActive(false);
        }
        MainCanvas= GameObject.Find("Canvas_Main");
        teamManager = FindAnyObjectByType<TeamManager>();
    }
    public void DialogueStart()
    {
        //開啟跳過鍵
        skipButton.interactable = true;
        //切換 Action Map 到對話專用
        SwitchActionMap("Dialogue");
        //關閉玩家操作、ui顯示
        if (dialogueType==false) {
            dialogueCanvas.SetActive(true);
            MainCanvas?.SetActive(false);
            teamManager.teamMembers[
            teamManager.currentLeaderIndex].enabled = false;
        }
        inkJSONAsset = GameManager.Instance.dialogueSaveData.inkJSONAsset;
        onDialogueEndScene = GameManager.Instance.dialogueSaveData.backToSceneName;
        story = new Story(inkJSONAsset.text);
        if(TrySetVariable<int>("teamID", (int)GameManager.Instance.teamIndex)==false)
        {
            Debug.Log("本段劇情沒有teamID變數或不因隊伍而有所差分");
        }
        ContinueStory();
    }
    //對話相關鍵盤輸入------------------------------------------------------
    public void OnAdvance(InputAction.CallbackContext context)
    {
        // 只有在按下且對話框顯示時才觸發
        if (context.performed && dialogueCanvas.activeSelf)
        {
            // 如果目前有選項，不能使用空白鍵，
            // 讓 ContinueStory 處理「有選項時不自動推進」的邏輯
            if (story.currentChoices.Count == 0)
            {
                ContinueStory();
            }
        }
    }
    public void OnCallLog(InputAction.CallbackContext context)
    {
        if (context.performed && dialogueCanvas.activeSelf)
        {
            ToggleLogCanvas();
        }
    }
    //設置愈顯示的劇本
    public void SetStoryJSON(TextAsset newInkJSONAsset)
    {
        inkJSONAsset = newInkJSONAsset;
        story = new Story(newInkJSONAsset.text);
    }
    //推進對話
    public void ContinueStory() {

        if (story.canContinue|| isTyping == true)
        {
            if (isTyping == true)
            {
                StopCoroutine(typingCoroutine);
                isTyping = false;
                string text = story.currentText;
                dialogueText.text = BuildStairText(text.Trim());
            }
            else
            {
                string text = BuildStairText(story.Continue());
                typingCoroutine=StartCoroutine(TypeText(text));
                ApplyTags(story.currentTags);
                AddLogBlock();
            }
        }
        else if (story.currentChoices.Count > 0)
        {
            if (isTyping == true)
            {
                StopCoroutine(typingCoroutine);
                isTyping = false;
                string text = story.currentText;
                dialogueText.text = BuildStairText(text.Trim());
            }
            dialogueChoices.gameObject.SetActive(true);
            ShowChoices();
        }
        else
        {
            OnDialougeEnd();
        }
    }
    //字串加工，使其有縮排(縮排功能目前關掉了)
    private string BuildStairText(string line, int indentStart = 10, int lettersPerLine = 25, int indentStep = 5, int maxLines = 3)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        int currentIndent = indentStart;
        int letterCount = 0;
        int lineCount = 1;

        //sb.Append($"<indent={currentIndent}%>");

        foreach (char letter in line.ToCharArray())
        {
            sb.Append(letter);
            letterCount++;

            //if (letterCount >= lettersPerLine && lineCount < maxLines)
            //{
            //    sb.AppendLine();
            //    lineCount++;
            //    letterCount = 0;

            //    currentIndent = Mathf.Max(0, currentIndent - indentStep);
            //    sb.Append($"<indent={currentIndent}%>");
            //}
        }
        return sb.ToString();
    }
    private IEnumerator TypeText(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        string processed = line;
        int i = 0;
        while (i < processed.Length)
        {
            if (processed[i] == '<') //偵測標籤開頭
            {
                int closeIndex = processed.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    // 一次性加入完整標籤
                    string tag = processed.Substring(i, closeIndex - i+ 1);
                    dialogueText.text += tag;
                    i = closeIndex + 1;
                    continue;
                }
            }
            //普通文字逐字顯示
            dialogueText.text += processed[i];
            i++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
    //改立繪與頭像
    void ApplyTags(List<string> tags)
    {
        //Debug.Log("Applying Tags: " + string.Join(", ", tags));
        string speakerTag = null;
        string emotionTag = null;
        List<string> fontTags = new List<string>();
        string bgmTag = null;
        string sfxTag = null;
        string backgroundTag = null;

        // 先掃一次 tags，存下來
        foreach (string tag in tags)
        {
            if (tag.StartsWith("speaker:"))
                speakerTag = tag.Substring("speaker:".Length);
            else if (tag.StartsWith("emotion:"))
                emotionTag = tag.Substring("emotion:".Length);
            else if (tag.StartsWith("font:"))
                fontTags.Add(tag.Substring("font:".Length));
            else if (tag.StartsWith("bgm:"))
                bgmTag = tag.Substring("bgm:".Length);
            else if (tag.StartsWith("sfx:"))
                sfxTag = tag.Substring("sfx:".Length);
            else if (tag.StartsWith("background:"))
                backgroundTag = tag.Substring("background:".Length);
        }
        if (dialogueType == true)
        {
            //也給立繪掃一次
            tachieManager.ApplyTachieTags(tags);
        }
        //更換字體性質
        if (fontTags.Count!=0)
        {
            foreach (string tag in fontTags)
            {
                if (tag == "Bold")
                {
                    dialogueText.fontStyle = FontStyles.Bold;
                }
                else if (tag == "Italic")
                {
                    dialogueText.fontStyle = FontStyles.Italic;
                }
                else if (tag == "Big")
                {
                    dialogueText.fontSize = 48;
                }
                else if (tag == "Red")
                {
                    dialogueText.color = Color.red;
                }
                if(tag == "Shake")
                {
                    //開始震動協程
                    StartCoroutine(Shake());
                }
                if (tag == "Normal")
                {
                    //恢復預設
                    dialogueText.fontStyle = FontStyles.Normal;
                    dialogueText.fontSize = 36;
                    dialogueText.color = Color.white;
                }
            }
        }
        //更換當前bgm
        if (!string.IsNullOrEmpty(bgmTag)&&dialogueType==true)
        {
            if (bgmTag == "Stop")
            {
                AudioManager.Instance.StopMusic();
                
                return;
            }
            AudioClip audioClip = bgmFiles.Find(bgm => bgm.BGMName == bgmTag)?.audioClip;
            if (audioClip != null)
            {
                AudioManager.Instance.SetMusic(audioClip);
            }
        }
        //播放sfx
        if (!string.IsNullOrEmpty(sfxTag) && dialogueType == true)
        {
            AudioClip audioClip = sfxFiles.Find(sfx =>sfx.sfxName==sfxTag)?.audioClip;
            if (audioClip != null)
            {
                AudioManager.Instance.PlaySFX(audioClip);
            }
        }
        else if(string.IsNullOrEmpty(sfxTag) && dialogueType == true)
        {
             AudioManager.Instance.StopSFX();
        }
        //更換當前背景圖
        if (!string.IsNullOrEmpty(backgroundTag) && dialogueType == true)
        {
            backGroundSetter.SetBackGround(backgroundTag);
        }
        // 如果沒有 speaker，代表這句是旁白，清空 UI
        if (string.IsNullOrEmpty(speakerTag))
        {
            speakerImage.sprite = EmptyImg;
            speakerName.text = "";
            return;
        }
        // 找角色
        CharacterDialogueProfile profile = characterDialogueProfiles.Find(p => p.characterTag == speakerTag);
        if (profile == null) return; // 沒找到就跳過
        // 更新角色名字
        speakerName.text = profile.characterName;
        // 判斷有沒有情緒
        if (!string.IsNullOrEmpty(emotionTag))
        {
            EmotionSprite emotion = profile.emotions.Find(e => e.emotion == emotionTag);
            if (emotion != null)
                speakerImage.sprite = emotion.portrait;
            else
                speakerImage.sprite = profile.defaultPortrait; // 沒找到情緒就用預設
        }
        else
        {
            speakerImage.sprite = profile.defaultPortrait;
        }
    }
    IEnumerator Shake(float strength = 15f, float duration = 0.4f, int vibrato = 2)
    {
        if (dialogueText == null) yield break;
        RectTransform rt = dialogueText.rectTransform;
        if (rt == null) yield break;

        Vector3 start = rt.localPosition;

        float t = 0f;
        while (t < 1f)
        {
            float progress = t / 1f; // 0→1
                                     // 振動 (sin 波 * 衰減)
            float offset = Mathf.Sin(progress * vibrato * Mathf.PI * 2) * strength;
            rt.localPosition = start + Vector3.right * offset;

            t += Time.deltaTime / duration;
            yield return null;
        }
        rt.localPosition = start; // 保證回到原點
    }
    public void ToggleLogCanvas()
    {
        if (LogCanvas.activeSelf)
        {
            LogCanvas.SetActive(false);
        }
        else
        {
            LogCanvas.SetActive(true);
            ScrollToButtom();
        }
    }
    //追加LogBlock
    public void AddLogBlock()
    {
        GameObject logBlockObj = Instantiate(LogBlock, LogContent);
        LogBlockSetting setLogBlock = logBlockObj.GetComponent<LogBlockSetting>();
        setLogBlock.setDialogueContent(story.currentText);
        setLogBlock.setSpeakerName(speakerName.text);
        LogBlocks.Add(logBlockObj);
        //將最新的log設為黃色，其他的設回白色
        foreach (GameObject oldLogBlock in LogBlocks)
        {
            LogBlockSetting oldSetLogBlock = oldLogBlock.GetComponent<LogBlockSetting>();
            oldSetLogBlock.boldLogBlock(false);
        }
        setLogBlock.boldLogBlock(true);
    }
    public void ScrollToButtom()
    {
        //自動捲動到底
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
    //跳轉至特定選項
    public void JumpToKnot(string knotName)
    {
        story.ChoosePathString(knotName);
    }
    //顯示選項
    public void ShowChoices()
    {
        //先刪除所有舊有選項
        foreach(Transform OldSelection in dialogueChoices)
        {
            Destroy(OldSelection.gameObject);
        }
        for(int i=0; i < story.currentChoices.Count; i++)
        {
            // 建立按鈕
            GameObject buttonObj = Instantiate(ChoiceButtomPrefab, dialogueChoices);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            //從story.currentChoices中指派選項內容
            buttonText.text = story.currentChoices[i].text;
            // 綁定事件 (需要保存 i)
            int choiceIndex = i;
            button.onClick.AddListener(() => {
                story.ChooseChoiceIndex(choiceIndex);
                ContinueStory();
                dialogueChoices.gameObject.SetActive(false);
            });
        }
    }
    //取變數與改變數
    public bool TryGetVariable<T>(string varName, out T result) {//檢查型別是否正確，正確就取值
        object value = story.variablesState[varName];
        if (value is T castValue) { result = castValue; return true; }
        result = default;
        return false;
    }
    public bool TrySetVariable<T>(string varName, T setValue)
    {//檢查型別是否正確，正確就設值
        object value = story.variablesState[varName];
        if (value is T) {
            story.variablesState[varName] = setValue;
            Debug.Log($"成功設置變數");
            return true;
        }
        return false;
    }
    public void Skip()
    {
        OnDialougeEnd();
    }
    private void OnDialougeEnd()
    {
        skipButton.interactable = false;//關閉跳過鍵，以免重複點擊
        if (skipButton.interactable == true) return;
        SwitchActionMap("PlayerActionMain"); //切換回玩家操作的 Action Map
        AudioManager.Instance.StopSFX();
        if (dialogueType == true) {
            SceneTransitionManager.Instance.teleportByTargetSceneName(onDialogueEndScene);
        }
        else {
            //如果是主場景對話結束，恢復玩家控制
            dialogueCanvas.SetActive(false);
            MainCanvas?.SetActive(true);
            if (teamManager != null)
            {
                teamManager.teamMembers[
            teamManager.currentLeaderIndex].enabled = true;
            }        
        }
        tachieManager.ResetAllTachieSlot();
        onDialogueFinish?.Invoke();
        onDialogueFinish = null;
    }
}
