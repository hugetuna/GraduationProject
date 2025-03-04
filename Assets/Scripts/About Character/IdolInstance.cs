using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdolInstance : MonoBehaviour
{
    //角色的基礎數值
    public Idols basicStatus;
    //實際運行的數值
    public int vocal;//歌唱力
    public int dance;//舞蹈力
    public int visual;//表現力
    public int charm;//魅力
    public int performance;//演技
    public int vigour;//體力
    public int vigourMax;//體力最大值
    //粉絲數
    public int fans;
    public int bondWithP;//與玩家的羈絆
    // 操作與動畫
    public Animator animator;//綁定角色動畫
    private Vector2 moveInput; // 儲存 Move Action 的輸入
    public float moveSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        IdolSetUp();
    }
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
        
    }
    //初始化
    private void IdolSetUp()
    {
        if (basicStatus == null)
        {
            Debug.LogError("基本狀態 (basicStatus) 未設定！");
            return;
        }
        vocal = basicStatus.vocal;
        dance = basicStatus.dance;
        visual = basicStatus.visual;
        charm = basicStatus.charm;
        performance = basicStatus.performance;
        vigour = vigourMax = basicStatus.vigour;
        fans = 0;
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        // 當按鍵被按下或釋放時讀取輸入
        moveInput = context.ReadValue<Vector2>();

        Debug.Log("Move Input: " + moveInput);
    }

}
