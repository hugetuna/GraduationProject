using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlMainWorld : MonoBehaviour
{
    public int itemOnHandIndex=0;//�μƦr��ܷ�e�������D��A0���L�D��
    public Dictionary<int, GameObject> tools = new Dictionary<int, GameObject>(); // �u�������
    public Dictionary<int, string> toolAnimations = new Dictionary<int, string>(); // �u������ʵe
    // �ާ@�P�ʵe
    public Animator animator;//�j�w����ʵe
    private Vector2 moveInput; // �x�s Move Action ����J
    public bool faceDirection = false;//true���V�k�Afalse���ۥ�
    public float moveSpeed = 1f;
    public Transform Bone;
    //�]�w��l�i�ާ@����
    void Start()
    {
        // ��l�Ƥu��->�j�wtools�r�� (�нT�O�o�Ǫ���w�g���b���Ⱙ�[�W)
        if (Bone != null)
        {
            tools[0] = null;
            tools[1] = Bone.Find("Tool")?.gameObject;
        }
        // ��l�Ƥu��������ʵe->�j�wtoolAnimations�r��
        toolAnimations[0] = null;
        toolAnimations[1] = "THoe";
        SetItemOnHandIndex(0);//����������A
        //�p�G�ۤv���O�����N���Ұ�
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
            animator.SetFloat("Speed", 60f);
            this.transform.position += new Vector3(moveInput.x, 0, moveInput.y)*Time.deltaTime*moveSpeed;
        }
        else
        {
            animator.SetFloat("Speed", 0);
        }
        //�M�w���V
        // �ϥ� Y �b�����½�ਤ��
        if (!faceDirection)
        {
            Bone.transform.rotation = Quaternion.Euler(45, 0, 0);  // �¥�
        }
        else
        {
            Bone.transform.rotation = Quaternion.Euler(-45, 180, 0); // �¥k
        }
    }
    public void SetItemOnHandIndex(int targetItem)
    {
        itemOnHandIndex = targetItem;
        UpdateToolVisibility();
    }
    //�ھ�itemOnHandIndex�վ�u��i����
    private void UpdateToolVisibility()
    {
        // ���éҦ��u��
        foreach (var tool in tools.Values)
        {
            if (tool != null) tool.SetActive(false);
        }
        // ��ܷ�e��ܪ��u��
        if (tools.ContainsKey(itemOnHandIndex) && tools[itemOnHandIndex] != null)
        {
            tools[itemOnHandIndex].SetActive(true);
        }
    }
    public void OnSwitchItem(InputAction.CallbackContext context)
    {
        // �����Q���U�������Ū����J
        if (context.performed)
        {
            Debug.Log("���D��");
            int newIndex=itemOnHandIndex == 0 ? 1 : 0;
            SetItemOnHandIndex(newIndex);
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
                    // **�����**
                    moveInput = Vector2.zero;
                    animator.SetFloat("Speed", 0);
                    // **����ʵe**
                    if (toolAnimations.ContainsKey(itemOnHandIndex))
                    {
                        animator.SetTrigger(toolAnimations[itemOnHandIndex]);
                        interactable.Interact(itemOnHandIndex);
                    }
                    Debug.Log("�P " + hit.gameObject.name + " ����"+ toolAnimations[itemOnHandIndex]);
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
