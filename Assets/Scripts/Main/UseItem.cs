using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/* 掛在背包頁面的使用按鈕上，按下按鈕時會使用選擇的道具在特定角色上 */
[DefaultExecutionOrder(0)]
public class UseItem : MonoBehaviour
{
    public TMP_Dropdown dropdown; // 可選擇使用道具的角色之下拉選單
    [SerializeField]
    private string selectedCharacterName; // 儲存選擇的角色名稱
    //-----------------------------------------------------------------//
    public TeamManager teamManager; // 透過 TeamManager 物件取得當前隊伍成員
    [SerializeField]
    private List<PlayerControlMainWorld> teamMembers = new(); // 記錄取得的隊伍成員
    //-----------------------------------------------------------------//
    private IdolInstance[] idolInstance; // 存放偶像資料參考
    private IdolInstance itemUser; // 使用道具的角色
    public ItemInfoUI itemInfoUI; // 用於獲取欲使用的道具資訊
    //-----------------------------------------------------------------//
    public AudioClip UseItemSound;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    [System.Obsolete]
    private void Start()
    {
        // 根據目前隊伍成員決定下拉選單的選項
        dropdown.options.Clear(); // 清空原有選項

        teamMembers = teamManager.teamMembers;

        for (int i = 0; i < teamMembers.Count; i++) // 確保不會超出陣列範圍
        {
            string memberName = teamMembers[i].name; // 取得隊伍成員名稱
            // 去除前後綴（只剩名字）
            int front = memberName.IndexOf("_");
            int end = memberName.IndexOf("2");
            memberName = memberName.Substring(front + 1, end - front - 1);
            Debug.Log("隊伍成員名稱：" + memberName);
            dropdown.options.Add(new TMP_Dropdown.OptionData("給 " + memberName));
        }
        dropdown.value = 0; // 預設選擇第一個選項
        dropdown.RefreshShownValue(); // 確保 UI 正確顯示
        selectedCharacterName = dropdown.options[0].text; // 初始化選擇的角色名稱，格式為「給 角色名稱」

        // 設定下拉選單的事件監聽器
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        // 設定按鈕的事件監聽器
        GetComponent<Button>().onClick.AddListener(OnUseItem);

        // 獲取場景中所有具備 IdolInstance 的物件
        idolInstance = FindObjectsByType<IdolInstance>(FindObjectsSortMode.None);
    }

    private void OnDropdownValueChanged(int index)
    {
        selectedCharacterName = dropdown.options[index].text;
    }

    private void OnUseItem()
    {
        // 使用道具的對象
        selectedCharacterName = selectedCharacterName.Replace("給 ", ""); // 去除名稱前綴
        IdolWho characterIndex = IdolWho.none;
        if (selectedCharacterName == "Kuma") characterIndex = IdolWho.Kuma;
        else if (selectedCharacterName == "Karo") characterIndex = IdolWho.Karo;
        else if (selectedCharacterName == "Sirius") characterIndex = IdolWho.Sirius;

        for (int i = 0; i < idolInstance.Length; i++) // 確保不會超出陣列範圍
        {
            if (idolInstance[i].idolIndex == characterIndex) // 找到對應的偶像資料
            {
                itemUser = idolInstance[i]; // 設定角色資訊
                break; // 找到後跳出迴圈
            }
        }

        // 欲使用的道具（可從 itemInfoUI 獲取）
        Item item = itemInfoUI.selectedItem;

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
        audioSource.PlayOneShot(UseItemSound); // 播放音效

        // 裝備的使用尚未實作
    }

    public void ResetDropdown()
    {
        dropdown.value = 0; // 重置為第一個選項
        dropdown.RefreshShownValue(); // 確保 UI 正確顯示
        selectedCharacterName = dropdown.options[0].text; // 重置選擇的角色名稱
    }
}
