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
    [Header("移動速度")]
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float flySpeed = 1.5f;

    [Header("移動時間")]
    [SerializeField] private float runDuration = 1.5f;
    [SerializeField] private float flyDuration = 2f;

    private bool isMoving = false;
    //-----------------------------------------------------------------//
    private Dictionary<int, string> actionTriggers = new()
    {
        {1, "DoCall"},
        {2, "DoFly"},
        {3, "DoRun"}
    };

    void Awake()
    {
        animator = GetComponent<Animator>();
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

            // 隨機選擇一個動作（1~3）
            int actionIndex = Random.Range(1, 4);
            string triggerName = actionTriggers[actionIndex];
            Debug.Log($"觸發動畫：{triggerName}");

            // 觸發對應動作
            animator.SetTrigger(triggerName);

            // 播放 Run 或 Fly 時同時進行位移
            if (triggerName == "DoRun")
            {
                Vector3 dir = new Vector3(Random.Range(-1f, 1f), 0, 0).normalized;
                StartCoroutine(Run(dir, runSpeed, runDuration));
            }
            else if (triggerName == "DoFly")
            {
                StartCoroutine(Fly(0.5f, 1f)); // 飛高單位 + 總時間
            }
        }
    }
    private IEnumerator Run(Vector3 direction, float speed, float duration)
    {
        if (isMoving) yield break;
        isMoving = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.Translate(speed * Time.deltaTime * direction);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
    }

    private IEnumerator Fly(float height = 2f, float duration = 2f)
    {
        if (isMoving) yield break;
        isMoving = true;

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration; // 0 -> 1
            float yOffset = Mathf.Sin(t * Mathf.PI) * height; // 正弦曲線飄升降落
            transform.position = startPos + new Vector3(0, yOffset, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 確保回到原位
        transform.position = startPos;
        isMoving = false;
    }


    // 讓其他的玩家操作（如點擊）也能觸發動畫
    // public void PlayAction(int actionIndex)
    // {
    //     animator.SetTrigger(actionTriggers[actionIndex]);
    // }
}

