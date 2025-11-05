using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainWorldDialogueTrigger : MonoBehaviour,IInteractable
{
    public string InteractionKey => null; // 這個字串用來指定動畫 key
    public DialogueSaveData inkJSONAsset;//指定對話劇本
    public DialogueManager dialogueManager;
    // Start is called before the first frame update
    void Start()
    {
        dialogueManager= FindAnyObjectByType<DialogueManager>();
    }
    public void Interact(int tool)
    {
        GameManager.Instance.SaveInkJSONAssetData(inkJSONAsset);
        dialogueManager.DialogueStart();
    }
}
