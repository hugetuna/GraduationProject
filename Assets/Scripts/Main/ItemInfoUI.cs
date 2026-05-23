using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Live2D.Cubism.Core;

/* 掛在背包 UI 根部，管理道具按鈕及道具詳細資訊 */
public class ItemInfoUI : MonoBehaviour
{
    [SerializeField] private Image itemInfoIcon; // 道具詳細資訊的圖示
    [SerializeField] private TextMeshProUGUI itemInfoName; // 道具詳細資訊的名稱
    [SerializeField] private TextMeshProUGUI itemInfoDescription; // 道具詳細資訊的描述
    [SerializeField] private TextMeshProUGUI itemInfoAdditional; // 道具詳細資訊的持續天數或持有角色
    [SerializeField] private GameObject dropdownMenu; // 選擇道具使用對象的下拉選單
    private TextMeshProUGUI dropdownLabel; // 下拉選單的標籤，用於顯示選擇的對象
    private Image dropdownArrow; // 下拉選單的箭頭圖示
    [SerializeField] private GameObject useItemButton; // 確認使用道具的按鈕
    private Button useButtonComponent;
    //-----------------------------------------------------------------//
    private List<Button> itemButtons = new(); // 儲存所有道具項目按鈕
    private Item selectedItem = null; // 當前選擇的道具
    public Item SelectedItem { get { return selectedItem; } }
    //-----------------------------------------------------------------//
    private Vector2 originalPos = Vector2.zero; // 按鈕們的起始位置
    public Vector2 OriginalPos { get { return originalPos; } set { originalPos = value; } }
    private Vector2 offset = new(11.0f, 0); // 被按下的按鈕會往右移動的距離
    //-----------------------------------------------------------------//
    private bool isInitialized = false;

    void Awake()
    {
        dropdownLabel = dropdownMenu.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        dropdownArrow = dropdownMenu.transform.Find("Arrow").GetComponent<Image>();
        useButtonComponent = useItemButton.GetComponent<Button>();
    }

    void Start()
    {
        ResetItemInfo(); // 初始化道具詳細資訊
        isInitialized = true;
    }

    void OnEnable()
    {
        if (isInitialized) ResetItemInfo(); // 每次開啟時先清空道具詳細資訊
    }

    public void OnItemClicked(Button clickedButton)
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            // 一般按鈕
            RectTransform rt = itemButtons[i].GetComponent<RectTransform>();
            rt.localPosition = originalPos;
        }

        // 被按下的按鈕（唯一）
        RectTransform clickedRt = clickedButton.GetComponent<RectTransform>();
        clickedRt.localPosition = originalPos + offset;

        selectedItem = clickedButton.GetComponent<SetItemUI>().Item;
        itemInfoName.text = selectedItem.itemName;
        itemInfoDescription.text = selectedItem.description;
        itemInfoIcon.sprite = selectedItem.icon;

        // 根據不同的道具類型顯示不同的資訊
        if (selectedItem is ConsumableItem consumable)
        {
            if (consumable.itemID.Contains("CS")) // 粉絲種子
            {
                itemInfoAdditional.text = "";
                SetDropdownInteractable(false);
                useButtonComponent.interactable = false;
            }
            else
            {
                itemInfoAdditional.text = $"持續天數：{consumable.duration}";

                SetDropdownInteractable(true);
                useButtonComponent.interactable = true;
            }
        }
        else if (selectedItem is FansItem fansItem)
        {
            string harvesterName = "";
            if (fansItem.harvester != IdolWho.none && TeamDataUtility.IdolDict.ContainsKey(fansItem.harvester))
            {
                harvesterName = TeamDataUtility.GetIdolNameTW(fansItem.harvester);
            }

            if (harvesterName != "") itemInfoAdditional.text = $"持有角色：{harvesterName}";
            else itemInfoAdditional.text = "";

            SetDropdownInteractable(false);
            useButtonComponent.interactable = false;
        }
        else // if (selectedItem is EquipmentItem equipment)
        {
            itemInfoAdditional.text = "";

            SetDropdownInteractable(false);
            useButtonComponent.interactable = false;
        }

        // 確保字型正確渲染
        itemInfoName.ForceMeshUpdate();
        itemInfoDescription.ForceMeshUpdate();
    }

    public void ResetItemInfo()
    {
        for (int i = 0; i < itemButtons.Count; i++)
        {
            RectTransform rt = itemButtons[i].GetComponent<RectTransform>();
            rt.localPosition = originalPos;
        }

        // 重置道具詳細資訊，並確保字形能正確渲染
        itemInfoIcon.sprite = null;
        itemInfoName.text = "";
        itemInfoDescription.text = "";
        itemInfoAdditional.text = "";

        SetDropdownInteractable(false);
        useButtonComponent.interactable = false;

        itemInfoName.ForceMeshUpdate();
        itemInfoDescription.ForceMeshUpdate();
        itemInfoAdditional.ForceMeshUpdate();

        selectedItem = null;
    }

    public void AddToItemButtons(Button btn)
    {
        itemButtons.Add(btn);
    }

    public void ClearItemButtons()
    {
        itemButtons.Clear();
    }

    private void SetDropdownInteractable(bool interactable)
    {
        dropdownMenu.GetComponent<TMP_Dropdown>().interactable = interactable;

        byte alpha = interactable ? (byte)255 : (byte)128;
        dropdownLabel.color = new Color32(50, 50, 50, alpha);
        dropdownArrow.color = new Color32(255, 255, 255, alpha);
    }
}
