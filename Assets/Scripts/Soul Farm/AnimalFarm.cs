using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum FarmLV
{
    Low,
    Medium,
    High
}
public class AnimalFarm : MonoBehaviour, IInteractable
{
    public string InteractionKey => "TGrow"; // 這個字串用來指定動畫 key

    public FarmLV farmLV;
    public bool isActivated = false; // 是否已經開啟這個農場的使用權
    public Transform seedSpawnPoint; // 種子的生成位置
    public GameObject[] seedPrefabs; // 儲存不同種類的種子預製體
    
    public OrderSet orderSeter;
    [Header("種田數值紀錄")]
    public List<SeedInstanceScript_Animal> seedsOnThisSoil;//紀錄所有被種植的種子
    public int maxSeedAmount=3;//最大種植數量
    public int foodBarn=0;//食物欄位
    public int foodBarnMax=0;//食物欄位上限
    [Header("介面")]
    public Canvas farmCanvas;//互動按鈕介面
    public Button plantSeedButton;
    public TextMeshProUGUI plantCounting;
    public Button addFoodBarnButton;
    public TextMeshProUGUI foodBarnCounting;
    public Button harvestSeedButton;
    public Button exitButton;
    private GameObject lastSelected; // 紀錄最後一個選取的物件
    [Header("外部 UI 阻斷")]
    public CanvasGroup mainUICanvasGroup; // 拖入主畫面的 CanvasGroup
    [Header("更改玩家操作")]
    public PlayerInput playerInput;
    [Header("Manager")]
    public TeamManager teamManager;
    public ResourceManager resourceManager;
    public SoilManager soilManager;
    [Header("種田相關音效")]
    public AudioClip audio_PlantSeed;
    public AudioClip audio_WaterSeed;
    public AudioClip audio_HarvestSeed;
    [Header("教學用")]
    public bool isTutorialFinished = false;

