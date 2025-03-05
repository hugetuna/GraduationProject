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

    void Update()
    {
        // 使用 moveInput 控制角色或其他行為
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);
        if (moveInput != new Vector2(0, 0))
        {
            animator.SetBool("isWalk", true);
        }
        else
        {
            animator.SetBool("isWalk", false);
        }
        Vector3 scale = transform.localScale;
        if (faceDirection)
        {
            scale.x = -1; // 反轉 x 軸
            transform.localScale = scale;
        }
        else
        {
            scale.x = 1;
            transform.localScale = scale;
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

            float interactRadius = 1f; // 設定玩家互動範圍
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

            foreach (Collider2D hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log("與 " + hit.gameObject.name + " 互動");
                    return; // 只與最近的物件互動
                }
            }

            Debug.Log("附近沒有可互動的物件");
        }
    }

}
