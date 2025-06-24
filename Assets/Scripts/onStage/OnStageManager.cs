using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class OnStageManager : MonoBehaviour
{
    [Header("關卡列表")]
    public List<StageAttribute> allStageData;
    [Header("當前關卡資料")]
    public int stageIDToLoad;
    public StageAttribute currentStageData;
    public AudioSource musicSource;
    public SpriteRenderer backgroundRenderer;
    [Header("有關卡片")]
    public List<ActionCard> deck;
    public List<GameObject> hands;
    public GameObject cardPrefab;//卡片ui預置件
    public Transform handArea; // UI 範圍 (Card 的父物件，例如是個 HorizontalLayoutGroup)
    [Header("偶像 Prefab")]
    public GameObject idolOnStagePrefab;
    [Header("上台位置（建議為3個）")]
    public Transform[] spawnPoints;

    [Header("目前場上的偶像")]
    private List<IdolInstance> onStageIdols = new List<IdolInstance>();

    void Start()
    {
        //LoadIdolsToStage();
        foreach(var stage in allStageData)
        {
            if(stage.stageID== stageIDToLoad)
            {
                currentStageData = stage;
            }
        }
        LoadStage(currentStageData);
        LoadIdolsToStage();
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
            instance.LoadData(idolDataList[i]);

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
        //建立卡組並打亂
        foreach(var singleStack in stageData.actionCardStacks)
        {
            for(int i = 0; i < singleStack.quantity; i++)
            {
                deck.Add(singleStack.actionCard);
            }
        }
        Shuffle();
        // 顯示描述（可以連接到 UI）
        Debug.Log($"載入關卡：{stageData.stageName} - {stageData.description}");
    }
    //洗牌(使用Fisher-Yates Shuffle 算法)
    [ContextMenu("Shuffle")]
    public void Shuffle()
    {
        System.Random rng = new System.Random();//要使用必須先創造一個實例
        for (int n = deck.Count - 1; n > 0; n--)//從牌組最尾端開始取出一張卡
        {
            int randomIndex = rng.Next(n + 1);//從牌組中所有牌隨機取出一張與此卡交換(可為自己)
            ActionCard temp = deck[n];
            deck[n] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
        Debug.Log("牌組已洗牌");
    }
    public void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (deck.Count == 0) break;
            if (hands.Count >= 6) break;

            // 1. 取出最上面的一張卡
            ActionCard drawnCard = deck[0];
            deck.RemoveAt(0);

            // 2. 實例化一個卡片 UI
            GameObject cardGO = Instantiate(cardPrefab, handArea);

            // 3. 設定卡片資料（你需要一個 Script 來顯示卡片內容）
            SetCardUI ui = cardGO.GetComponent<SetCardUI>();
            ui.SetCard(drawnCard);

            // 4. 加進手牌列表
            hands.Add(cardGO);
        }
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

