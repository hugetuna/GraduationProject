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
    private Story story;
    [Header("文本與按鈕等UI元件")]
    public TextMeshProUGUI dialogueText;
    public Transform dialogueChoices;
    public GameObject ChoiceButtomPrefab;
    //對話結束時呼叫的函式
    [Header("對話結束時呼叫的函式")]
    public UnityEvent onDialogueEnd;
    void Start()
    {
        story = new Story(inkJSONAsset.text);
        TrySetVariable<string>("playerName","郭家豪");
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
            string text = story.Continue();
            dialogueText.text = text.Trim();
        }
        else if (story.currentChoices.Count > 0)
        {
            dialogueChoices.gameObject.SetActive(true);
            ShowChoices();
        }
        else
        {
            dialogueText.text = "(劇情結束)";
        }
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
}
