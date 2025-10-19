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
}
