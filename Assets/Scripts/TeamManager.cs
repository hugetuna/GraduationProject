using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
[DefaultExecutionOrder(-100)]
public class TeamManager : MonoBehaviour
{
    public List<PlayerControlMainWorld> teamMembers = new List<PlayerControlMainWorld>(); // 隊員列表
    public List<PlayerControlMainWorld> busyMembers = new List<PlayerControlMainWorld>(); // 忙碌中的隊員列表
    public List<GameObject> allIdols = new List<GameObject>();//全角色列表
    public int currentLeaderIndex = 0; // 當前操縱角色索引
    public float followDistance = 3f; // 角色之間的距離
    public float followSpeed = 5f; // 角色跟隨速度
    private bool isSwitchingLeader = false; // SwitchLeader執行時為真
    [SerializeField]
    private Vector3 startPosition; // 起始位置
    private void Start()
    {
        BuildUpIdolsTeam();
        //封鎖隊長外的PlayerControlMainWorld
        //for (int i = 0; i < teamMembers.Count; i++)
        //{
        //    bool isLeader = (i == currentLeaderIndex);
        //    teamMembers[i].GetComponent<PlayerControlMainWorld>().enabled = isLeader;
        //}
    }
    void Update()
    {
        HandleFollowers();
        UpdateSortingOrder();
    }
    public void BuildUpIdolsTeam()
    {
        var idolDataList = GameManager.Instance.idolDataList;
        for (int i = 0; i < idolDataList.Count; i++)
        {
            var data = idolDataList[i];
            var idol = Instantiate(allIdols[(int)data.idolIndex], startPosition, Quaternion.identity);
            var idolAbility = idol.GetComponent<IdolInstance>();
            var isActive = data.isAvailable;
            //手動把資料填回去
            idolAbility.LoadData(data);
            //idolAbility.positionInTeam = i;
            if (isActive) //判斷是否在隊伍中
            {
                teamMembers.Add(idol.GetComponent<PlayerControlMainWorld>());
            }
            else
            {
                busyMembers.Add(idol.GetComponent<PlayerControlMainWorld>());
            }
            idol.SetActive(isActive);
        }

        //初始化位置編號
        SetUpTeamPosition();
    }
    public void ResetIdolsTeam() // 每日重置用
    {
        for (int i = busyMembers.Count - 1; i >= 0; i--)
        {
            RemoveBusyMember(busyMembers[i]);
        }

        foreach (var member in teamMembers)
        {
            // 因為寫在 DayManager 的 EndDay 似乎沒用，於是直接寫在這
            IdolInstance idol = member.GetComponent<IdolInstance>();
            idol.gameObject.SetActive(idol.isAvailable);
        }
    }

