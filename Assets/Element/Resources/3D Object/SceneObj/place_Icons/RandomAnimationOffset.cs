using UnityEngine;

public class RandomAnimationOffset : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            // 獲取當前 Animator 的第一個狀態資訊
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            
            // 隨機設定動畫的播放進度 (0.0 到 1.0 之間)
            // 這樣每個物件一開始的位置就會在波浪的不同點上
            float randomOffset = Random.Range(0f, 1f);
            
            // 參數分別為：狀態名稱或Hash, 層級, 偏移進度
            anim.Play(state.fullPathHash, -1, randomOffset);
        }
    }
}