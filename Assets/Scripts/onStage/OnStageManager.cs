using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class OnStageManager : MonoBehaviour
{

    [Header("關卡資料")]
    public StageAttribute currentStageData;
    public AudioSource musicSource;
    public SpriteRenderer backgroundRenderer;
    [Header("偶像 Prefab")]
    public GameObject idolOnStagePrefab;

    [Header("上台位置（建議為3個）")]
    public Transform[] spawnPoints;

    [Header("目前場上的偶像")]
    private List<IdolInstance> onStageIdols = new List<IdolInstance>();

    void Start()
    {
        //LoadIdolsToStage();
    }
    //將儲存的idol save data讀入不同於主世界的game object
    void LoadIdolsToStage()
    {
        var idolDataList = GameManager.Instance.idolDataList;

        for (int i = 0; i < idolDataList.Count && i < spawnPoints.Length; i++)
        {
            GameObject idolObj = Instantiate(idolOnStagePrefab, spawnPoints[i].position, Quaternion.identity);
            IdolInstance instance = idolObj.GetComponent<IdolInstance>();

            if (instance == null)
            {
                Debug.LogError("IdolOnStage Prefab 缺少 IdolInstance 組件！");
                continue;
            }

            // 載入儲存的資料（你要實作）
            //instance.LoadData(idolDataList[i]);

            onStageIdols.Add(instance);
        }
    }
    //根據關卡編號與角色所持道具生成卡組
    //根據關卡編號生成背景、音樂等次要素
    public void LoadStage(StageAttribute stageData)
    {
        currentStageData = stageData;
        // 設定背景圖
        backgroundRenderer.sprite = stageData.backgroundImage;

        // 播放音樂
        musicSource.clip = stageData.backgroundMusic;
        musicSource.Play();

        // 顯示描述（可以連接到 UI）
        Debug.Log($"載入關卡：{stageData.stageName} - {stageData.description}");
    }
    // 結束演出：計算表演得分並更新 GameManager / ResourceManager
    public void EndPerformance()
    {

        // 把更新後的 idol 資料重新塞回 GameManager
        GameManager.Instance.SaveIdolData(onStageIdols);

        // 回原場景（你要設定好原場景名稱）
        SceneManager.LoadScene("MainScene"); // 替換為你的主場景名稱
    }
}

