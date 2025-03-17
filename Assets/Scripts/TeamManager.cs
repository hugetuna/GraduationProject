using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<PlayerControlMainWorld> teamMembers = new List<PlayerControlMainWorld>(); // 角色列表
    private int currentLeaderIndex = 0; // 當前操縱角色索引
    public float followDistance = 100f; // 角色之間的距離
    public float followSpeed = 5f; // 角色跟隨速度
    void Update()
    {
        HandleFollowers();
    }
    // 角色切換(+1為下一個-1為上一個)
    public void SwitchLeader(int direction)
    {
        //釋出切換後的結果
        currentLeaderIndex = (currentLeaderIndex + direction + teamMembers.Count) % teamMembers.Count;

        for (int i = 0; i < teamMembers.Count; i++)
        {
            if (i == currentLeaderIndex)
                teamMembers[i].enabled = true; // 啟用主角色控制
            else
                teamMembers[i].enabled = false; // 禁用其他角色控制
        }

        Debug.Log("當前主控角色：" + teamMembers[currentLeaderIndex].gameObject.name);
    }
    // 讓非主控角色跟隨
    void HandleFollowers()
    {
        for (int i = 0; i < teamMembers.Count; i++)
        {
            if (i != currentLeaderIndex)
            {
                Vector3 leaderPos = teamMembers[currentLeaderIndex].transform.position;
                Vector3 targetPos = leaderPos - (leaderPos - teamMembers[i].transform.position).normalized * Mathf.Min(followDistance, (leaderPos - teamMembers[i].transform.position).magnitude);
                // 計算當前距離，判斷是否移動
                float distance = Vector3.Distance(teamMembers[i].transform.position, targetPos);
                bool isMoving = distance > 0.01f; // 設定一個小閾值，防止微小晃動
                // 取得 Animator 並根據是否處於移動設置動畫狀態
                Animator animator = teamMembers[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isWalk", isMoving);
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
    [ContextMenu("switch")]
    // 切換角色
    void UpdateInput()
    {
        SwitchLeader(1);
    }
}

