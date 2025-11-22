using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 掛在惡魔桌寵的物件上 */
public class DemonPetAnimation : MonoBehaviour
{
    private Animator animator;
    private Transform model; // 惡魔桌寵模型
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
    [SerializeField] private float rotationAngle = 39.857f; // UI 平面傾斜角度
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
        model = gameObject.transform.Find("Drawables");
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
            // Debug.Log($"等待 {waitTime:F1} 秒後觸發動作");
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
        Vector3 planeNormal = new Vector3(
            0f,
            Mathf.Sin(Mathf.Deg2Rad * rotationAngle),
            -Mathf.Cos(Mathf.Deg2Rad * rotationAngle)
        );
        direction = Vector3.ProjectOnPlane(direction, planeNormal).normalized;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 currentPos = transform.position;
            Vector3 nextPos = currentPos + direction * speed * Time.deltaTime;

            // 檢查是否仍在合法區域
            bool insideAnyVolume = false;
            foreach (var vol in movementVolumes)
            {
                if (vol.bounds.Contains(nextPos))
                {
                    insideAnyVolume = true;
                    break;
                }
            }

            if (!insideAnyVolume) // 碰到邊界後沿著邊界移動
            {
                // 找距離邊界最近的點
                Vector3 closestPoint = movementVolumes[0].bounds.ClosestPoint(nextPos);
                foreach (var vol in movementVolumes)
                {
                    Vector3 candidate = vol.bounds.ClosestPoint(nextPos);
                    if ((candidate - nextPos).sqrMagnitude < (closestPoint - nextPos).sqrMagnitude)
                        closestPoint = candidate;
                }

                // 計算靠近邊界的方向向量
                Vector3 toBoundary = closestPoint - currentPos;

                // 將合法方向投影到移動方向（滑動）
                Vector3 slide = Vector3.Project(toBoundary, direction);

                // 得到最自然的貼邊移動位置
                nextPos = currentPos + slide;
                transform.position = nextPos;

                // 這幀不需要再做翻面與其餘處理，跳到下一幀
                elapsed += Time.deltaTime;
                yield return null;
                continue;
            }

            // 更新座標
            transform.position = nextPos;

            // 水平翻面
            if (Mathf.Abs(direction.x) > 0.01f)
            {
                float faceDir = Mathf.Sign(direction.x);
                model.localRotation = Quaternion.Euler(0, faceDir > 0 ? 0 : 180, 0);
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
        float faceDir = Random.value < 0.5f ? -1f : 1f;
        model.localRotation = Quaternion.Euler(0, faceDir > 0 ? 0 : 180, 0);

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

