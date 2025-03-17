using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<PlayerControlMainWorld> teamMembers = new List<PlayerControlMainWorld>(); // ����C��
    private int currentLeaderIndex = 0; // ��e���a�������
    public float followDistance = 100f; // ���⤧�����Z��
    public float followSpeed = 5f; // ������H�t��
    void Update()
    {
        HandleFollowers();
    }
    // �������(+1���U�@��-1���W�@��)
    public void SwitchLeader(int direction)
    {
        //���X�����᪺���G
        currentLeaderIndex = (currentLeaderIndex + direction + teamMembers.Count) % teamMembers.Count;

        for (int i = 0; i < teamMembers.Count; i++)
        {
            if (i == currentLeaderIndex)
                teamMembers[i].enabled = true; // �ҥΥD���ⱱ��
            else
                teamMembers[i].enabled = false; // �T�Ψ�L���ⱱ��
        }

        Debug.Log("��e�D������G" + teamMembers[currentLeaderIndex].gameObject.name);
    }
    // ���D�D��������H
    void HandleFollowers()
    {
        for (int i = 0; i < teamMembers.Count; i++)
        {
            if (i != currentLeaderIndex)
            {
                Vector3 leaderPos = teamMembers[currentLeaderIndex].transform.position;
                Vector3 targetPos = leaderPos - (leaderPos - teamMembers[i].transform.position).normalized * Mathf.Min(followDistance, (leaderPos - teamMembers[i].transform.position).magnitude);
                // �p���e�Z���A�P�_�O�_����
                float distance = Vector3.Distance(teamMembers[i].transform.position, targetPos);
                bool isMoving = distance > 0.01f; // �]�w�@�Ӥp�H�ȡA����L�p�̰�
                // ���o Animator �îھڬO�_�B�󲾰ʳ]�m�ʵe���A
                Animator animator = teamMembers[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isWalk", isMoving);
                }
                //�ΥؼЦ�m�P��e��m�M�w�¦V
                bool moveDirection = (targetPos - teamMembers[i].transform.position).x > 0;//true=�V�k�Afalse=�V��
                if (moveDirection)
                {
                    teamMembers[i].transform.rotation = Quaternion.Euler(0, 180, 0); // �¥k
                }else
                {
                    teamMembers[i].transform.rotation = Quaternion.Euler(0, 0, 0); // �¥k
                }
                teamMembers[i].transform.position = Vector3.Lerp(teamMembers[i].transform.position, targetPos, followSpeed * Time.deltaTime);
                
            }
        }
    }
    [ContextMenu("switch")]
    // ��������
    void UpdateInput()
    {
        SwitchLeader(1);
    }
}

