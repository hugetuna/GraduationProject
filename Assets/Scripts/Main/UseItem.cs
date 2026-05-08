using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

/* 掛在背包 UI 根部，按下按鈕時會將選擇的道具用在特定角色上 */
[DefaultExecutionOrder(0)]
public class UseItem : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown; // 可選擇使用道具的角色之下拉選單
    [SerializeField] private Button useButton; // 確認使用道具的按鈕
    //-----------------------------------------------------------------//
    private string selectedCharacterName; // 儲存選擇的角色名稱
    private List<IdolInstance> idolInstances; // 存放偶像資料參考
    private IdolInstance itemUser; // 使用道具的角色
    //-----------------------------------------------------------------//
    [SerializeField] private GameObject hintPrefab; // 使用道具後的提示 prefab
    [SerializeField] private AudioClip UseItemSound;
    //-----------------------------------------------------------------//
    private ItemUIGenerator itemUIGenerator; // 用於用來刷新背包的道具生成
    private ItemInfoUI itemInfoUI; // 用於獲取當前選擇的道具資訊
    private bool isInitialized = false;


    [System.Obsolete]
    void Start()
    {
        itemInfoUI = GetComponent<ItemInfoUI>();
        itemUIGenerator = GetComponent<ItemUIGenerator>();

        // 根據目前隊伍成員決定下拉選單的選項
        dropdown.options.Clear(); // 清空原有選項

        idolInstances = TeamDataUtility.IdolInstanceList;

        foreach(var idol in idolInstances)
        {
            string memberName = TeamDataUtility.GetIdolNameTW(idol.idolIndex); // 取得隊伍成員名稱
            dropdown.options.Add(new TMP_Dropdown.OptionData("給 " + memberName)); // 新增選單項目
        }
        ResetDropdown(); // 初始化下拉選單
        isInitialized = true;

        // 設定下拉選單的事件監聽器
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // 設定使用按鈕的事件監聽器
        useButton.onClick.AddListener(OnUseItem);
    }

    void OnEnable()
    {
        if(isInitialized) ResetDropdown(); // 每次開啟時重置下拉選單
    }

    private void OnDropdownValueChanged(int index)
    {
        selectedCharacterName = dropdown.options[index].text;
    }

    private void OnUseItem()
    {
        // 使用道具的對象
        var characterName = selectedCharacterName.Replace("給 ", ""); // 去除名稱前綴
        var name = TeamDataUtility.GetIdolEnumTW(characterName);
        var characterIndex = name == IdolWho.none ? IdolWho.none : name;
        if(characterIndex == IdolWho.none)
        {
            Debug.LogWarning("選擇的角色無效，無法使用道具！");
            return;
        }

        foreach (var idol in idolInstances)
        {
            if (idol.idolIndex == characterIndex) // 找到對應的偶像資料
            {
                // 設定角色資訊，找到後跳出迴圈
                itemUser = idol; 
                break;
            }
        }

        // 欲使用的道具（可從 itemInfoUI 獲取）
        Item item = itemInfoUI.SelectedItem;
        if (item == null)
        {
            Debug.LogWarning("未選擇任何道具，無法使用！");
            return;
        }

        // 使用道具（目前只有 Consumable 可以在背包內使用）
        if (item.itemType == ItemType.Consumable)
        {
            var itemToUse = item as ConsumableItem;
            var resourceManager = ResourceManager.Instance;
            resourceManager.UseItem(itemToUse, itemUser); // 透過 ResourceManager 處理道具使用邏輯
            
            // 刷新背包 UI 以反映道具數量變化 ＆ 重置詳細資訊顯示
            itemUIGenerator.RefreshPackUI(); 
            if(resourceManager.GetItemCount(itemToUse) <= 0) itemInfoUI.ResetItemInfo();
        }
        
        AudioManager.Instance.PlaySFX(UseItemSound); // 播放音效
        GameObject hintInstance = Instantiate(hintPrefab, transform); // 生成使用道具提示
        hintInstance.GetComponent<PackHintToggler>().SetHintUI(characterName, item.itemName);

        // 裝備的使用尚未實作
    }

    public void ResetDropdown()
    {
        dropdown.value = 0; // 預設選擇第一個選項
        dropdown.RefreshShownValue(); // 確保 UI 正確顯示
        selectedCharacterName = dropdown.options[0].text; // 初始化選擇的角色名稱，格式為「給 角色名稱」
    }
}
