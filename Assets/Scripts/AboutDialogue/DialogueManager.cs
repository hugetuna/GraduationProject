using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Ink.Runtime;

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
    public TextMeshProUGUI dialogueText;
    public Transform dialogueChoices;
    public GameObject ChoiceButtomPrefab;
    [Header("應應tag改變演示")]
    public List<CharacterDialogueProfile> characterDialogueProfiles;
    public TachieManager tachieManager;
    public TextMeshProUGUI speakerName;
    public Image speakerImage;
    public Sprite EmptyImg;
    public List<BGMFile> bgmFiles;
    public BackGroundSetter backGroundSetter;
    [Header("Log相關")]
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
    public SceneTransferTrigger sceneTransferTrigger;
    public string onDialogueEndScene;
    void Start()
    {
        //TrySetVariable<string>("playerName", "郭家豪");
        if (dialogueType==true)
        {
            DialogueStart();
            teamManager= FindAnyObjectByType<TeamManager>();
        }
    }
    public void DialogueStart()
    {
        //關閉玩家操作、ui顯示
        if (dialogueType==false) {
            dialogueCanvas.SetActive(true);
            MainCanvas.SetActive(false);
            FindAnyObjectByType<TeamManager>().teamMembers[
            FindAnyObjectByType<TeamManager>().currentLeaderIndex].enabled = false;
        }
        inkJSONAsset = GameManager.Instance.dialogueSaveData.inkJSONAsset;
        onDialogueEndScene = GameManager.Instance.dialogueSaveData.backToSceneName;
        story = new Story(inkJSONAsset.text);
        string text = BuildStairText(story.Continue());
        typingCoroutine = StartCoroutine(TypeText(text));
        ApplyTags(story.currentTags);
    }
    //設置愈顯示的劇本
    public void SetStoryJSON(TextAsset newInkJSONAsset)
    {
        inkJSONAsset = newInkJSONAsset;
        story = new Story(newInkJSONAsset.text);
    }
    //推進對話
    public void ContinueStory() {

        if (story.canContinue)
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
                AddLogBlock();
                ApplyTags(story.currentTags);
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
    //字串加工，使其有縮排
    private string BuildStairText(string line, int indentStart = 10, int lettersPerLine = 25, int indentStep = 5, int maxLines = 3)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        int currentIndent = indentStart;
        int letterCount = 0;
        int lineCount = 1;

        sb.Append($"<indent={currentIndent}%>");

        foreach (char letter in line.ToCharArray())
        {
            sb.Append(letter);
            letterCount++;

            if (letterCount >= lettersPerLine && lineCount < maxLines)
            {
                sb.AppendLine();
                lineCount++;
                letterCount = 0;

                currentIndent = Mathf.Max(0, currentIndent - indentStep);
                sb.Append($"<indent={currentIndent}%>");
            }
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
        string speakerTag = null;
        string emotionTag = null;
        string bgmTag = null;
        string backgroundTag = null;

        // 先掃一次 tags，存下來
        foreach (string tag in tags)
        {
            if (tag.StartsWith("speaker:"))
                speakerTag = tag.Substring("speaker:".Length);
            else if (tag.StartsWith("emotion:"))
                emotionTag = tag.Substring("emotion:".Length);
            else if (tag.StartsWith("bgm:"))
                bgmTag = tag.Substring("bgm:".Length);
            else if (tag.StartsWith("background:"))
                backgroundTag = tag.Substring("background:".Length);
        }
        if (dialogueType == true)
        {
            //也給立繪掃一次
            tachieManager.ApplyTachieTags(tags);
        }
        //更換當前bgm
        if (!string.IsNullOrEmpty(bgmTag)&&dialogueType==true)
        {
            AudioClip audioClip = bgmFiles.Find(bgm => bgm.BGMName == bgmTag)?.audioClip;
            if (audioClip != null)
            {
                 AudioManager.Instance.SetMusic(audioClip);
            }
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
    private void OnDialougeEnd()
    {
        if (dialogueType == true) { sceneTransferTrigger.teleportByTargetSceneName(onDialogueEndScene); }
        else {
            dialogueCanvas.SetActive(false);
            MainCanvas.SetActive(true);
            FindAnyObjectByType<TeamManager>().teamMembers[
            FindAnyObjectByType<TeamManager>().currentLeaderIndex].enabled = true;
        }
    }
}
