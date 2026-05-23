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
    [SerializeField] private AkumaRealm movementLimit; // 移動範圍限制
    [SerializeField] private float flyMinY = 5.2f;
    [SerializeField] private float rotationAngle = 39.857f; // UI 平面傾斜角度
    [SerializeField] private bool isRotationLocked = true; // 是否強制鎖定斜面角度
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
        model = transform.Find("Drawables");

        // 初始化時確保父物件旋轉正確
        transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
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
            // Debug.Log($"觸發動畫：{triggerName}");
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

    private void LateUpdate()
    {
        if (isRotationLocked)
        {
            // 強制鎖定父物件的旋轉角度，避免產生莫名其妙的偏移
            transform.rotation = Quaternion.Euler(rotationAngle, 0, 0);
        }
    }

    private IEnumerator Run(Vector3 direction, float speed, float duration)
    {
        if (isMoving || movementLimit == null) yield break;
        isMoving = true;

        // 利用 transform 軸向確保位移絕對在斜面上
        Vector3 moveVector = (direction.x * transform.right + direction.y * transform.up).normalized;

        // 翻面方向＆計時器
        float elapsed = 0f;
        float currentFacing = (model.localEulerAngles.y > 90 && model.localEulerAngles.y < 270) ? -1f : 1f;
        float flipTimer = 0f;
        
        while (elapsed < duration)
        {
            Vector3 nextPos = transform.position + moveVector * runSpeed * Time.deltaTime;

            // 將桌寵投射到平面範圍內，取得最終座標
            Vector3 clampedPos = movementLimit.ClampToMoveArea(nextPos);

            // 如果位置沒變，代表撞牆了（因為座標轉換不會超出範圍）
            if (clampedPos == transform.position && elapsed > 0.1f) break;
            transform.position = clampedPos;

            // 防抖動翻面邏輯
            float moveX = (clampedPos - transform.position).x;
            if (Mathf.Abs(moveX) > 0.001f)
            {
                float sign = Mathf.Sign(moveX);
                if (sign != currentFacing)
                {
                    flipTimer += Time.deltaTime;
                    if (flipTimer > 0.1f)
                    {
                        currentFacing = sign;
                        model.localRotation = Quaternion.Euler(0, currentFacing > 0 ? 0 : 180, 0);
                        flipTimer = 0f;
                    }
                }
                else flipTimer = 0f;
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

        // 隨機翻面
        float faceDir = Random.value < 0.5f ? -1f : 1f;
        model.localRotation = Quaternion.Euler(0, faceDir > 0 ? 0 : 180, 0);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float offset = Mathf.Sin(t * Mathf.PI) * height; // 飄升降落曲線

            // 角色沿著斜面往「上方」移動，但不離開平面
            Vector3 newPos = startPos + transform.up * offset;
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

