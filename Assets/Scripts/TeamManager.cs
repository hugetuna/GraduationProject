using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TeamManager : MonoBehaviour
{
    public List<PlayerControlMainWorld> teamMembers = new List<PlayerControlMainWorld>(); // ����C��
    public int currentLeaderIndex = 0; // ��e���a�������
    public float followDistance = 3f; // ���⤧�����Z��
    public float followSpeed = 5f; // ������H�t��
    private bool isSwitchingLeader = false; // SwitchLeader����ɬ��u
    void Update()
    {
        HandleFollowers();
        UpdateSortingOrder();
    }
    // �������(+1���U�@��-1���W�@��)
    public void SwitchLeader(int direction)
    {
        // �ȮɸT�θ��H�޿�A�קK�����л\�s��m
        StopAllCoroutines();
        isSwitchingLeader = true;

        int previousLeaderIndex = currentLeaderIndex; // �O��춤������
        currentLeaderIndex = (currentLeaderIndex + direction + teamMembers.Count) % teamMembers.Count; // ��s��������

        // ���o�춤���P�s��������m
        Vector3 previousLeaderPos = teamMembers[previousLeaderIndex].transform.position;
        Vector3 newLeaderPos = teamMembers[currentLeaderIndex].transform.position;
        
        // �Ұʲ��ʨ�{�A����Ө��⤬����m
        StartCoroutine(SwapPositionSmoothly(teamMembers[previousLeaderIndex], teamMembers[currentLeaderIndex], previousLeaderPos, newLeaderPos));

        Debug.Log("��e�D������G" + teamMembers[currentLeaderIndex].gameObject.name);
        
    }
    // ����Ө��⥭�ƨ��쩼������m
    private IEnumerator SwapPositionSmoothly(PlayerControlMainWorld oldLeader, PlayerControlMainWorld newLeader, Vector3 oldPos, Vector3 newPos)
    {
        float duration = 0.7f;
        float elapsedTime = 0f;
        //�����H���ʵe
        Animator oldLeaderAnim = oldLeader.GetComponent<Animator>();
        Animator newLeaderAnim = newLeader.GetComponent<Animator>();

        while (elapsedTime < duration)
        {
            // ���춤�����M�i�H��s�ʵe
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // 0 ~ 1

            // �p��t�סA�T�O�ʵeĲ�o
            float speed = Vector3.Distance(oldLeader.transform.position, newPos) / Time.deltaTime;

            if (oldLeaderAnim) oldLeaderAnim.SetFloat("Speed", speed);
            if (newLeaderAnim) newLeaderAnim.SetFloat("Speed", speed);
            //0~1���Ӵ���
            oldLeader.transform.position = Vector3.Lerp(oldPos, newPos, t);
            newLeader.transform.position = Vector3.Lerp(newPos, oldPos, t);

            yield return null;
        }

        // �T�O�̫ᰱ�U��
        oldLeader.transform.position = newPos;
        newLeader.transform.position = oldPos;
        //����ʵe
        if (oldLeaderAnim) oldLeaderAnim.SetFloat("Speed", 0);
        if (newLeaderAnim) newLeaderAnim.SetFloat("Speed", 0);
        // ���� 0.1 ���A�T�O�s�����ҥΡA�¶����T��
        yield return new WaitForSeconds(0.1f);
        oldLeader.GetComponent<PlayerControlMainWorld>().enabled = false; // �u�T�� PlayerControlMainWorld�A�Ӥ��O��� GameObject
        newLeader.GetComponent<PlayerControlMainWorld>().enabled = true;
        isSwitchingLeader = false;
        StartCoroutine(ResumeFollowAfterDelay(0.1f));
    }
    // ����Ұ� HandleFollowers�A�T�O�洫�ͮ�
    private IEnumerator ResumeFollowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HandleFollowers(); // ���s�Ұʸ��H
    }
    // ���D�D��������H
    void HandleFollowers()
    {
        for (int i = 0; i < teamMembers.Count; i++)
        {
            if (i != currentLeaderIndex)
            {
                //�p�ⶤ����m�P�ؼЦ�m
                Vector3 leaderPos = teamMembers[currentLeaderIndex].transform.position;
                Vector3 targetPos = leaderPos - (leaderPos - teamMembers[i].transform.position).normalized * Mathf.Min(followDistance, (leaderPos - teamMembers[i].transform.position).magnitude);
                // �p���e�Z���A�P�_�O�_����
                float distance = Vector3.Distance(teamMembers[i].transform.position, targetPos);
                float speed = distance / Time.deltaTime; // �p���e���ʳt��

                Animator animator = teamMembers[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetFloat("Speed", speed);
                    if (speed > 20f)
                    {
                        animator.SetFloat("Speed", speed);
                    }
                    else
                    {
                        SyncIdleAnimation();
                    }
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
    //�P�B����ݾ��ʵe(�H�����ɶ�����)
    void SyncIdleAnimation()
    {
        if (isSwitchingLeader != true)
        {
            Animator leaderAnimator = teamMembers[currentLeaderIndex].GetComponent<Animator>();
            float idleTime = leaderAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1; // ���o���� Idle �ʵe�i��

            foreach (var member in teamMembers)
            {
                Animator memberAnimator = member.GetComponent<Animator>();
                AnimatorStateInfo stateInfo = memberAnimator.GetCurrentAnimatorStateInfo(0);
                if (member != teamMembers[currentLeaderIndex] && stateInfo.tagHash == Animator.StringToHash("Idle"))
                {
                    int IdleAnimationName = stateInfo.shortNameHash;
                    memberAnimator.Play(IdleAnimationName, 0, idleTime); // ���Ҧ��H�q�������ɶ��I�}�l����
                }
            }
        }    
    }
    //�󴫴�V�ƧǤ覡
    void UpdateSortingOrder()
    {
        foreach (var member in teamMembers)
        {
            SortingGroup sortingGroup = member.GetComponent<SortingGroup>();
            if (sortingGroup != null)
            {
                sortingGroup.sortingOrder = Mathf.RoundToInt(-member.transform.position.z * 100);
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

