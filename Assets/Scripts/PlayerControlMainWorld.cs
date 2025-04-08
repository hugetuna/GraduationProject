using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlMainWorld : MonoBehaviour
{
    public int itemOnHandIndex=0;//用數字表示當前持有的道具，0為無道具
    public Dictionary<int, GameObject> tools = new Dictionary<int, GameObject>(); // 工具對應表
    public Dictionary<int, string> toolAnimations = new Dictionary<int, string>(); // 工具對應動畫
    // 操作與動畫
    public Animator animator;//綁定角色動畫
    private Vector2 moveInput; // 儲存 Move Action 的輸入
    public bool faceDirection = false;//true面向右，false面相左
    public float moveSpeed = 1f;
    public Transform Bone;
    //設定初始可操作角色
    void Start()
    {
        // 初始化工具->綁定tools字典 (請確保這些物件已經掛在角色骨架上)
        if (Bone != null)
        {
            tools[0] = null;
            tools[1] = Bone.Find("Tool")?.gameObject;
        }
        // 初始化工具對應的動畫->綁定toolAnimations字典
        toolAnimations[0] = null;
        toolAnimations[1] = "THoe";
        SetItemOnHandIndex(0);//重制為未持狀態
        //如果自己不是隊長就不啟動
        if (this != FindObjectOfType<TeamManager>().teamMembers[0])
        {
            this.enabled = false; // 只有第一個角色預設可動
        }
    }
    void Update()
    {
        // 使用 moveInput 控制角色或其他行為
        //移動
        if (moveInput != new Vector2(0, 0))
        {
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
        // 隱藏所有工具
        foreach (var tool in tools.Values)
        {
            if (tool != null) tool.SetActive(false);
        }
        // 顯示當前選擇的工具
        if (tools.ContainsKey(itemOnHandIndex) && tools[itemOnHandIndex] != null)
        {
            tools[itemOnHandIndex].SetActive(true);
        }
    }
    public void OnSwitchItem(InputAction.CallbackContext context)
    {
        // 當按鍵被按下或釋放時讀取輸入
        if (context.performed)
        {
            Debug.Log("換道具");
            int newIndex=itemOnHandIndex == 0 ? 1 : 0;
            SetItemOnHandIndex(newIndex);
        }
    }
    public void OnMove(InputAction.CallbackContext context)
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
        Debug.Log("Move Input: " + moveInput);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
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
                    // **停止移動**
                    moveInput = Vector2.zero;
                    animator.SetFloat("Speed", 0);
                    // **撥放動畫**
                    if (toolAnimations.ContainsKey(itemOnHandIndex))
                    {
                        animator.SetTrigger(toolAnimations[itemOnHandIndex]);
                        interactable.Interact(itemOnHandIndex);
                    }
                    Debug.Log("與 " + hit.gameObject.name + " 互動"+ toolAnimations[itemOnHandIndex]);
                    return; // 只與最近的物件互動
                }
            }

            Debug.Log("附近沒有可互動的物件");
        }
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
}
