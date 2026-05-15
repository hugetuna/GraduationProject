using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

/* 掛在 Settle UI prefab 的根部上 */
public class SetSettleUI : MonoBehaviour
{
    [Header("UI 元素")]
    [SerializeField] private List<GameObject> characters = new(); // 另外呼叫各自底下的函式設定內容
    [SerializeField] private TextMeshProUGUI moneyText;
    // [SerializeField] private List<Image> itemImages = new(); // 以後會改生成
    [SerializeField] private Button endDayButton;
    //-----------------------------------------------------------------//
    [Header("顯示資料")]
    // [SerializeField] private ResourceManager resourceManager;
    // [SerializeField] private List<ItemStack> itemsEarned;
    private int moneyEarned = 0;
    private bool isSettleActivityMoney = false; // 是否已結算商演賺的錢（避免全員重複計算）
    [SerializeField] private TeamManager teamManager;
    private List<IdolInstance> idolInstances;
    //-----------------------------------------------------------------//
    [Header("背景音樂")]
    [SerializeField] private AudioClip settleBGM;


    public void ShowTodayBenefits() // 結算 UI 打開時才呼叫執行（每天一次）
    {
        // 播放背景音樂（一次性）
        if (settleBGM != null)
        {
            AudioManager.Instance.StopMusic(); // 停掉 4F 的配樂
            AudioManager.Instance.PlaySFX(settleBGM);
        }

        // 呼叫角色顯示（已排序）＆計算金錢變化（還沒有完全改好）
        idolInstances = TeamDataUtility.IdolInstanceList;
        if (DayManager.Instance.day == 1) moneyEarned = 1000;

        foreach (var idol in idolInstances)
        {
            AvailableAction action = idol.currentAction;
            if (action == AvailableAction.Train) ShowTrainingBenefits(idol);
            else if (action == AvailableAction.Activity) ShowActivityBenefits(idol);
            else if (action == AvailableAction.Baito) ShowBaitoBenefits(idol);
            else ShowFreeBenefits(idol); // 閒置狀態（沒有做任何事）也要顯示基本資料，但不會有金錢或經驗值變化
        }

        // 金錢資料顯示
        moneyText.text = $"+{moneyEarned}";

        // 裝備和物品顯示（目前沒東西...但如果商店可以買東西再來改）
        // 正式結算請放到 DayManager 的 EndDay() 之類的地方
        endDayButton.onClick.RemoveAllListeners(); // 避免重複綁定
        endDayButton.onClick.AddListener(OnEndDayButtonClicked);
    }

    private void ShowTrainingBenefits(IdolInstance idol)
    {
        // 根據訓練紀錄來計算體力和經驗值的變化，並更新 UI 顯示（尚未正式結算）
        int index = idolInstances.IndexOf(idol);
        GameObject character = characters[index];

        int finalVigour = idol.vigour - idol.trainRecord.vigourCost;
        character.GetComponent<SetSettleCharacterUI>().ShowCharacterBenefits(
            idol.idolUISprites.spriteTachie,
            idol.basicStatus.idolName,
            finalVigour, idol.vigourMax, 0,  // 目前沒有最大體力值變動
            idol.dance, idol.trainRecord.danceExp,
            idol.vocal, idol.trainRecord.vocalExp,
            idol.visual, idol.trainRecord.visualExp,
            idol.performance, 0 // 目前沒有演技變動
        );

        // moneyEarned += 0; // 訓練不會賺錢，也不會拿到道具
    }

    private void ShowBaitoBenefits(IdolInstance idol)
    {
        // 根據打工紀錄來計算體力和經驗值的變化，並更新 UI 顯示（尚未正式結算）
        int index = idolInstances.IndexOf(idol);
        GameObject character = characters[index];

        int finalVigour = idol.vigour - idol.baitoRecord.selectedBaito.vigourCost;
        character.GetComponent<SetSettleCharacterUI>().ShowCharacterBenefits(
            idol.idolUISprites.spriteTachie,
            idol.basicStatus.idolName,
            finalVigour, idol.vigourMax, 0,  // 目前沒有最大體力值變動
            idol.dance, 0, // 沒有舞蹈經驗變動
            idol.vocal, 0, // 沒有歌唱經驗變動
            idol.visual, 0, // 沒有表現力經驗變動
            idol.performance, 0 // 目前沒有演技變動
        );

        moneyEarned += idol.baitoRecord.selectedBaito.MoneyGain; // 打工會賺錢，但不會拿到道具
    }

    private void ShowActivityBenefits(IdolInstance idol)
    {
        // 根據商演紀錄來計算體力和經驗值的變化，並更新 UI 顯示（尚未正式結算）
        int index = idolInstances.IndexOf(idol);
        GameObject character = characters[index];

        int finalVigour = idol.vigour - idol.activityRecord.selectedActivity.vigourCost;
        finalVigour = Mathf.Max(finalVigour, 0); // 確保體力不會變成負數
        character.GetComponent<SetSettleCharacterUI>().ShowCharacterBenefits(
            idol.idolUISprites.spriteTachie,
            idol.basicStatus.idolName,
            finalVigour, idol.vigourMax, 0,  // 目前沒有最大體力值變動
            idol.dance, 0, // 沒有舞蹈經驗變動
            idol.vocal, 0, // 沒有歌唱經驗變動
            idol.visual, 0, // 沒有表現力經驗變動
            idol.performance, 0 // 目前沒有演技變動
        );

        if (!isSettleActivityMoney)
        {
            moneyEarned += idol.activityRecord.selectedActivity.MoneyGain; // 商演會賺錢，但不會拿到道具
            isSettleActivityMoney = true; // 確保只結算一次商演賺的錢
        }
    }

    private void ShowFreeBenefits(IdolInstance idol)
    {
        // 設定閒置狀態 UI
        int index = idolInstances.IndexOf(idol);
        GameObject character = characters[index];

        character.GetComponent<SetSettleCharacterUI>().ShowCharacterBenefits(
            idol.idolUISprites.spriteTachie,
            TeamDataUtility.GetIdolNameTW(idol.idolIndex),
            idol.vigour, idol.vigourMax, 0,  // 目前沒有最大體力值變動
            idol.dance, 0,
            idol.vocal, 0,
            idol.visual, 0,
            idol.performance, 0 // 目前沒有演技變動
        );

        // 不在這裡處理金錢和道具變化
    }

    private void OnEndDayButtonClicked()
    {
        foreach (var idol in idolInstances)
        {
            // 結算角色的訓練、打工和商演紀錄，並重置狀態以準備新的一天
            idol.SettleRecords(); 
            idol.isAvailable = true;
            idol.currentAction = AvailableAction.Free;
        }
        DayManager.Instance.AfterDayEndEventStart();
    }
}
