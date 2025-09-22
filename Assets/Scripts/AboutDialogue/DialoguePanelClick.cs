using UnityEngine;
using UnityEngine.EventSystems;

public class DialoguePanelClick : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// 產生階梯式文字
    /// </summary>
    /// <param name="lines">每一行的文字</param>
    /// <param name="indentStepPercent">每階縮排的百分比 (相對於 Text 寬度)</param>
    /// <returns>帶有 <indent> RichText 的文字</returns>
    public DialogueManager dialogueManager;  // 對話管理器

    public void OnPointerClick(PointerEventData eventData)
    {
        dialogueManager.ContinueStory();
    }
}
