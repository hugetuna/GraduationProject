using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlMainWorld : MonoBehaviour
{
    // �ާ@�P�ʵe
    public Animator animator;//�j�w����ʵe
    private Vector2 moveInput; // �x�s Move Action ����J
    public bool faceDirection = false;//true���V�k�Afalse���ۥ�
    public float moveSpeed = 1f;

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
        Vector3 scale = transform.localScale;
        if (faceDirection)
        {
            scale.x = -1; // ���� x �b
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
        // �����Q���U�������Ū����J
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
            Debug.Log("����");

            float interactRadius = 1f; // �]�w���a���ʽd��
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

            foreach (Collider2D hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact();
                    Debug.Log("�P " + hit.gameObject.name + " ����");
                    return; // �u�P�̪񪺪��󤬰�
                }
            }

            Debug.Log("����S���i���ʪ�����");
        }
    }

}
