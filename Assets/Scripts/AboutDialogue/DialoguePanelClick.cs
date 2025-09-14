using UnityEngine;
using UnityEngine.EventSystems;

public class DialoguePanelClick : MonoBehaviour, IPointerClickHandler
{
    public DialogueManager dialogueManager;  // 對話管理器

    public void OnPointerClick(PointerEventData eventData)
    {
        dialogueManager.ContinueStory();
    }
}
