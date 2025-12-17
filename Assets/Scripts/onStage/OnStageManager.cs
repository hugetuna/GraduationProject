using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class OnStageManager : MonoBehaviour
{
    [Header("關卡列表")]
    public List<StageAttribute> allStageData;
    [Header("當前關卡資料")]
    public int stageIDToLoad;
    public StageAttribute currentStageData;
    public SpriteRenderer backgroundRenderer;
    [Header("關卡開始與結束旗標")]
    public bool gameStarted = false;
    public bool gameBreak = false;//回合間休息
    public bool gamePaused = false;
    public bool gameEnded = false;
    public GameObject gameStartUIPanel;
    public GameObject gameOngoingUIPanel;
    public GameObject gamePauseUIPanel;
    public GameObject gameEndUIPanel;
    public GameObject Monitor;
    //public bool gamePaused = false;
    [Header("計數相關")]
    [SerializeField]
    private int playerPoint=0;//玩家分數
    public int round=0;//回合數
    public float roundTimer = 0;//計時器
    public float drawChance = 0;//抽排次數
    public float drawCharge = 0;//抽排充能條
    public float drawChargeLimit = 40;//抽排充能上限，超過就抽一張
    public float breakTime = 5f;//每回合的休息時間
    [Header("視覺化計數")]
    public Image timerFillImg;
    public TextMeshProUGUI roundText;
    public Transform roundBlockParant;
    public GameObject roundBlockPrefab;
    private GameObject[] roundBlocks;
    public TextMeshProUGUI musicNameText;
    public TextMeshProUGUI playerPointText;
    public List<GameObject> showDrawChanceCard;
    public Image drawChargeGauge;
    [Header("遊戲開始UI")]
    public List<Sprite> showStageIdolPrefabs;
    public List<Image> showStageIdolPic;
    [Header("遊戲結束UI")]
    public TextMeshProUGUI endStageName;
    public TextMeshProUGUI endFansRewardText;
    public TextMeshProUGUI endMoneyRewardText;
    [Header("有關卡片")]
    public List<ActionCard> deck;
    public List<GameObject> hands;
    public GameObject cardPrefab;//卡片ui預置件
    public Transform handArea; // UI 範圍 (Card 的父物件，例如是個 HorizontalLayoutGroup)
    [Header("有關音效")]
    public AudioClip drawCardSFX;
    public AudioClip RoundEndSFX;
    public AudioClip gainPointSFX;
    [Header("偶像 Prefab")]
    public GameObject idolOnStagePrefab;
    [Header("上台位置（建議為3個）")]
    public Transform[] spawnPoints;
    [Header("目前場上的偶像")]
    private List<IdolInstance> onStageIdols = new List<IdolInstance>();

    void Start()
    {
        currentStageData = GameManager.Instance.onStageStage;
        gameStartUIPanel.SetActive(true);
        LodeStartDemonstration();
        gameOngoingUIPanel.SetActive(false);
        gameEndUIPanel.SetActive(false);
        //寫字
        roundText.text = "ROUND " + round.ToString();
        musicNameText.text = "music: "+currentStageData.musicName;
        //生成回合塊
        roundBlocks = new GameObject[currentStageData.roundMax];
        for (int i=0;i< currentStageData.roundMax; i++)
        {
            GameObject newBlock=Instantiate(roundBlockPrefab, roundBlockParant);
            if(i< round)
            {
                Color color;
                if (ColorUtility.TryParseHtmlString("#B8DAFF", out color))
                {
                    newBlock.GetComponent<Image>().color = color;
                }
                else
                {
                    Debug.LogWarning("顏色字串格式錯誤");
                }
            }
            else
            {
                newBlock.GetComponent<Image>().color = Color.black;
            }
            roundBlocks[i]= newBlock;
        }
    }
    private void Update()
    {
        if (!gameStarted || gameEnded||gamePaused) return;
        // 每秒更新 roundTimer
        if (!gameBreak)
        {
            roundTimer += Time.deltaTime;
            timerFillImg.fillAmount = roundTimer / currentStageData.secPerRound;
        }
        // 每秒更新 drawCharge
        if (drawChance < 3)
        {
            drawCharge += Time.deltaTime * 10f; // 比如 1 秒增加 20點充能
        }
        else
        {
            drawCharge += 0;
        }
        drawChargeGauge.fillAmount = (float)drawCharge/drawChargeLimit;
        // 檢查是否達到充能上限，可以增加抽牌次數
        if (drawCharge >= drawChargeLimit)
        {
            if (drawChance < 3)
            {
                drawChance += 1;
                //drawChanceText.text = drawChance.ToString();
                drawCharge -= drawChargeLimit;
                UpdateDrawChanceUI();
                Debug.Log($"充能完成，獲得一次抽牌機會，目前抽牌次數：{drawChance}");
            }
        }

        // 每過一回合秒數（例如 10 秒）自動增加 round
        if (roundTimer >= currentStageData.secPerRound) // 這個值你可調
        {
            round++;
            roundText.text = "ROUND "+round.ToString();
            roundTimer = 0;
            //更新回合塊顏色
            for (int i = 0; i < currentStageData.roundMax; i++)
            {
                if (i < round)
                {
                    Color color;
                    if (ColorUtility.TryParseHtmlString("#B8DAFF", out color))
                    {
                        roundBlocks[i].GetComponent<Image>().color = color;
                    }
                    else
                    {
                        Debug.LogWarning("顏色字串格式錯誤");
                    }
                }
                else
                {
                    roundBlocks[i].GetComponent<Image>().color = Color.black;
                }
            }
            AudioManager.Instance.PlaySFX(RoundEndSFX);
            StartCoroutine(Break());//開始回合間休息
            Debug.Log($"進入第 {round} 回合！");
            if(round> currentStageData.roundMax)
            {
                Debug.Log("達到最大回合數，遊戲結束！");
                GameEnd();
            }
        }
    }
    //在遊戲開始前於UI展示卡組內容
    public void LodeStartDemonstration()
    {
        //展示卡片列表
        foreach (var singleStack in currentStageData.actionCardStacks)
        {
            for(int i=0;i< singleStack.quantity;i++)
            {
                //從卡片組中抓資料(CardFactory會深拷貝)->實例化->設定UI
                ActionCard actionCard = CardFactory.CreateCardInstance(singleStack.actionCard);
                Transform content = gameStartUIPanel.transform.Find("ShowDeckAndEquipment").Find("AcionCardDemonstration").Find("Viewport").Find("Content");
                GameObject cardGO = Instantiate(cardPrefab, content);
                SetCardUI ui = cardGO.GetComponent<SetCardUI>();
                ui.isInteractive = false;
                ui.SetCard(actionCard);
            }
        }
        //展示偶像及裝備列表
        var idolDataList = GameManager.Instance.idolDataList;
        for (int i=0;i< showStageIdolPrefabs.Count && i< showStageIdolPic.Count; i++)
        {
            showStageIdolPic[i].sprite = showStageIdolPrefabs[(int)idolDataList[i].idolIndex];
        }
    }
    public void GameStart()
    {
        gameStarted = true;
        gameStartUIPanel.SetActive(false);
        gameOngoingUIPanel.SetActive(true);
        Monitor.SetActive(true);
        LoadIdolsToStage();
        LoadStage(currentStageData);
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

            // 載入儲存的資料
            instance.LoadData(idolDataList[i]);

            onStageIdols.Add(instance);
        }
    }
    //根據關卡與角色所持道具生成卡組
    //根據關卡生成背景、音樂等次要素
    public void LoadStage(StageAttribute stageData)
    {
        currentStageData = stageData;
        // 設定背景圖
        backgroundRenderer.sprite = stageData.backgroundImage;

        // 播放音樂
        AudioManager.Instance.SetMusic(stageData.backgroundMusic);

        //建立卡組並打亂
        foreach (var singleStack in stageData.actionCardStacks)
        {
            for (int i = 0; i < singleStack.quantity; i++)
            {
                deck.Add(singleStack.actionCard);
            }
        }
        Shuffle();
        // 顯示描述（可以連接到 UI）
        Debug.Log($"載入關卡：{stageData.stageName} - {stageData.description}");
    }
    private IEnumerator Break()
    {
        gameBreak = true;
        Debug.Log("進入回合間休息時間");
        float breakTimer = 0;
        while (breakTimer <= breakTime)
        {
            breakTimer += Time.deltaTime;
            yield return null;
        }
        gameBreak = false;
    }
    public void PauseGame()
    {
        if (gamePaused || !gameStarted || gameEnded) return;
        gamePaused = true;
        Time.timeScale = 0f;        // 暫停所有 Update-based 動畫、計時
        // 停止 UI 互動誤觸（可選）
        EventSystem.current.sendNavigationEvents = false;
    }
    public void ResumeGame()
    {
        if (!gamePaused) return;
        gamePaused = false;
        Time.timeScale = 1f;
        EventSystem.current.sendNavigationEvents = true;
    }
    public void TogglePausePanel()
    {
        if (gamePaused)
        {
            gamePauseUIPanel.SetActive(true);
        }
        else
        {
            gamePauseUIPanel.SetActive(false);
        }
    }
    public void GameEnd()
    {
        gameStarted = false;
        gameEnded = true;
        //設置並顯示結算畫面
        endStageName.text= currentStageData.stageName;
        endFansRewardText.text= $"{currentStageData.baseRewardFans}";
        endMoneyRewardText.text= $"{currentStageData.baseRewardMoney}";
        foreach(var idol in onStageIdols)
        {
            idol.transform.gameObject.SetActive(false);
        }
        gameOngoingUIPanel.SetActive(false);
        gameEndUIPanel.SetActive(true);
        Monitor.SetActive(false);
        // 停止遊戲、顯示結果或記錄分數
    }
    // 結束演出：計算表演得分並更新 GameManager / ResourceManager
    public void EndAndLeave()
    {
        //TODO:用關卡資料動態回歸場景
        if(currentStageData.clearDialogue!=null) GameManager.Instance.SaveInkJSONAssetData(currentStageData.clearDialogue);
        SceneTransitionManager.Instance.teleportByTargetSceneName(currentStageData.nextSceneName);
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
    public bool DrawCards(int count)
    {
        bool drewAny = false;
        for (int i = 0; i < count; i++)
        {
            if (deck.Count == 0) break;
            if (hands.Count >= 5) break;

            // 1. 取出最上面的一張卡並複製
            ActionCard drawnCard = deck[0];
            deck.RemoveAt(0);
            ActionCard runtimeCard = CardFactory.CreateCardInstance(drawnCard);
            if (runtimeCard == null)
            {
                Debug.LogError("複製卡片失敗！");
                continue;
            }
            // 2. 實例化一個卡片 UI
            GameObject cardGO = Instantiate(cardPrefab, handArea);

            // 3. 設定卡片資料（你需要一個 Script 來顯示卡片內容）
            SetCardUI ui = cardGO.GetComponent<SetCardUI>();
            ui.SetCard(runtimeCard);

            // 4. 加進手牌列表
            hands.Add(cardGO);
            drewAny = true;
        }
        return drewAny;
    }
    //依據是否有抽牌權抽卡
    public void CheckDrawChanceAndDraw()
    {
        if (drawChance == 0)
        {
            Debug.Log($"抽牌權為0，無法抽牌");
            return;
        }

        if (DrawCards(1))
        {
            drawChance--;
            if(drawChance<0) drawChance=0;
            AudioManager.Instance.PlaySFX(drawCardSFX);
            UpdateDrawChanceUI();
            Debug.Log($"成功抽牌，剩餘抽牌權{drawChance}");
        }
        else
        {
            Debug.Log("抽牌失敗（可能牌組已空或手牌滿了）");
        }
    }
    //改變抽牌權視覺化
    public void UpdateDrawChanceUI()
    {
        for (int i = 0; i < showDrawChanceCard.Count; i++)
        {
            if (i < drawChance)
            {
                showDrawChanceCard[i].SetActive(true);
            }
            else
            {
                showDrawChanceCard[i].SetActive(false);
            }
        }
    }
    //-----------------------------------計數----------------------------------------
    //得到分數
    public void GainPoint(int point,float mutiply)
    {
        playerPoint += (int)(point * mutiply);
        playerPointText.GetComponent<LerpChange>().SetText(playerPoint);
        AudioManager.Instance.PlaySFX(gainPointSFX);
    }
    public void GaindrawCharge(int amount)
    {
        drawCharge += amount;
    }
    
}

