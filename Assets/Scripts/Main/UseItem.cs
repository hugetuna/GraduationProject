using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

/* 掛在背包 UI 的 UseButton 上，按下按鈕時會將選擇的道具用在特定角色上 */
[DefaultExecutionOrder(0)]
public class UseItem : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown; // 可選擇使用道具的角色之下拉選單
    [SerializeField] private string selectedCharacterName; // 儲存選擇的角色名稱
    //-----------------------------------------------------------------//
    private List<IdolInstance> idolInstances; // 存放偶像資料參考
    private IdolInstance itemUser; // 使用道具的角色
    //-----------------------------------------------------------------//
    [SerializeField] private AudioClip UseItemSound;


    [System.Obsolete]
    void Start()
    {
        InitializeWhenStart();
        PackUIHandler.OnPackUIClosed += ResetDropdown; // 訂閱背包 UI 關閉事件
    }

    void OnDestroy()
    {
        PackUIHandler.OnPackUIClosed -= ResetDropdown; // 取消訂閱背包 UI 關閉事件
    }

    private void InitializeWhenStart()
    {
        // 根據目前隊伍成員決定下拉選單的選項
        dropdown.options.Clear(); // 清空原有選項

        idolInstances = TeamDataUtility.IdolInstanceList;
        var teamMembers = idolInstances.Select(idol => idol.GetComponent<PlayerControlMainWorld>()).ToList();

        for (int i = 0; i < teamMembers.Count; i++) // 確保不會超出陣列範圍
        {
            string memberName = TeamDataUtility.CleanNameOfCharacterObject(teamMembers[i].name); // 取得隊伍成員名稱
            dropdown.options.Add(new TMP_Dropdown.OptionData("給 " + memberName)); // 新增選單項目
        }
        ResetDropdown();

        // 設定下拉選單的事件監聽器
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // 設定按鈕的事件監聽器
        GetComponent<Button>().onClick.AddListener(OnUseItem);
    }

    private void OnDropdownValueChanged(int index)
    {
        selectedCharacterName = dropdown.options[index].text;
    }

    private void OnUseItem()
    {
        // 使用道具的對象
        selectedCharacterName = selectedCharacterName.Replace("給 ", ""); // 去除名稱前綴
        var name = TeamDataUtility.GetIdolEnum(selectedCharacterName);
        var characterIndex = name == IdolWho.none ? IdolWho.none : name;
        if(characterIndex == IdolWho.none)
        {
            Debug.LogWarning("選擇的角色無效，無法使用道具！");
            return;
        }

        for (int i = 0; i < idolInstances.Count; i++) // 確保不會超出陣列範圍
        {
            if (idolInstances[i].idolIndex == characterIndex) // 找到對應的偶像資料
            {
                itemUser = idolInstances[i]; // 設定角色資訊
                break; // 找到後跳出迴圈
            }
        }

        // 欲使用的道具（可從 itemInfoUI 獲取）
        Item item = ItemInfoUI.SelectedItem;
        if (item == null)
        {
            Debug.LogWarning("未選擇任何道具，無法使用！");
            return;
        }

        // 使用道具
        if (item.itemType == ItemType.Consumable)
        {
            var itemToUse = item as ConsumableItem;
            itemToUse.Use(itemUser);
        }
        else if (item.itemType == ItemType.Fans)
        {
            var itemToUse = item as FansItem;
            itemToUse.Use(itemUser);
        }
        AudioManager.Instance.PlaySFX(UseItemSound); // 播放音效

        // 裝備的使用尚未實作
    }

    public void ResetDropdown()
    {
        dropdown.value = 0; // 預設選擇第一個選項
        dropdown.RefreshShownValue(); // 確保 UI 正確顯示
        selectedCharacterName = dropdown.options[0].text; // 初始化選擇的角色名稱，格式為「給 角色名稱」
    }
}
