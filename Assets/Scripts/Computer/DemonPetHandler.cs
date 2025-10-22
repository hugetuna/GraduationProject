using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* 掛在 UIManager 等惡魔桌寵以外的物件上 */
public class DemonPetHandler : MonoBehaviour
{
    [Header("惡魔桌寵與頁面設定")]
    [Tooltip("惡魔桌寵按鈕")] public GameObject demonPet;
    [Tooltip("惡魔頁面（可透過點擊惡魔桌寵開啟）")] public GameObject demonUI;
    [Tooltip("退出惡魔頁面的按鈕")] public Button byeButton;
    //-----------------------------------------------------------------//
    [Header("畫面切換設定")]
    [Tooltip("用來控制視窗的圖層順序")] public WindowManager windowManager;
    [Tooltip("可切換電腦桌面和惡魔頁面配色的腳本")] public ChangeTheme changeTheme;


    void Start()
    {
        demonUI.SetActive(false); // 初始時隱藏惡魔頁面
        byeButton.onClick.AddListener(OnByeButtonClick); // 設置退出按鈕點擊事件
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 點擊惡魔桌寵可開啟惡魔頁面
            if (IsPointerOver3DObject(demonPet.transform))
            {
                OnDemonButtonClick();
            }

            // 點擊惡魔頁面可將其置前
            RectTransform rt = demonUI.GetComponent<RectTransform>();
            if (IsPointerOverUIObject(rt))
            {
                windowManager.BringToFront(rt);
            }
        }
    }

    public void OnDemonButtonClick()
    {
        Debug.Log("點擊了惡魔桌寵");
        demonPet.SetActive(false); // 隱藏惡魔桌寵
        changeTheme.SetDemonTheme(); // 切換到惡魔 UI 主題

        demonUI.SetActive(true); // 顯示惡魔頁面
        windowManager.BringToFront(demonUI.GetComponent<RectTransform>()); // 將惡魔頁面置於最上層
    }

    public void OnByeButtonClick()
    {
        Debug.Log("對惡魔說：沒事");
        demonPet.SetActive(true); // 顯示惡魔桌寵
        changeTheme.SetDesktopTheme(); // 換回桌面 UI 主題
        demonUI.SetActive(false); // 隱藏惡魔頁面
    }

    private bool IsPointerOverUIObject(RectTransform uiElement) // 檢查特定 UI 元件是否被滑鼠點擊
    {
        PointerEventData pointerData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.transform == uiElement ||
                result.gameObject.transform.IsChildOf(uiElement))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsPointerOver3DObject(Transform target) // 檢查特定場景物件是否被滑鼠點擊
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.collider.transform;

            // 如果命中物件是目標或目標的子物件
            if (hitTransform == target || hitTransform.IsChildOf(target)) return true;
        }

        return false;
    }
}