    public void AddBusyMember(PlayerControlMainWorld member)
    {
        if (teamMembers.Contains(member) && !busyMembers.Contains(member))
        {
            // //移動到忙碌列表
            // busyMembers.Add(member);
            // teamMembers.Remove(member);
            // //如果是隊長，切換隊長並重洗位置編號
            // if (teamMembers.Count > 0 && currentLeaderIndex >= teamMembers.Count)
            // {
            //     currentLeaderIndex = 0;
            //     SetUpTeamPosition();
            // }

            // 判斷是否是隊長
            bool wasLeader = (member == teamMembers[currentLeaderIndex]);

            // 移動到忙碌列表
            busyMembers.Add(member);
            teamMembers.Remove(member);
            // 如果移除的是隊長，就要換新隊長
            if (wasLeader && teamMembers.Count > 0|| teamMembers.Count > 0 && currentLeaderIndex >= teamMembers.Count)
            {
                currentLeaderIndex = 0;
                //封鎖隊長外的PlayerControlMainWorld
                for (int i = 0; i < teamMembers.Count; i++)
                {
                    bool isLeader = (i == currentLeaderIndex);
                    teamMembers[i].GetComponent<PlayerControlMainWorld>().enabled = isLeader;
                }
                SetUpTeamPosition();
            }
        }
    }
    public void RemoveBusyMember(PlayerControlMainWorld member)
    {
        if (busyMembers.Contains(member) && !teamMembers.Contains(member))
        {
            busyMembers.Remove(member);
            teamMembers.Add(member);
            currentLeaderIndex = 0;
            //封鎖隊長外的PlayerControlMainWorld
            for (int i = 0; i < teamMembers.Count; i++)
            {
                bool isLeader = (i == currentLeaderIndex);
                teamMembers[i].GetComponent<PlayerControlMainWorld>().enabled = isLeader;
            }
            //重洗位置編號
            SetUpTeamPosition();
        }
    }
    public void SetUpTeamPosition()
    {
        for (int i = 0; i < teamMembers.Count; i++)
        {
            teamMembers[(currentLeaderIndex + i) % teamMembers.Count].GetComponent<IdolInstance>().positionInTeam = i;
        }
        for (int i = 0; i < busyMembers.Count; i++)
        {
            busyMembers[i].GetComponent<IdolInstance>().positionInTeam = i + teamMembers.Count;//忙碌隊員編號接在隊伍後面
        }
    }
    // 角色切換(+1為下一個-1為上一個)
    public void SwitchLeader(int direction)
    {
        // 暫時禁用跟隨邏輯，避免瞬間覆蓋新位置
        StopAllCoroutines();
        isSwitchingLeader = true;

        int previousLeaderIndex = currentLeaderIndex; // 記住原隊長索引
        currentLeaderIndex = (currentLeaderIndex + direction + teamMembers.Count) % teamMembers.Count; // 更新隊長索引

        // 取得原隊長與新隊長的位置
        Vector3 previousLeaderPos = teamMembers[previousLeaderIndex].transform.position;
        Vector3 newLeaderPos = teamMembers[currentLeaderIndex].transform.position;
        //封鎖隊長外的PlayerControlMainWorld
        for (int i = 0; i < teamMembers.Count; i++)
        {
            bool isLeader = (i == currentLeaderIndex);
            teamMembers[i].GetComponent<PlayerControlMainWorld>().enabled = isLeader;
        }
        // 啟動移動協程，讓兩個角色互換位置
        StartCoroutine(SwapPositionSmoothly(teamMembers[previousLeaderIndex], teamMembers[currentLeaderIndex], previousLeaderPos, newLeaderPos));
        //交換站位編號
        SetUpTeamPosition();
        Debug.Log("當前主控角色：" + teamMembers[currentLeaderIndex].gameObject.name + direction);

    }
    // 讓兩個角色平滑走到彼此的位置
    private IEnumerator SwapPositionSmoothly(PlayerControlMainWorld oldLeader, PlayerControlMainWorld newLeader, Vector3 oldPos, Vector3 newPos)
    {
        float duration = 0.7f;
        float elapsedTime = 0f;
        //獲取兩人的動畫
        Animator oldLeaderAnim = oldLeader.GetComponent<Animator>();
        Animator newLeaderAnim = newLeader.GetComponent<Animator>();
        while (elapsedTime < duration)
        {
            // 讓原隊長仍然可以更新動畫
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration; // 0 ~ 1

            // 計算速度，確保動畫觸發
            float speed = Vector3.Distance(oldLeader.transform.position, newPos) / Time.deltaTime;

            if (oldLeaderAnim) oldLeaderAnim.SetFloat("Speed", speed);
            if (newLeaderAnim) newLeaderAnim.SetFloat("Speed", speed);
            //0~1插植換位
            oldLeader.transform.position = Vector3.Lerp(oldPos, newPos, t);
            newLeader.transform.position = Vector3.Lerp(newPos, oldPos, t);

            yield return null;
        }
        // 確保最後停下來
        oldLeader.transform.position = newPos;
        newLeader.transform.position = oldPos;
        //停止動畫
        if (oldLeaderAnim) oldLeaderAnim.SetFloat("Speed", 0);
        if (newLeaderAnim) newLeaderAnim.SetFloat("Speed", 0);
        // 延遲 0.1 秒後，確保新隊長啟用，舊隊長禁用
        yield return new WaitForSeconds(0.1f);

        isSwitchingLeader = false;
        StartCoroutine(ResumeFollowAfterDelay(0.1f));
    }
    // 延遲啟動 HandleFollowers，確保交換生效
    private IEnumerator ResumeFollowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HandleFollowers(); // 重新啟動跟隨
    }
    // 讓非主控角色跟隨
    void HandleFollowers()
    {
        for (int i = 0; i < teamMembers.Count; i++)
        {
            if (i != currentLeaderIndex)
            {
                //計算隊長位置與目標位置
                Vector3 leaderPos = teamMembers[currentLeaderIndex].transform.position;
                Vector3 targetPos;
                //向下輪轉一個隊員
                if ((currentLeaderIndex + 1 + teamMembers.Count) % teamMembers.Count == i)
                {
                    targetPos = leaderPos - (leaderPos - teamMembers[i].transform.position).normalized * Mathf.Min(followDistance, (leaderPos - teamMembers[i].transform.position).magnitude);
                }
                //向下輪轉兩個隊員
                else if ((currentLeaderIndex + 2 + teamMembers.Count) % teamMembers.Count == i)
                {
                    targetPos = leaderPos - (leaderPos - teamMembers[i].transform.position).normalized * Mathf.Min(followDistance * 2, (leaderPos - teamMembers[i].transform.position).magnitude);
                }
                else
                {
                    targetPos = leaderPos;
                }
                float zOffset = 0.1f; // 微小偏移值，避免卡住
                if (teamMembers[i].transform.position.z <= leaderPos.z - zOffset)
                {
                    targetPos.z = leaderPos.z + zOffset; // 稍微往遠處推
                }
                // 計算當前距離，判斷是否移動
                float distance = Vector3.Distance(teamMembers[i].transform.position, targetPos);
                float speed = distance / Time.deltaTime; // 計算當前移動速度

                Animator animator = teamMembers[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetFloat("Speed", speed);
                    if (speed > 10f)
                    {
                        animator.SetFloat("Speed", speed);
                    }
                    else
                    {
                        SyncIdleAnimation();
                    }
                }
                //用目標位置與當前位置決定朝向
                bool moveDirection = (targetPos - teamMembers[i].transform.position).x > 0;//true=向右，false=向左
                // **方向變更緩衝機制**
                float directionThreshold = 0.2f; // 只有當方向變化超過這個閾值時，才會翻轉
                if (moveDirection)
                {
                    if ((targetPos - teamMembers[i].transform.position).x >= directionThreshold)
                    {
                        teamMembers[i].Bone.transform.rotation = Quaternion.Euler(-45, 180, 0); // 朝右
                    }
                }
                else
                {
                    if ((targetPos - teamMembers[i].transform.position).x <= directionThreshold)
                    {
                        teamMembers[i].Bone.transform.rotation = Quaternion.Euler(45, 0, 0); // 朝左
                    }
                }
                //teamMembers[i].transform.position = Vector3.Lerp(teamMembers[i].transform.position, targetPos, followSpeed * Time.deltaTime);
                teamMembers[i].GetComponent<PlayerControlMainWorld>().controller.Move((targetPos - teamMembers[i].transform.position).normalized * followSpeed * Time.deltaTime);
            }
        }
    }
    //同步播放待機動畫(以隊長時間為準)
    void SyncIdleAnimation()
    {
        if (isSwitchingLeader != true)
        {
            Animator leaderAnimator = teamMembers[currentLeaderIndex].GetComponent<Animator>();
            if (leaderAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Idle"))
            {
                float idleTime = leaderAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1; // 取得隊長 Idle 動畫進度

                foreach (var member in teamMembers)
                {
                    Animator memberAnimator = member.GetComponent<Animator>();
                    AnimatorStateInfo stateInfo = memberAnimator.GetCurrentAnimatorStateInfo(0);
                    if (member != teamMembers[currentLeaderIndex] && stateInfo.tagHash == Animator.StringToHash("Idle"))
                    {
                        int IdleAnimationName = stateInfo.shortNameHash;
                        memberAnimator.Play(IdleAnimationName, 0, idleTime); // 讓所有人從隊長的時間點開始播放
                    }
                }
            }
        }
    }
    //更換渲染排序方式
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
    // 切換角色
    public void UpdateInput()
    {
        SwitchLeader(1);
    }
}
