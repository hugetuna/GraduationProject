using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class IdolInstance : MonoBehaviour
{
    //���⪺��¦�ƭ�
    public Idols basicStatus;
    //��ڹB�檺�ƭ�
    public int vocal;//�q�ۤO
    public int dance;//�R�ФO
    public int visual;//��{�O
    public int charm;//�y�O
    public int performance;//�t��
    public int vigour;//��O
    public int vigourMax;//��O�̤j��
    //������
    public int fans;
    public int bondWithP;//�P���a������
    // �ާ@�P�ʵe
    public Animator animator;//�j�w����ʵe
    private Vector2 moveInput; // �x�s Move Action ����J
    public float moveSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        IdolSetUp();
    }
    void Update()
    {
        // �ϥ� moveInput �����Ψ�L�欰
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
    //��l��
    private void IdolSetUp()
    {
        if (basicStatus == null)
        {
            Debug.LogError("�򥻪��A (basicStatus) ���]�w�I");
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
        // �����Q���U�������Ū����J
        moveInput = context.ReadValue<Vector2>();

        Debug.Log("Move Input: " + moveInput);
    }

}
