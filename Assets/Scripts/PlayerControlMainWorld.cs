using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlMainWorld : MonoBehaviour
{
    // 操作與動畫
    public Animator animator;//綁定角色動畫
    private Vector2 moveInput; // 儲存 Move Action 的輸入
    public bool faceDirection = false;//true面向右，false面相左
    public float moveSpeed = 1f;

    //設定初始可操作角色
    void Start()
    {
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
            animator.SetBool("isWalk", true);
            this.transform.position += new Vector3(moveInput.x, 0, moveInput.y)*Time.deltaTime*moveSpeed;
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
        Vector3 scale = transform.localScale;
        //決定面向
        // 使用 Y 軸旋轉來翻轉角色
        if (!faceDirection)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);  // 朝左
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // 朝右
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
            Debug.Log("互動");

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
                    interactable.Interact(0);
                    animator.SetTrigger("THoe");
                    Debug.Log("與 " + hit.gameObject.name + " 互動");
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
