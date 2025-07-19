using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class IdolOnStage : MonoBehaviour, IDropHandler
{
    [Header("上台的偶像資料")]
    public IdolInstance idolInstance;
    public float actionTimer=0;
    public bool isAcion = false;
    public ActionCard applyingCard=null;
    private OnStageManager stageManager;
    //不同的偶像有不同的視覺呈現，在此以連續圖片列表模擬動畫
    [Header("上台的偶像視覺呈現")]
    //圖片動畫
    public SpriteRenderer spriteRenderer;
    public SpriteAnimator spriteAnimator;
    public List<Sprite> idleFrames;
    public List<Sprite> actionFrames;
    //旋轉部分
    private bool isRotating = false;
    private float rotationTimer = 0f;
    private float rotationDuration = 0.2f; // 旋轉持續時間 (秒)
    private Quaternion startRotation;
    private Quaternion endRotation;
    // Start is called before the first frame update
    void Start()
    {
        stageManager = FindObjectOfType<OnStageManager>();
        spriteAnimator = gameObject.GetComponent<SpriteAnimator>();
        spriteAnimator.SetFrames(idleFrames);
        startRotation = Quaternion.Euler(0, 0, 0);
        endRotation = Quaternion.Euler(0, 180f, 0);
    }
    private void Update()
    {
        if(isAcion==true&& applyingCard != null)
        {
            actionTimer += Time.deltaTime;
            //使用時間到，歸零計時
            if (actionTimer >= applyingCard.applyDuration)
            {
                ApllyOnEndAndReset();
                isRotating = true;
            }
        }
        if (isRotating == true)
        {
            rotationTimer += Time.deltaTime;
            float t = rotationTimer / rotationDuration;

            if (t >= 1f)
            {
                t = 1f;
                isRotating = false;
                rotationTimer = 0;
            }
            // 動作時順轉，結束時逆轉
            if (isAcion == true)
            {
                transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(endRotation, startRotation, t);
            }
            
        }
    }
    //設定卡片到偶像上，若可設定，回傳true，反之回傳否
    public bool SetApplyingCard(ActionCard cardToApply)
    {
        if (cardToApply == null&& applyingCard != null&&isAcion == true)
        {
            Debug.LogError("SetApplyingCard：傳入的 cardToApply 是 null，或已處於動作狀態！");
            return false;
        }
        if (isAcion == false&& applyingCard==null)
        {
            applyingCard = cardToApply;
            //如果有過標準就結算效果
            if (idolInstance.vocal >= applyingCard.voGate && idolInstance.dance >= applyingCard.daGate && idolInstance.visual >= applyingCard.viGate)
            {
                spriteAnimator.SetFrames(actionFrames);
                foreach (var applyEffect in applyingCard.effects)
                {
                    applyEffect.OnApply(this, stageManager);
                }
            }
            actionTimer = 0;
            isAcion = true;
            isRotating = true;
            return true;
        }
        else
        {
            return false;
        }
    }
    //結算卡片
    public void ApllyOnEndAndReset()
    {
        //如果有過標準就結算效果
        if (idolInstance.vocal>=applyingCard.voGate&& idolInstance.dance >= applyingCard.daGate&& idolInstance.visual >= applyingCard.viGate)
        {
            spriteAnimator.SetFrames(idleFrames);
            foreach (var endEffect in applyingCard.effects)
            {
                endEffect.OnEnd(this, stageManager);
            }
        }
        actionTimer = 0;
        isAcion = false;
        applyingCard = null;
    }
    //拖曳落點
    public void OnDrop(PointerEventData eventData)
    {
        // 嘗試從拖曳來源取得 SetCardUI
        SetCardUI draggedCardUI = eventData.pointerDrag?.GetComponent<SetCardUI>();
        if (draggedCardUI != null)
        {
            ActionCard incomingCard = draggedCardUI.cardData;
            bool accepted = SetApplyingCard(incomingCard);
            if (accepted)
            {
                Debug.Log($"{idolInstance.name} 成功接收到卡片 {incomingCard.cardName}！");
                stageManager.hands.Remove(draggedCardUI.gameObject);
                Destroy(draggedCardUI.gameObject); // 卡片被使用後消失
            }
            else
            {
                Debug.Log($"{idolInstance.name} 無法使用該卡片。");
            }
        }
    }

}
