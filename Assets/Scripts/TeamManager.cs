using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TeamManager : MonoBehaviour
{
    public List<PlayerControlMainWorld> teamMembers = new List<PlayerControlMainWorld>(); // 角色列表
    public int currentLeaderIndex = 0; // 當前操縱角色索引
    public float followDistance = 3f; // 角色之間的距離
    public float followSpeed = 5f; // 角色跟隨速度
    private bool isSwitchingLeader = false; // SwitchLeader執行時為真
    void Update()
    {
        HandleFollowers();
        UpdateSortingOrder();
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
        
        // 啟動移動協程，讓兩個角色互換位置
        StartCoroutine(SwapPositionSmoothly(teamMembers[previousLeaderIndex], teamMembers[currentLeaderIndex], previousLeaderPos, newLeaderPos));

        Debug.Log("當前主控角色：" + teamMembers[currentLeaderIndex].gameObject.name);
        
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
        oldLeader.GetComponent<PlayerControlMainWorld>().enabled = false; // 只禁用 PlayerControlMainWorld，而不是整個 GameObject
        newLeader.GetComponent<PlayerControlMainWorld>().enabled = true;
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
                Vector3 targetPos = leaderPos - (leaderPos - teamMembers[i].transform.position).normalized * Mathf.Min(followDistance, (leaderPos - teamMembers[i].transform.position).magnitude);
                // 計算當前距離，判斷是否移動
                float distance = Vector3.Distance(teamMembers[i].transform.position, targetPos);
                float speed = distance / Time.deltaTime; // 計算當前移動速度

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
                //用目標位置與當前位置決定朝向
                bool moveDirection = (targetPos - teamMembers[i].transform.position).x > 0;//true=向右，false=向左
                if (moveDirection)
                {
                    teamMembers[i].transform.rotation = Quaternion.Euler(0, 180, 0); // 朝右
                }else
                {
                    teamMembers[i].transform.rotation = Quaternion.Euler(0, 0, 0); // 朝右
                }
                teamMembers[i].transform.position = Vector3.Lerp(teamMembers[i].transform.position, targetPos, followSpeed * Time.deltaTime);
            }
        }
    }
    //同步播放待機動畫(以隊長時間為準)
    void SyncIdleAnimation()
    {
        if (isSwitchingLeader != true)
        {
            Animator leaderAnimator = teamMembers[currentLeaderIndex].GetComponent<Animator>();
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
    void UpdateInput()
    {
        SwitchLeader(1);
    }
}

