using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class TachieSlot
{
    public string slotName;
    public Image image;
    //記位置
    public Transform slotTransform;
    public Vector3 originalPos; // 原始位置
    public Vector3 nowPos; // 當前位置
    //當前正執行的Coroutine
    public Coroutine runningAnimation;
    public string runningType;
}
public class TachieManager : MonoBehaviour
{
    public List<TachieSlot> tachieSlots;
    public DialogueManager dialogueManager;
    void Start()
    {
        foreach (var slot in tachieSlots)
        {
            if (slot.slotTransform != null)
                slot.originalPos = slot.slotTransform.localPosition;
        }
    }
    //傳入標籤組，一次處理標籤行為
    public void ApplyTachieTags(List<string> tags)
    {
        string characterTag=null;
        string emotionTag = null;
        string behaviorTag = null;
        string moveTag = null;
        foreach (TachieSlot slot in tachieSlots)
        {
            characterTag=GetTag(tags, slot.slotName + "_Character:");
            emotionTag = GetTag(tags, slot.slotName + "_Emotion:");
            behaviorTag = GetTag(tags, slot.slotName + "_Behavior:");
            moveTag = GetTag(tags, slot.slotName + "_Move:");
            //用角色與情緒tag改變立繪
            if (!string.IsNullOrEmpty(characterTag))
            {
                CharacterDialogueProfile profile = dialogueManager.characterDialogueProfiles.Find(p => p.characterTag == characterTag);
                ChangeSlotImg(slot.slotName, profile.defaultPortrait);
                if (!string.IsNullOrEmpty(emotionTag))
                {
                    if (emotionTag == "Empty") { slot.image.gameObject.SetActive(false); }
                    else
                    {
                        slot.image.gameObject.SetActive(true);
                        EmotionSprite emotionSprite = profile.emotions.Find(p => p.emotion == emotionTag);
                        ChangeSlotImg(slot.slotName, emotionSprite.portrait);
                    }
                }
            }
            //用動作撥放小動畫
            if (!string.IsNullOrEmpty(behaviorTag))
            {
                if (slot.runningAnimation != null)
                {
                    StopCoroutine(slot.runningAnimation);
                    slot.slotTransform.localPosition = slot.nowPos;
                }
                SlotBehavior(slot.slotName, behaviorTag);
            }
            //用移動量平移
            if (!string.IsNullOrEmpty(moveTag))
            {
                divideMoveTagAndMoveSlot(slot.slotName, moveTag);
            }
        }
        
    }
    string GetTag(List<string> tags, string prefix)
    {
        foreach(string tag in tags)
        {
            if (tag.StartsWith(prefix)) return tag.Substring(prefix.Length);
        }
        return null;
    }
    //改變插槽的圖片
    public void ChangeSlotImg(string targetSlotName,Sprite Picture)
    {
        foreach(TachieSlot slot in tachieSlots)
        {
            if (slot.slotName == targetSlotName)
            {
                slot.image.sprite = Picture;
                return;
            }
        }
        
    }
    //位移
    public void divideMoveTagAndMoveSlot(string targetSlotName, string divideTarget)
    {
        foreach (TachieSlot slot in tachieSlots)
        {
            if (slot.slotName == targetSlotName)
            {
                string paramInfo = divideTarget.Trim('(', ')'); // 去掉外層括號((30,10),1)->30,10),1;
                string[] args = paramInfo.Split("),");// 割成兩邊((30,10),1)->args[0]="30,10" args[1]="1";
                string[] moveVectorString = args[0].Split(',');// 再割成xy->moveVectorString[0]=30 [1]=10;
                float x = float.Parse(moveVectorString[0]);
                float y = float.Parse(moveVectorString[1]);
                float duration = float.Parse(args[1]);
                Debug.Log($"{x},{y},{duration}");
                Vector2 moveVector = new Vector2(x, y);

                slot.nowPos = slot.slotTransform.localPosition + (Vector3)moveVector;//更新位置
                slot.runningType = "Move";//設定撥放種類
                slot.runningAnimation= StartCoroutine(Move(slot.slotTransform, moveVector, duration));//撥放
            }
        }
    }
    IEnumerator Move(Transform targetSlot, Vector2 moveVector, float duration = 0.25f)
    {
        Vector3 start = targetSlot.localPosition;
        Vector3 target = start + (Vector3)moveVector;
        float t = 0;
        while (t < 1)
        {
            float progress = t / duration;
            targetSlot.localPosition = Vector3.Lerp(start, target, progress);
            t += Time.deltaTime;
            yield return null;
        }
        targetSlot.localPosition = target;
    }
    //些微的動畫效果(跳躍、抖動等)
    public void SlotBehavior(string targetSlotName, string BehaviorType)
    {
        foreach (TachieSlot slot in tachieSlots)
        {
            if (slot.slotName == targetSlotName)
            {
                if (BehaviorType == "Jump")//跳一次
                {
                    slot.runningType = "Jump";
                    StartCoroutine(Jump(slot.image));
                }
                else if (BehaviorType == "Shake")//搖兩下
                {
                    slot.runningType = "Shake";
                    StartCoroutine(Shake(slot.image));
                }
            }
        }
    }
    IEnumerator Jump(Image img, float height = 30f, float duration = 0.25f)
    {
        if (img == null) yield break;
        RectTransform rt = img.rectTransform;
        if (rt == null) yield break;

        Vector3 start = rt.localPosition;
        Vector3 target = start + Vector3.up * height;

        float t = 0f;
        while (t < 1f)
        {
            float progress = t / 1f; // 0→1
            // 用 Sine 曲線平滑跳上去再下來
            float yOffset = Mathf.Sin(progress * Mathf.PI) * height;
            rt.localPosition = start + Vector3.up * yOffset;

            t += Time.deltaTime / duration;
            yield return null;
        }

        rt.localPosition = start; // 保證回到原點
    }

    IEnumerator Shake(Image img, float strength = 15f, float duration = 0.4f, int vibrato = 2)
    {
        if (img == null) yield break;
        RectTransform rt = img.rectTransform;
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
}
