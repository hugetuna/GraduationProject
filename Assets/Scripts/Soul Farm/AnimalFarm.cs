using System.Collections;
using System.Collections.Generic;
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
    public Button addFoodBarnButton;
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
    //安全切換 Map
    private void SwitchActionMap(string mapName)
    {
        if (teamManager != null)
        {
            PlayerInput captain = teamManager?.teamMembers[
            teamManager.currentLeaderIndex].GetComponent<PlayerInput>();
            if (captain != null)
            {
                captain.SwitchCurrentActionMap(mapName);
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
        //顯示並選取按鈕
        SwitchActionMap("FarmConfig");
        // 讓主畫面 UI 看得到但點不到，且不接受鍵盤導覽
        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.interactable = false;
            mainUICanvasGroup.blocksRaycasts = false;
        }
        farmCanvas.gameObject.SetActive(true);
        StartCoroutine(SelectButtonWithDelay());// 等待一幀再選取按鈕，確保不會被當前的空白鍵觸發 onClick
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
    public void HideInteractionUI()
    {
        SwitchActionMap("PlayerActionMain");
        // 讓主畫面 UI 看得到但點不到，且不接受鍵盤導覽
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
    //種植個種子
    public void PlantSeed()
    {
        if (seedsOnThisSoil.Count < maxSeedAmount)
        {
            //生成種子
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);//設定種子生成的旋轉角度
            GameObject newSeed = Instantiate(seedPrefabs[(int)farmLV], seedSpawnPoint.position, rotation);
            seedsOnThisSoil.Add(newSeed.GetComponent<SeedInstanceScript_Animal>());
            //消耗體力
            IdolInstance leader = teamManager.teamMembers[teamManager.currentLeaderIndex].GetComponent<IdolInstance>();
            leader.costVigour(leader.plantVigourCost);
        }
        else
        {
            Debug.Log("此農場已達最大種植數量");
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
        }
        else
        {
            foodBarn = foodBarnMax;
            Debug.Log("食物欄位已滿");
        }
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

                return; // 成功收割一個就跳出
            }
        }
        Debug.Log("沒有可以收割的種子");
    }


}
