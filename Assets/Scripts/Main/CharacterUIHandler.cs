using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject CharacterUI;
    [SerializeField] private Button CharacterButton;

    void Start()
    {
        CharacterUI.SetActive(false);
        CharacterButton.onClick.AddListener(OpenCharacterUI); // 設置按鈕點擊事件
    }


    void Update()
    {
        if (CharacterUI.activeSelf && Input.GetMouseButtonDown(0)) // 檢查滑鼠左鍵是否被按下
        {
            if (!IsCursorClickUIObject()) // 點擊非 UI 區域時關閉 UI
            {
                Debug.Log("關閉角色 UI");
                CharacterUI.SetActive(false);
            }
        }

        // if (!CharacterUI.activeSelf)
        // {
        //     CharacterButton.interactable = true; // 啟用角色按鈕
        // }
        // else
        // {
        //     CharacterButton.interactable = false; // 禁用角色按鈕
        // }
    }

    private void OpenCharacterUI()
    {
        if (!CharacterUI.activeSelf) // 如果角色 UI 未開啟，則打開它
        {
            CharacterUI.SetActive(true);
        }
    }

    private bool IsCursorClickUIObject()
    {
        // 根據當前操作，設定滑鼠或觸控位置
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // RaycastAll 會從 eventData 中的滑鼠位置發射一條射線，檢測所有碰撞的 UI 元素
        // 符合條件的 UI 元素會被加到 raycastResults 清單中
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        return raycastResults.Count > 0;
    }
}
