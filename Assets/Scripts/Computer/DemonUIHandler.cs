using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/* 掛在 UIManager 等惡魔桌寵以外的物件上 */
public class DemonUIHandler : MonoBehaviour
{
    [Header("惡魔桌寵與頁面設定")]
    [SerializeField] private GameObject demonPet; // 惡魔桌寵按鈕
    private Transform demonPetTransform;
    [SerializeField] private GameObject demonUI; // 惡魔頁面（可透過點擊惡魔桌寵開啟）
    [SerializeField] private Button byeButton; // 退出惡魔頁面的按鈕【沒事】
    [SerializeField] private AudioClip clickDemonSound; // 點擊惡魔桌寵的音效

    void Awake()
    {
        demonPetTransform = demonPet.GetComponent<Transform>();
    }

    void Start()
    {
        demonUI.SetActive(false); // 初始時隱藏惡魔頁面
        byeButton.onClick.AddListener(OnByeButtonClick); // 設置退出按鈕點擊事件
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 點擊惡魔桌寵可開啟惡魔頁面（前面沒點到 UI 才會做檢查）
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (demonPet.activeSelf && IsPointerOver3DObject(demonPetTransform))
                {
                    OnDemonButtonClick();
                }
            }
        }
    }

    public void OnDemonButtonClick()
    {
        Debug.Log("點擊了惡魔桌寵");
        // demonPet.SetActive(false); // 隱藏惡魔桌寵
        demonUI.SetActive(true); // 顯示惡魔頁面（預設就在圖層最前面）
        AudioManager.Instance.PlaySFX(clickDemonSound); // 播放音效
    }

    public void OnByeButtonClick()
    {
        Debug.Log("退出惡魔頁面");
        // demonPet.SetActive(true); // 顯示惡魔桌寵
        demonUI.SetActive(false); // 隱藏惡魔頁面
    }

    private bool IsPointerOver3DObject(Transform target) // 檢查特定場景物件是否被滑鼠點擊
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 排除特定 Layer
        int mask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mask))
        {
            return hit.transform == target || hit.transform.IsChildOf(target);
        }

        return false;
    }
}
