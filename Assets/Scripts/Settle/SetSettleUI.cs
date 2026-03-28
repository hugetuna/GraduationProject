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
    private int moneyEarned;
    [SerializeField] private TeamManager teamManager;
    private List<IdolInstance> idolInstances;
    //-----------------------------------------------------------------//
    [Header("背景音樂")]
    [SerializeField] private AudioClip settleBGM;


    public void ShowTodayBenefits() // 結算 UI 打開時才呼叫執行（每天一次）
    {
        // 播放背景音樂（一次性）
        if(settleBGM != null){
            AudioManager.Instance.StopMusic(); // 停掉 4F 的配樂
            AudioManager.Instance.PlaySFX(settleBGM);
        }

        // 呼叫角色顯示（已排序）
        idolInstances = TeamDataUtility.IdolInstanceList;

        foreach(var idol in idolInstances)
        {
            int index = idolInstances.IndexOf(idol);
            GameObject character = characters[index];

            int finalVigour = idol.vigour - idol.trainRecord.vigourCost;
            character.GetComponent<SetSettleCharacterUI>().ShowCharacterBenefits(
                idol.spriteTachie,
                idol.basicStatus.idolName,
                finalVigour, idol.vigourMax, 0,  // 目前沒有最大體力值變動
                idol.dance, idol.trainRecord.danceExp,
                idol.vocal, idol.trainRecord.vocalExp,
                idol.visual, idol.trainRecord.visualExp,
                idol.performance, 0 // 目前沒有演技變動
            );
        }

        // 金錢資料顯示（目前沒東西所以先寫死）
        moneyEarned = 1000;
        moneyText.text = $"+{moneyEarned}";

        // 裝備和物品顯示（目前沒東西...但如果商店可以買東西再來改）
        // 正式結算請放到 DayManager 的 EndDay() 之類的地方
        endDayButton.onClick.RemoveAllListeners(); // 避免重複綁定
        endDayButton.onClick.AddListener(OnEndDayButtonClicked);
    }

    private void OnEndDayButtonClicked()
    {
        foreach (var idol in idolInstances)
        {
            idol.SettleTrainRecord(); // 換天前搞定訓練結算
            idol.SettleBaitoRecord(); // 同上，這是打工結算
            idol.isAvailable = true;
        }
        DayManager.Instance.AfterDayEndEventStart();
    }
}
