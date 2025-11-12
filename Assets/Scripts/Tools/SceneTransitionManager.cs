using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    [Header("轉場動畫")]
    public Animator transitionAnimator; // Animator 應該包含 CoverIn / CoverOut 兩個 Trigger
    public Image imageTransition; // 用於圖片轉場的組件

    [Header("轉場設定")]
    public float minimumShowTime = 1.0f; // 最少顯示 CoverIn 的時間（避免讀取太快）
    private bool isTransitioning = false;
    private void Awake()
    {
        // Singleton 模式，確保跨場景唯一存在
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    /// <summary>
    /// 呼叫這個方法自動儲存資料並處理動畫
    /// <summary>
    public void teleportByTargetSceneName(string targetSceneName)
    {
        // 儲存土壤資料
        Soil[] allSoils = FindObjectsByType<Soil>(FindObjectsSortMode.None);
        if (allSoils.Length != 0)
        {
            GameManager.Instance.SaveSoilData(new List<Soil>(allSoils));
        }
        //儲存偶像資料
        IdolInstance[] allIdolInstances = FindObjectsByType<IdolInstance>(FindObjectsSortMode.None);
        if (allIdolInstances.Length != 0)
        {
            var sortedIdols = allIdolInstances.OrderBy(i => i.positionInTeam).ToList();
            GameManager.Instance.SaveIdolData(sortedIdols);
        }
        //儲存資源
        ResourceManager resourceManager = FindAnyObjectByType<ResourceManager>();
        if (resourceManager != null)
        {
            GameManager.Instance.SaveResourceData(resourceManager);
        }
        // 傳送到指定場景
        AudioManager.Instance.StopMusic();
        LoadSceneWithTransition(targetSceneName);
    }
    /// <summary>
    /// 呼叫這個方法來切換場景，會自動處理動畫
    /// </summary>
    public void LoadSceneWithTransition(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(TransitionRoutine(sceneName));
        
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;
        // 1️.播放覆蓋動畫
        transitionAnimator.SetTrigger("CoverIn");
        float coverInTime = GetAnimationClipLength("CoverIn");
        yield return new WaitForSeconds(coverInTime > 0 ? coverInTime : 0.5f);

        // 2️.開始非同步載入新場景（暫不啟用）
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;

        float timer = 0f;
        while (!async.isDone)
        {
            timer += Time.deltaTime;

            // 當載入完成但尚未啟用場景時
            if (async.progress >= 0.9f && timer >= minimumShowTime)
            {
                // 3️.正式啟用新場景
                async.allowSceneActivation = true;
            }

            yield return null;
        }

        // 4️.等待新場景完全載入（避免畫面閃爍）
        yield return new WaitForSeconds(0.1f);

        // 5️.播放淡出動畫（離開）
        transitionAnimator.SetTrigger("CoverOut");
        float coverOutTime = GetAnimationClipLength("CoverOut");
        yield return new WaitForSeconds(coverOutTime > 0 ? coverOutTime : 0.5f);
        
        isTransitioning = false;
    }

    //取得動畫片段長度
    private float GetAnimationClipLength(string clipName)
    {
        if (transitionAnimator == null || transitionAnimator.runtimeAnimatorController == null)
            return 0f;

        foreach (var clip in transitionAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }

        return 0f;
    }
}
