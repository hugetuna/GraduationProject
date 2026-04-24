using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class IdolOnStage : MonoBehaviour, IDropHandler,IPointerEnterHandler, IPointerExitHandler
{
    [Header("上台的偶像資料")]
    public IdolInstance idolInstance;
    public int idolPersonalPoint;//偶像個人得分數
    public List<ActionCard> usedCards;
    public float actionTimer=0;
    public bool isAcion = false;//是否正在執行動作
    public ActionCard applyingCard=null;//正在執行的卡片
    public int StageStamina;
    public int StageStaminaMax;
    public int StageVocal;
    public int StageDance;
    public int StageVisual;
    public float StageFansPointMutiplier=1;
    [SerializeField]
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
    public GameObject actionTimerUI;
    public Image circleClockUI;
    public TextMeshProUGUI StageStaminaText;
    public Image StaminaBarUI;
    // Start is called before the first frame update
    void Start()
    {
        //spriteAnimator = gameObject.GetComponent<SpriteAnimator>();
        //設置動作圖片
        idleFrames = idolInstance.basicStatus.idleFrames;
        actionFrames = idolInstance.basicStatus.actionFrames;
        spriteAnimator.SetFrames(idleFrames);
        //設置旋轉量
        startRotation = Quaternion.Euler(0, 0, 0);
        endRotation = Quaternion.Euler(0, 180f, 0);
    }
    public void ApplyAbility()
    {
        //設置血量
        StageStaminaMax = idolInstance.basicStatus.onStageStamina;
        StageStamina = StageStaminaMax;
        StageStaminaText.text = $"{StageStamina}/{StageStaminaMax}";
        //設置屬性
        StageVocal = idolInstance.vocal;
        StageDance = idolInstance.dance;
        StageVisual = idolInstance.visual;
    }
    public void ApplyEquipment()
    {
        stageManager = FindAnyObjectByType<OnStageManager>();
        //適用裝備
        if (idolInstance.equipmentItemNow != null)
        {
            StageVocal+= idolInstance.equipmentItemNow.vocalBonus;
            StageDance+= idolInstance.equipmentItemNow.danceBonus;
            StageVisual+= idolInstance.equipmentItemNow.visualBonus;
            StageStaminaMax+= idolInstance.equipmentItemNow.staminaBonus;
            StageStamina = StageStaminaMax;//裝備後補滿血
            foreach (var singleStack in idolInstance.equipmentItemNow.actionCardsAddByEquipment)
            {
                ActionCard runtimeCard = CardFactory.CreateCardInstance(singleStack);
                stageManager.deck.Add(runtimeCard);
            }
            stageManager.Shuffle();
        }
        else
        {
            Debug.Log($"{idolInstance.name} 沒有裝備，跳過數值套用。");
        }
    }
    private void Update()
    {
        if(isAcion==true&& applyingCard != null)
        {
            actionTimer += Time.deltaTime;

            //用(總時長-現在時長)來設置計時器文字及填滿ui
            circleClockUI.fillAmount = (float)(actionTimer/ applyingCard.applyDuration);
            //使用時間到，歸零計時及填滿ui
            if (actionTimer >= applyingCard.applyDuration)
            {
                circleClockUI.fillAmount = 0;
                actionTimerUI.SetActive(false);
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
            if (StageVocal >= applyingCard.voGate && StageDance >= applyingCard.daGate && StageVisual >= applyingCard.viGate)
            {
                if (idolInstance.fans >= applyingCard.fanGate)
                {
                    foreach (var applyEffect in applyingCard.effects)
                    {
                        applyEffect.OnApply(this, stageManager);
                    }
                }   
            }
            spriteRenderer.flipX = true;//因為要轉身但還是要保持正確方向
            spriteAnimator.SetFrames(actionFrames);//變成動作姿勢
            isRotating = true;//開始旋轉
            actionTimer = 0;
            actionTimerUI.SetActive(true);
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
        if (StageVocal>=applyingCard.voGate&& StageDance >= applyingCard.daGate&& StageVisual >= applyingCard.viGate)
        {
            if (idolInstance.fans >= applyingCard.fanGate)
            {
                foreach (var endEffect in applyingCard.effects)
                {
                    usedCards.Add(applyingCard);
                    endEffect.OnEnd(this, stageManager);
                }
            }   
        }
        spriteRenderer.flipX = false;//轉回去
        spriteAnimator.SetFrames(idleFrames);
        FindAnyObjectByType<MonitorEffect>()?.SetIdolInMonitor(idolInstance.idolIndex);
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
        if (stageManager.gameBreak) return; // 遊戲暫停中不可拖曳
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
                if (draggedCardUI.isCard)
                {
                    if(incomingCard.isBanishCard==false) stageManager.Grave.Add(incomingCard);
                    else stageManager.Banish.Add(incomingCard);
                    Destroy(draggedCardUI.gameObject); // 卡片被使用後消失
                }
            }
            else
            {
                Debug.Log($"{idolInstance.name} 無法使用該卡片。");
            }
        }
    }
    //滑鼠進入與出
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // 2. 嘗試取得該物件上的卡片資訊
            SetCardUI cardUI = eventData.pointerDrag?.GetComponent<SetCardUI>();
            // 3. 如果卡片可使用，使其發光
            if (cardUI != null && cardUI.cardData != null)
            {
                if (StageVocal >= cardUI.cardData.voGate && StageDance >= cardUI.cardData.daGate && StageVisual >= cardUI.cardData.viGate)
                {
                    Debug.Log($"滑鼠進入 {idolInstance.name}，檢查卡片 {cardUI.cardData.cardName} 可用...");
                    cardUI.ShowGlowEffect(true);
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetCardUI cardUI = eventData.pointerDrag?.GetComponent<SetCardUI>();
        cardUI?.ShowGlowEffect(false);
    }
}