    private void Start()
    {
        
        teamManager = FindAnyObjectByType<TeamManager>();
        resourceManager = FindAnyObjectByType<ResourceManager>();
        soilManager = FindAnyObjectByType<SoilManager>();
    }
    void Update()
    {
        // 只有在農場 UI 開啟時才執行此邏輯
        if (farmCanvas.gameObject.activeInHierarchy)
        {
            // 如果當前有選取物件，更新紀錄
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                lastSelected = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                // 如果選取不見了（點到外面），強制選回最後一個紀錄的物件
                if (lastSelected != null)
                {
                    EventSystem.current.SetSelectedGameObject(lastSelected);
                }
                else
                {
                    // 如果連紀錄都沒有，就選回預設的第一個按鈕
                    plantSeedButton.Select();
                }
            }
        }
    }
    public void updateFarmButtonInteractable()
    {
        if (!isTutorialFinished) return;
        plantSeedButton.interactable = !(seedsOnThisSoil.Count == maxSeedAmount);
        addFoodBarnButton.interactable = !(foodBarn >= foodBarnMax);
        harvestSeedButton.interactable = seedsOnThisSoil.Count > 0;
        exitButton.interactable = true;
    }
    //安全切換 Map
    private void SwitchActionMap(string mapName)
    {
        if (teamManager != null)
        {
            foreach (PlayerControlMainWorld member in teamManager.teamMembers)
            {
                if (member != null)
                {
                    member.SwitchSelfActionMap(mapName);
                }
            }
        }
        if (playerInput != null)
        {
            playerInput.SwitchCurrentActionMap(mapName);
            Debug.Log($"Action Map 切換至: {mapName}");
        }
    }
    public void ShowInteractionUI()
    {
        SwitchActionMap("FarmConfig");
        // 讓主畫面 UI 看得到但點不到，且不接受鍵盤導覽
        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.interactable = false;
            mainUICanvasGroup.blocksRaycasts = false;
        }
        UpdateCountingText();
        farmCanvas.gameObject.SetActive(true);
        StartCoroutine(SelectButtonWithDelay());// 等待一幀再選取按鈕，確保不會被當前的空白鍵觸發 onClick
        if (DayManager.Instance.date == 2&&DayManager.Instance.dayEventManager.currentEvent.Type==EventType.WaitTutorialEnd)//第二天教學專用
        {
            StartCoroutine(FarmButtomTutorial());
        }
    }
    private IEnumerator SelectButtonWithDelay()
    {
        // 先清空所有選取，防止意外觸發
        EventSystem.current.SetSelectedGameObject(null);

        // 等待一幀，讓這幀的 Input 訊號結束
        yield return null;

        // 這時候再選取，就不會被當前的空白鍵觸發 onClick 了
        plantSeedButton.Select();
    }
    public IEnumerator ControlAllButtons(bool enable)
    {
        EventSystem.current.SetSelectedGameObject(null);
        WaitForSeconds wait = new WaitForSeconds(0.2f); // 0.2秒的延遲
        yield return wait; // 等待延遲時間
        plantSeedButton.interactable = enable;
        addFoodBarnButton.interactable = enable;
        harvestSeedButton.interactable = enable;
        exitButton.interactable = enable;
    }
    public IEnumerator FarmButtomTutorial()
    {
        Debug.Log("開始農場教學");
        bool step1Completed = false;//第一步：引導玩家點擊種植按紐
        bool step2Completed = false;//第二步：引導玩家點擊澆灌按紐直到全滿
        // 先清空所有選取，防止意外觸發
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(ControlAllButtons(false));// 先禁用所有按鈕，確保玩家只能點擊被教學引導的按鈕
        WaitForSeconds wait = new WaitForSeconds(0.5f); // 0.5秒的延遲
        yield return wait; // 等待延遲時間
        plantSeedButton.interactable = true;
        plantSeedButton.Select();
        while (!step1Completed)
        {
            if (seedsOnThisSoil.Count > 0)
            {
                step1Completed = true;
            }
            yield return null;
        }
        plantSeedButton.interactable = false;
        while (!step2Completed)
        {
            addFoodBarnButton.interactable = true;
            addFoodBarnButton.Select();
            if (foodBarn >= foodBarnMax)
            {
                step2Completed = true;
            }
            yield return null;
        }
        StartCoroutine(ControlAllButtons(true));
        HideInteractionUI();
        isTutorialFinished = true;
    }
    public void HideInteractionUI()
    {
        SwitchActionMap("PlayerActionMain");
        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.interactable = true;
            mainUICanvasGroup.blocksRaycasts = true;
        }
        farmCanvas.gameObject.SetActive(false);
    }
    void IInteractable.Interact(int toolType) // 互動行為
    {
        ShowInteractionUI();
    }
    //按下種植按鈕(因為按鈕回傳值不能是SeedInstanceScript_Animal所以千套)
    public void pressPlantButton()
    {
        //種植種子(消耗玩家道具)
        PlantSeed();
        teamManager?.teamMembers[teamManager.currentLeaderIndex].GetComponent<PlayerControlMainWorld>().OnFarmAnimation();
        StartCoroutine(ButtonCooldown(plantSeedButton, 0.2f)); // 種植按鈕冷卻0.2秒
        //AudioManager.Instance.PlaySFX(audio_PlantSeed);
    }
    public IEnumerator ButtonCooldown(Button button, float cooldownTime)
    {
        button.interactable = false; // 禁用按鈕
        yield return new WaitForSeconds(cooldownTime); // 等待冷卻時間
        updateFarmButtonInteractable(); // 根據當前狀態更新按鈕互動性
    }
    //種植個種子
    public SeedInstanceScript_Animal PlantSeed()
    {
        if (seedsOnThisSoil.Count < maxSeedAmount)
        {
            //生成種子
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);//設定種子生成的旋轉角度
            GameObject newSeed = Instantiate(seedPrefabs[(int)farmLV], seedSpawnPoint.position, rotation);
            seedsOnThisSoil.Add(newSeed.GetComponent<SeedInstanceScript_Animal>());
            //消耗體力
            //IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
            //leader.costVigour(leader.plantVigourCost);
            UpdateCountingText();
            updateFarmButtonInteractable();
            return newSeed.GetComponent<SeedInstanceScript_Animal>();
        }
        else
        {
            Debug.Log("此農場已達最大種植數量");
            return null;
        }
        
    }
    //補充食物欄位
    public void AddFoodBarn(int amount)
    {

        if (foodBarn + amount <= foodBarnMax)
        {
            foodBarn += amount;
            //消耗體力
            IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
            leader.costVigour(leader.waterVigourCost);
            leader.gameObject.GetComponent<PlayerControlMainWorld>().OnFarmAnimation();
        }
        else
        {
            foodBarn = foodBarnMax;
            Debug.Log("食物欄位已滿");
        }
        UpdateCountingText();
        updateFarmButtonInteractable();
        StartCoroutine(ButtonCooldown(addFoodBarnButton, 0.2f));
    }
    //種子消耗食物欄位(每天結束時)
    public void SeedsConsumeBarn()
    {
        if (foodBarn > 0)
        {
            foreach (SeedInstanceScript_Animal seed in seedsOnThisSoil)
            {
                if (!seed.GetIsWateredToday())
                {
                    seed.Water();
                    foodBarn--;
                }
            }
        }
        else
        {
            Debug.Log("食物欄位沒有食物了");
        }
    }
    //Harvest種子
    public void HarvestSeed()
    {
        // 使用倒序迴圈，這樣在刪除元素時才不會導致索引出錯
        for (int i = seedsOnThisSoil.Count - 1; i >= 0; i--)
        {
            SeedInstanceScript_Animal seed = seedsOnThisSoil[i];

            // 檢查 1: 物件是否還活著
            if (seed == null)
            {
                seedsOnThisSoil.RemoveAt(i);
                continue;
            }

            // 檢查 2: 是否成熟
            if (seed.GetDaysGrown() >= seed.seedData.growthDays)
            {
                IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();

                // 執行收割邏輯
                int seedRewardPoint = seed.Harvest();

                int finalSeedRewardPoint = Random.Range(seedRewardPoint - 80 + leader.charm, seedRewardPoint + 30 + leader.charm);
                FansItem newFan = soilManager.RollFansItem(finalSeedRewardPoint, leader.idolIndex);
                resourceManager.AddItem(newFan);

                AudioManager.Instance.PlaySFX(audio_HarvestSeed);

                // 重要：從 List 移除並銷毀物件
                seedsOnThisSoil.RemoveAt(i);
                Destroy(seed.gameObject);
                UpdateCountingText();
                updateFarmButtonInteractable();
                return; // 成功收割一個就跳出
            }
        }
        Debug.Log("沒有可以收割的種子");
        StartCoroutine(ButtonCooldown(harvestSeedButton, 0.2f));
    }
    public void UpdateCountingText()
    {
        plantCounting.text = $"{seedsOnThisSoil.Count}/{maxSeedAmount}";
        foodBarnCounting.text = $"{foodBarn}/{foodBarnMax}";
    }

}
