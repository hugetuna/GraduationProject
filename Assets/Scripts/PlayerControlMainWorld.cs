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

    //�]�w��l�i�ާ@����
    void Start()
    {
        if (this != FindObjectOfType<TeamManager>().teamMembers[0])
        {
            this.enabled = false; // �u���Ĥ@�Ө���w�]�i��
        }
    }
    void Update()
    {
        // �ϥ� moveInput �����Ψ�L�欰
        //����
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
        //�M�w���V
        // �ϥ� Y �b�����½�ਤ��
        if (!faceDirection)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);  // �¥�
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0); // �¥k
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

            float interactRadius = 1.8f; // ���ʽd��
            float sphereOffset;
            if (!faceDirection) {
                sphereOffset = -1;
            }
            else
            {
                sphereOffset = 1;
            }
            //�ͦ��@�Ӥ��ʰ�
            Collider[] hits = Physics.OverlapSphere(transform.position+new Vector3(sphereOffset, interactRadius, 0), interactRadius);
            //���ʰ餤���A�̪񪺪��󤬰�
            foreach (Collider hit in hits)
            {
                IInteractable interactable = hit.GetComponent<IInteractable>();

                if (interactable != null)
                {
                    interactable.Interact(0);
                    animator.SetTrigger("THoe");
                    Debug.Log("�P " + hit.gameObject.name + " ����");
                    return; // �u�P�̪񪺪��󤬰�
                }
            }

            Debug.Log("����S���i���ʪ�����");
        }
    }
    //�i���Ƥ��ʰ�
    void OnDrawGizmos()
    {
        float interactRadius = 1.8f; // ���ʽd��
        float sphereOffset;
        if (!faceDirection)
        {
            sphereOffset = -1;
        }
        else
        {
            sphereOffset = 1;
        }
        // �]�w�C��
        Gizmos.color = Color.green;
        // �e�X�@�Ӳy�A�N���ʽd��
        Gizmos.DrawWireSphere(transform.position + new Vector3(sphereOffset, interactRadius, 0), interactRadius);
    }
}
