using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在惡魔桌寵的物件上 */
public class DemonPetAnimation : MonoBehaviour
{
    private Animator animator;
    //-----------------------------------------------------------------//
    [Header("動作間隔秒數")]
    [SerializeField] private float minWait = 3f;
    [SerializeField] private float maxWait = 8f;
    //-----------------------------------------------------------------//
    [Header("移動效果")]
    [SerializeField] private float runSpeed = 1.5f;
    [SerializeField] private float flyHeight = 0.75f;

    [Header("移動時間")]
    [SerializeField] private float runDuration = 1.5f;
    [SerializeField] private float flyDuration = 1.5f;
    //-----------------------------------------------------------------//
    [Header("活動範圍設定")]
    [SerializeField] private Collider[] movementVolumes; // 用來限制移動範圍的 Invisible Volume
    [SerializeField] private float flyMinY = 5.2f;
    private float rotationAngle = 39.857f; // UI 平面傾斜角度
    //-----------------------------------------------------------------//
    private Dictionary<int, string> actionTriggers = new()
    {
        {1, "DoCall"},
        {2, "DoFly"},
        {3, "DoRun"}
    };
    private bool isMoving = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        StartCoroutine(RandomActionCoroutine());
    }

    // 隨機播放動作 Coroutine
    private IEnumerator RandomActionCoroutine()
    {
        while (true)
        {
            // 等待一段隨機時間
            float waitTime = Random.Range(minWait, maxWait);
            Debug.Log($"等待 {waitTime:F1} 秒後觸發動作");
            yield return new WaitForSeconds(waitTime);

            // 根據位置範圍決定可用動作
            List<int> availableActions = new() { 1, 3 }; // 預設 Call + Run
            if (CanFlyHere()) availableActions.Add(2);

            // 隨機選擇一個動作
            int actionIndex = availableActions[Random.Range(0, availableActions.Count)];
            string triggerName = actionTriggers[actionIndex];

            // 觸發對應動作
            Debug.Log($"觸發動畫：{triggerName}");
            animator.SetTrigger(triggerName);

            // 播放 Run 或 Fly 同時進行位移
            if (triggerName == "DoRun")
            {
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), 0).normalized;
                StartCoroutine(Run(dir, runSpeed, runDuration));
            }
            else if (triggerName == "DoFly")
            {
                StartCoroutine(Fly(flyHeight, flyDuration)); // 飛高單位 + 總時間
            }
        }
    }
    private IEnumerator Run(Vector3 direction, float speed, float duration)
    {
        if (isMoving) yield break;
        isMoving = true;

        // 將水平方向投影到傾斜平面上
        Vector3 planeNormal = new(0f, Mathf.Sin(Mathf.Deg2Rad * rotationAngle), -Mathf.Cos(Mathf.Deg2Rad * rotationAngle));
        direction = Vector3.ProjectOnPlane(direction, planeNormal).normalized;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // 計算新位置
            Vector3 pos = transform.position;
            pos += speed * Time.deltaTime * direction;

            // 檢查是否超出任何 Invisible Volume
            bool insideAnyVolume = false;
            foreach (var vol in movementVolumes)
            {
                if (vol.bounds.Contains(pos))
                {
                    insideAnyVolume = true;
                    break;
                }
            }

            if (!insideAnyVolume)
            {
                // 找到距離最近的合法位置（看起來會停留在邊界附近）
                Vector3 closestPoint = movementVolumes[0].bounds.ClosestPoint(pos);
                foreach (var vol in movementVolumes)
                {
                    Vector3 candidate = vol.bounds.ClosestPoint(pos);
                    if ((candidate - pos).sqrMagnitude < (closestPoint - pos).sqrMagnitude)
                        closestPoint = candidate;
                }

                pos = closestPoint;
                transform.position = pos;
                break; // 跳出迴圈，不繼續移動
            }

            // 更新座標
            transform.position = pos;

            // 根據水平移動方向轉向
            if (Mathf.Abs(direction.x) > 0.01f) // 避免微小抖動時亂翻面
            {
                Vector3 scale = transform.localScale;
                scale.x = -Mathf.Sign(direction.x) * Mathf.Abs(scale.x);
                transform.localScale = scale;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }

    private IEnumerator Fly(float height, float duration)
    {
        if (isMoving) yield break;
        isMoving = true;

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        // 計算「沿著斜面往上」的方向
        Vector3 slopeTangent = new Vector3(0f, Mathf.Cos(Mathf.Deg2Rad * rotationAngle), Mathf.Sin(Mathf.Deg2Rad * rotationAngle)).normalized;

        // 隨機翻面
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Sign(Random.Range(-1f, 1f)) * Mathf.Abs(scale.x);
        transform.localScale = scale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float offset = Mathf.Sin(t * Mathf.PI) * height; // 飄升降落曲線

            // 角色沿著斜面往「上方」移動，但不離開平面
            Vector3 newPos = startPos + slopeTangent * offset;

            transform.position = newPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos; // 回到起始位置
        isMoving = false;
    }

    private bool CanFlyHere()
    {
        return transform.position.y > flyMinY; // 大於 flyMinY 才可以飛
    }

    // 讓其他的玩家操作（如點擊）也能觸發動畫
    // public void PlayAction(int actionIndex)
    // {
    //     animator.SetTrigger(actionTriggers[actionIndex]);
    // }
}

