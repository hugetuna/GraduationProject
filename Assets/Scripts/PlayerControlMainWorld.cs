using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlayerControlMainWorld : MonoBehaviour
{
    //更換角色用
    public TeamManager teamManager;
    static private bool switchLock = false;
    private float lockCooldown = 0.2f;  // 防止連按或多次觸發
    //基礎設定
    public int itemOnHandIndex=0;//用數字表示當前持有的道具，0為無道具
    public SpriteResolver toolSpriteResolver; // 工具的 Sprite Resolver 元件
    public Dictionary<int, string> tools = new Dictionary<int,string>(); // 工具對應Sprite Resolver的tag表單
    public Dictionary<string, string> toolAnimationName = new Dictionary<string, string>();// 工具對應動畫的名稱表單
    // 操作與動畫
    private bool actionLock=true;
    public Animator animator;//綁定角色動畫
    private Vector2 moveInput; // 儲存 Move Action 的輸入
    public bool faceDirection = false;//true面向右，false面相左
    public float moveSpeed = 1f;
    public Transform Bone;
    //互動事件參數
    public System.Action onInteractionFinish;
    public string waitInteractionKey;
    //設定初始可操作角色
    void Start()
    {
        teamManager= FindAnyObjectByType<TeamManager>();
        //初始化工具對應的圖樣tag->綁定toolAnimations字典
        tools[0] = "None";
        tools[1] = "Normal";
        // 初始化工具對應的動畫->綁定toolAnimations字典
        toolAnimationName["TGrow"] = "Grow";
        //toolAnimations[1] = "TGrow";
        SetItemOnHandIndex(0);//重制為未持狀態
        //如果自己不是隊長就不啟動
        if (teamManager == null) return;
        if (this != FindAnyObjectByType<TeamManager>().teamMembers[0])
        {
            this.enabled = false; // 只有第一個角色預設可動
        }
        //偵測是否處於等待互動事件狀態
        if (DayManager.Instance.dayEventManager.isWaitingForInteract==true)
        {
            waitInteractionKey = DayManager.Instance.dayEventManager.currentEvent.interactableObjectKey;
            onInteractionFinish =() =>
            {
                DayManager.Instance.dayEventManager.isWaitingForInteract = false;
                DayManager.Instance.dayEventManager.interactObjectKey = "";
                DayManager.Instance.dayEventManager.TriggerNextEvent();
            };
        }
    }
    void Update()
    {
        // 使用 moveInput 控制角色或其他行為
        //移動
        if (moveInput != new Vector2(0, 0))
        {
            if (moveInput.magnitude > 1)
            {
                moveInput.Normalize(); // 確保斜對角長度也是 1，不會超速
            }
            animator.SetFloat("Speed", 60f);

            this.transform.position += new Vector3(moveInput.x, 0, moveInput.y)*Time.deltaTime*moveSpeed;
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
        //決定面向
        // 使用 Y 軸旋轉來翻轉角色
        if (!faceDirection)
        {
            Bone.transform.rotation = Quaternion.Euler(45, 0, 0);  // 朝左
        }
        else
        {
            Bone.transform.rotation = Quaternion.Euler(-45, 180, 0); // 朝右
        }
    }
    public void SetItemOnHandIndex(int targetItem)
    {
        itemOnHandIndex = targetItem;
        UpdateToolVisibility();
    }
    //根據itemOnHandIndex調整工具可見性
    private void UpdateToolVisibility()
    {
        toolSpriteResolver.SetCategoryAndLabel("Tool", tools[itemOnHandIndex]);
        //Debug.Log("switch to" + tools[itemOnHandIndex]);
    }
    //切換隊長
    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (!actionLock) return;

        if (context.performed)
        {
            float value = context.ReadValue<float>();
            if (value != 0)
            {
                if (!switchLock)
                {
                    switchLock = true;

                    teamManager.SwitchLeader((int)value);

                    StartCoroutine(UnlockSwitchAfterDelay());
                }
            }
        }
    }
    private IEnumerator UnlockSwitchAfterDelay()
    {
        yield return new WaitForSeconds(lockCooldown);
        switchLock = false;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (actionLock)
        {
            // 當按鍵被按下或釋放時讀取輸入
            moveInput = context.ReadValue<Vector2>();
            if (context.ReadValue<Vector2>().x > 0)
            {
                faceDirection = true;
            }
            else if (context.ReadValue<Vector2>().x == 0)
            {
            }
            else if (context.ReadValue<Vector2>().x < 0)
            {
                faceDirection = false;
            }
            //Debug.Log("Move Input: " + moveInput);
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        //如果自己不是隊長就不啟動
        if (FindAnyObjectByType<TeamManager>().teamMembers[
            FindAnyObjectByType<TeamManager>().currentLeaderIndex] != this)
        {
            return;
        }
        if (context.performed&&actionLock)
        {
            float interactRadius = 1.8f; // 互動範圍
            float sphereOffset;
            if (!faceDirection) {
                sphereOffset = -1;
            }
            else
            {
                sphereOffset = 1;
            }
            //生成一個互動圈
            Collider[] hits = Physics.OverlapSphere(transform.position+new Vector3(sphereOffset, interactRadius, 0), interactRadius);
            //互動圈中離你最近的物件互動
            foreach (Collider hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact(itemOnHandIndex);
                    Debug.Log("與 " + hit.gameObject.name + "互動");
                    if (interactable.InteractionKey == waitInteractionKey)
                    {
                        var temp = onInteractionFinish;
                        waitInteractionKey = "";
                        onInteractionFinish = null;
                        temp?.Invoke();
                    }
                    return; // 只與最近的物件互動
                }
            }

            Debug.Log("附近沒有可互動的物件");
        }
    }
    //因為現在改成按按鈕觸發動畫，所以新增一個種植專用的動畫觸發函式
    public void OnFarmAnimation()
    {
        animator.SetTrigger("TGrow");
        StartCoroutine(InteractionAnimation(GetAnimationLength(toolAnimationName["TGrow"])));
    }
    //根據名字找到一動畫的長度
    float GetAnimationLength(string animName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        foreach (var clip in ac.animationClips)
        {
            if (clip.name == animName)
            {
                return clip.length; // 秒數
            }
        }
        Debug.LogWarning("找不到動畫：" + animName);
        return 0f;
    }
    IEnumerator InteractionAnimation(float animationTime)
    {
        Debug.Log("動畫開始");
        actionLock = false;//封鎖玩家操作
        //歸零玩家移動
        moveInput = new Vector2(0, 0);
        animator.SetFloat("Speed", 0);
        //監測動畫長度
        yield return new WaitForSeconds(animationTime); // 等動畫跑完
        actionLock = true;
    }
    //可視化互動區
    void OnDrawGizmos()
    {
        float interactRadius = 1.8f; // 互動範圍
        float sphereOffset;
        if (!faceDirection)
        {
            sphereOffset = -1;
        }
        else
        {
            sphereOffset = 1;
        }
        // 設定顏色
        Gizmos.color = Color.green;
        // 畫出一個球，代表互動範圍
        Gizmos.DrawWireSphere(transform.position + new Vector3(sphereOffset, interactRadius, 0), interactRadius);
    }
    public void OnCallConfig(InputAction.CallbackContext context)
    {
        //如果自己不是隊長就不啟動
        if (FindAnyObjectByType<TeamManager>().teamMembers[
            FindAnyObjectByType<TeamManager>().currentLeaderIndex] != this)
        {
            return;
        }
        if (context.performed)
        {
            Debug.Log("開關設定選單");
            if (GameManager.Instance.Canvas_Config!=null)
            {
                GameManager.Instance.Canvas_Config.SetActive(!GameManager.Instance.Canvas_Config.activeSelf);
            }
        }
    }
}
