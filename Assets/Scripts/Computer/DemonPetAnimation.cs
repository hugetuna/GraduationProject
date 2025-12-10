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

            // 播放 Run 或 Fly 動畫
            if (triggerName == "DoRun")
            {
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), 0).normalized;
                // 確保動畫播完才會執行下一段程式碼
                yield return StartCoroutine(Run(dir, runSpeed, runDuration));
            }
            else if (triggerName == "DoFly")
            {
                // 確保動畫播完才會執行下一段程式碼
                yield return StartCoroutine(Fly(flyHeight, flyDuration)); // 飛高單位 + 總時間
            }
            else if (triggerName == "DoCall")
            {
                // 如果是單純播放動畫沒有 Coroutine，可以手動等待動畫長度
                yield return new WaitForSeconds(1.0f);
            }
        }
    }

    private IEnumerator Run(Vector3 direction, float speed, float duration)
    {
        if (isMoving) yield break;
        isMoving = true;

        // 將輸入方向投影到平面並標準化
        Vector3 planeNormal = new(
            0f,
            Mathf.Sin(Mathf.Deg2Rad * rotationAngle),
            -Mathf.Cos(Mathf.Deg2Rad * rotationAngle)
        );
        Vector3 inputDirection = Vector3.ProjectOnPlane(direction, planeNormal).normalized;

        // 初始化翻面判斷變數
        // 判斷當前面向：若 Y 軸旋轉在 90~270 之間視為向左 (-1)，否則向右 (1)
        float currentFacing = (model.localEulerAngles.y > 90 && model.localEulerAngles.y < 270) ? -1f : 1f;
        float flipTimer = 0f;
        float flipThreshold = 0.1f; // 設定 0.1 秒的防抖動時間

        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = startPos + inputDirection * speed * Time.deltaTime;

            // 碰撞檢測邏輯
            bool insideAnyVolume = false;
            foreach (var vol in movementVolumes)
            {
                if (vol.bounds.Contains(targetPos))
                {
                    insideAnyVolume = true;
                    break;
                }
            }

            // 如果撞到邊界 (不在任何 Volume 內)
            if (!insideAnyVolume)
            {
                // 找出最近的邊界點
                Vector3 closestPoint = movementVolumes[0].bounds.ClosestPoint(targetPos);
                foreach (var vol in movementVolumes)
                {
                    Vector3 candidate = vol.bounds.ClosestPoint(targetPos);
                    if ((candidate - targetPos).sqrMagnitude < (closestPoint - targetPos).sqrMagnitude)
                        closestPoint = candidate;
                }

                // 計算修正向量
                Vector3 toBoundary = closestPoint - startPos;

                // 計算碰到邊界後的滑動
                Vector3 slide = Vector3.Project(toBoundary, inputDirection);

                // 更新目標位置
                targetPos = startPos + slide;
            }

            // 統一執行實際移動
            transform.position = targetPos;

            // 依據「實際位移」進行防抖動翻面判斷
            Vector3 actualMovement = targetPos - startPos;

            if (Mathf.Abs(actualMovement.x) > 0.001f)
            {
                float moveDir = Mathf.Sign(actualMovement.x);

                // 如果移動方向與當前面向相反，開始計時
                if (moveDir != currentFacing)
                {
                    flipTimer += Time.deltaTime;

                    // 持續反向移動超過閾值才執行翻面
                    if (flipTimer > flipThreshold)
                    {
                        currentFacing = moveDir;
                        model.localRotation = Quaternion.Euler(0, currentFacing > 0 ? 0 : 180, 0);
                        flipTimer = 0f;
                    }
                }
                else
                {
                    // 方向一致，重置計時器
                    flipTimer = 0f;
                }
            }
            else
            {
                // 幾乎沒有移動，重置計時器
                flipTimer = 0f;
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

