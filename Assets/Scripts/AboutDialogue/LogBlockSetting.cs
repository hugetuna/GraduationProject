using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LogBlockSetting : MonoBehaviour
{
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;
    public void setSpeakerName(string name)
    {
        speakerNameText.text = name;
    }
    public void setDialogueContent(string content)
    {
        dialogueContentText.text = content;
    }
    //將對話紀錄區塊設為黃色顯示
    public void boldLogBlock(bool flag)
    {
        if (!flag)
        {
            speakerNameText.color = Color.white;
            dialogueContentText.color = Color.white;
            return;
        }
        speakerNameText.color=Color.yellow;
        dialogueContentText.color = Color.yellow;
    }
}
