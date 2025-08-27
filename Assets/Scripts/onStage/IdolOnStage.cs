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
    public int StageStamina;
    public int StageStaminaMax;
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
    [Header("UI視覺引導")]
    //UI提示計時器
    public TextMeshProUGUI actionTimerText;
    public Image circleClockUI;
    public TextMeshProUGUI StageStaminaText;
    public Image StaminaBarUI;
    // Start is called before the first frame update
    void Start()
    {
        stageManager = FindObjectOfType<OnStageManager>();
        //spriteAnimator = gameObject.GetComponent<SpriteAnimator>();
        //設置動作圖片
        idleFrames = idolInstance.basicStatus.idleFrames;
        actionFrames = idolInstance.basicStatus.actionFrames;
        spriteAnimator.SetFrames(idleFrames);
        //設置旋轉量
        startRotation = Quaternion.Euler(0, 0, 0);
        endRotation = Quaternion.Euler(0, 180f, 0);
        //設置血量
        StageStaminaMax = idolInstance.basicStatus.onStageStamina;
        StageStamina = StageStaminaMax;
        StageStaminaText.text = $"{StageStamina}/{StageStaminaMax}";
    }
    private void Update()
    {
        if(isAcion==true&& applyingCard != null)
        {
            actionTimer += Time.deltaTime;

            //用(總時長-現在時長)來設置計時器文字及填滿ui
            actionTimerText.text = Mathf.RoundToInt(actionTimer).ToString();
            circleClockUI.fillAmount = (float)(actionTimer/ applyingCard.applyDuration);
            //使用時間到，歸零計時及填滿ui
            if (actionTimer >= applyingCard.applyDuration)
            {
                actionTimerText.text = "0";
                circleClockUI.fillAmount = 0;
                ApllyOnEndAndReset();
                //觸發轉回
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
                spriteRenderer.gameObject.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);
            }
            else
            {
                spriteRenderer.gameObject.transform.localRotation = Quaternion.Slerp(endRotation, startRotation, t);
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
            spriteRenderer.flipX = true;//因為要轉身但還是要保持正確方向
            spriteAnimator.SetFrames(actionFrames);//變成動作姿勢
            isRotating = true;//開始旋轉
            actionTimer = 0;
            isAcion = true;
            SetStamina(StageStamina - applyingCard.staminaCost);//扣血
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
            foreach (var endEffect in applyingCard.effects)
            {
                endEffect.OnEnd(this, stageManager);
            }
        }
        spriteRenderer.flipX = false;//轉回去
        spriteAnimator.SetFrames(idleFrames);
        actionTimer = 0;
        isAcion = false;
        applyingCard = null;
    }
    //血量變動及其協程
    // 呼叫這個函數來更新體力
    public void SetStamina(int targetValue)
    {
        //targetValue = Mathf.Clamp01(targetValue); // 限制範圍 0~1
        StopAllCoroutines(); // 避免多個 Coroutine 疊加
        if (targetValue >= StageStaminaMax) targetValue = StageStaminaMax;
        else if (targetValue <= 0) targetValue = 0;
        StartCoroutine(AnimateStamina(targetValue));
    }
    IEnumerator AnimateStamina(int targetValue)
    {
        float startValue = StageStamina;
        float changeDuration = 0.4f;
        float elapsed = 0f;

        while (elapsed < changeDuration)
        {
            elapsed += Time.deltaTime;
            StageStamina = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, elapsed / changeDuration));
            StaminaBarUI.fillAmount = (float)StageStamina/ StageStaminaMax;
            StageStaminaText.text = $"{StageStamina}/{StageStaminaMax}";
            yield return null;
        }

        StageStamina = targetValue; // 確保結束時精準
        StaminaBarUI.fillAmount = (float)StageStamina / StageStaminaMax;
        StageStaminaText.text = $"{StageStamina}/{StageStaminaMax}";
        
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


