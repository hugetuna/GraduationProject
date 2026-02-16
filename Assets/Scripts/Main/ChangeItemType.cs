using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/* 掛在背包 UI 底下的 TypeSelector 上 */
public class ChangeItemType : MonoBehaviour
{
    [Header("分類按鈕設定")]
    [SerializeField] private List<Button> itemTypeButtons = new(); // 儲存按鈕的列表，分別代表消耗品、粉絲和裝備
    [SerializeField] private Sprite activeBtnImg;
    [SerializeField] private Sprite normalBtnImg;
    //-----------------------------------------------------------------//
    [SerializeField] private PackUIAction packUIAction; // 用於處理背包 UI 的 input system
    private int currentIndex = 0; // 當前選中的按鈕索引
    //-----------------------------------------------------------------//
    [Header("分類頁面")]
    [SerializeField] private GameObject consumablePage;
    [SerializeField] private GameObject fansPage;
    [SerializeField] private GameObject equipPage;
    //-----------------------------------------------------------------//
    private bool isInitialized = false;

    void Awake()
    {
        packUIAction = new PackUIAction();
    }

    void OnEnable()
    {
        packUIAction.TypeControl.SwitchLeft.performed += OnSwitchLeft;
        packUIAction.TypeControl.SwitchRight.performed += OnSwitchRight;
        packUIAction.TypeControl.SwitchLeft.Enable();
        packUIAction.TypeControl.SwitchRight.Enable();

        if(isInitialized) ResetTypeUI(); // 如果已經初始化過了，改為重置預設頁面
    }

    void OnDisable()
    {
        packUIAction.TypeControl.SwitchLeft.performed -= OnSwitchLeft;
        packUIAction.TypeControl.SwitchRight.performed -= OnSwitchRight;
        packUIAction.TypeControl.SwitchLeft.Disable();
        packUIAction.TypeControl.SwitchRight.Disable();
    }

    private void OnSwitchLeft(InputAction.CallbackContext ctx)
    {
        //currentIndex = (currentIndex - 1 + itemTypeButtons.Count) % itemTypeButtons.Count;
        if (currentIndex > 0) currentIndex--;
        SelectButton(currentIndex);
    }

    private void OnSwitchRight(InputAction.CallbackContext ctx)
    {
        // currentIndex = (currentIndex + 1) % itemTypeButtons.Count;
        if (currentIndex < itemTypeButtons.Count - 1) currentIndex++;
        SelectButton(currentIndex);
    }

    private void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(itemTypeButtons[index].gameObject);
        OnButtonClick(itemTypeButtons[index]);
    }

    //-----------------------------------------------------------------//

    void Start()
    {
        // 設定按鈕的點擊事件
        foreach (Button btn in itemTypeButtons)
        {
            Button tempBtn = btn; // 捕捉當下按鈕以避免閉包問題
            tempBtn.onClick.AddListener(() => OnButtonClick(tempBtn));
        }

        ResetTypeUI(); // 初始化為預設頁面
        isInitialized = true;
    }

    public void OnButtonClick(Button clickedButton)
    {
        for (int i = 0; i < itemTypeButtons.Count; i++)
        {
            // 一般按鈕：切換成普通圖片
            Image img = itemTypeButtons[i].GetComponent<Image>();
            img.sprite = normalBtnImg;
            img.SetNativeSize(); // 確保圖片大小正確
        }

        // 被按下的按鈕（唯一）：切換成選中圖片，並顯示對應的道具頁面
        Image clickedImg = clickedButton.GetComponent<Image>();
        clickedImg.sprite = activeBtnImg;
        clickedImg.SetNativeSize(); // 確保圖片大小正確
        if (clickedButton == itemTypeButtons[0]) // 假設第一個按鈕是消耗品
        {
            consumablePage.SetActive(true);
            fansPage.SetActive(false);
            equipPage.SetActive(false);
            currentIndex = 0; // 更新當前索引
        }
        else if (clickedButton == itemTypeButtons[1]) // 假設第二個按鈕是粉絲
        {
            consumablePage.SetActive(false);
            fansPage.SetActive(true);
            equipPage.SetActive(false);
            currentIndex = 1; // 更新當前索引
        }
        else if (clickedButton == itemTypeButtons[2]) // 假設第三個按鈕是裝備
        {
            consumablePage.SetActive(false);
            fansPage.SetActive(false);
            equipPage.SetActive(true);
            currentIndex = 2; // 更新當前索引
        }
    }

    public void ResetTypeUI() // 預設顯示消耗品頁面（已在 Unity 編輯器中預先設定好圖片）
    {
        consumablePage.SetActive(true);
        fansPage.SetActive(false);
        equipPage.SetActive(false);

        SelectButton(0); // 重置為第一個按鈕
    }
}
