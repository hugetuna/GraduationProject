using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class IdolOnStage : MonoBehaviour, IDropHandler
{
    [Header("上台的偶像資料")]
    public SpriteRenderer spriteRenderer;
    public IdolInstance idolInstance;
    public float actionTimer=0;
    public bool isAcion = false;
    public ActionCard applyingCard=null;
    private OnStageManager stageManager;
    // Start is called before the first frame update
    void Start()
    {
        stageManager = FindObjectOfType<OnStageManager>();
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
                foreach (var applyEffect in applyingCard.effects)
                {
                    applyEffect.OnApply(this, stageManager);
                }
            }
            actionTimer = 0;
            isAcion = true;
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
        if(idolInstance.vocal>=applyingCard.voGate&& idolInstance.dance >= applyingCard.daGate&& idolInstance.visual >= applyingCard.viGate)
        {
            foreach(var endEffect in applyingCard.effects)
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
