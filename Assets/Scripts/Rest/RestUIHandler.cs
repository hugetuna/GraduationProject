using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/* 掛在 RestManager 底下 */
public class RestUIHandler : MonoBehaviour
{
    public static event Action<bool> OnRestConfirmed; // 定義休息室 UI 確認事件
    //-----------------------------------------------------------------//
    [Header("休息室 UI 元素")]
    [SerializeField] private GameObject restUI; // 直接使用場景中的，不必另外生成
    //-----------------------------------------------------------------//
    // [SerializeField] private Button panelBackground; // 點擊背景關閉 UI 的按鈕
    [SerializeField] private Button closeButton; // 關閉 UI 的叉叉按鈕
    [SerializeField] private List<Image> characterImages = new(); //  UI 上的（角色）圖片插槽
    [SerializeField] private List<RestDropZone> memberDropZones = new();
    [SerializeField] private List<RestDropZone> restDropZones = new();
    //-----------------------------------------------------------------//
    [Header("相關音效")]
    [SerializeField] private AudioClip openSound; // 開啟訓練 UI 的音效
    [SerializeField] private AudioClip assignSound; // 按下指派按鈕的音效
    [SerializeField] private AudioClip cancelSound; // 按下"否"按鈕的音效
    //-----------------------------------------------------------------//
    [Header("提示視窗")]
    [SerializeField] private GameObject hintObj; // 前往電腦的提示物件
    [SerializeField] private Button yesBtn; // 提示的 "是" 按鈕
    [SerializeField] private Button noBtn; // 提示的 "否" 按鈕

    void Start()
    {
        RestInteraction.OnRestInteracted += ShowRestUI; // 訂閱開啟休息室 UI 的事件

        closeButton.onClick.AddListener(ConfirmToAssign); // 設定關閉按鈕的監聽事件
        yesBtn.onClick.AddListener(JumpToComputer);
        noBtn.onClick.AddListener(CloseHint);

        restUI.SetActive(false); // 預設關閉休息室 UI
        hintObj.SetActive(false); // 預設關閉提示物件
    }

    void OnDestroy()
    {
        RestInteraction.OnRestInteracted -= ShowRestUI; // 取消訂閱事件
    }

    public void ShowRestUI()
    {
        restUI.SetActive(true);

        Debug.Log("開啟休息室 UI");
        AudioManager.Instance.PlaySFX(openSound);
        UIAndPlayerInput.DisableAllPlayerInputs();

        //-----------------------------------------------------------------//

        UpdateCharacterImagesAndPositions(); // 設定角色 UI 圖片及位置

        RefreshDragSystem(); // 初始化或刷新拖曳系統，確保每次開啟 UI 都能正確顯示拖曳功能

        //-----------------------------------------------------------------//

        // 休息室新手教學提示
        // var currentEvent = DayManager.Instance.dayEventManager.currentEvent;
        // if (DayManager.Instance.chapter == 0 && DayManager.Instance.date == 1 && currentEvent != null && currentEvent.TriggerTimeIndex >= 6)
        // {
        //     OnRestUIOpened?.Invoke();
        // }
    }

    private void UpdateCharacterImagesAndPositions()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            Image img = characterImages[i];
            IdolInstance idol = TeamDataUtility.IdolInstanceList[i];
            RestRecord restRecord = idol.restRecord;

            // 為圖片插槽放置角色圖片
            if (i < TeamDataUtility.idolCount)
            {
                img.sprite = idol.idolUISprites.spriteQ;
            }
            else
            {
                // 超出角色範圍就設為空，避免錯誤
                img.sprite = null;
                continue;
            }

            // 根據角色的打工紀錄決定是否顯示圖片（在打工中或在隊伍中的角色才顯示）
            bool isActive = idol.CanShowInTheAction(AvailableAction.Rest);
            img.gameObject.SetActive(isActive);

            // 還原上次圖片位置，如果沒有就初始化
            if (restRecord.position != Vector2.zero)
            {
                img.rectTransform.anchoredPosition = restRecord.position;
            }
            else
            {
                restRecord.position = img.rectTransform.anchoredPosition;
            }
        }
    }

    private void CloseRestUI()
    {
        Debug.Log("關閉休息室 UI");
        restUI.SetActive(false);
        UIAndPlayerInput.EnableAllPlayerInputs();

        // OnRestUIClosed?.Invoke(); // 觸發休息室 UI 關閉事件
    }

    private void RefreshDragSystem()
    {
        for (int i = 0; i < characterImages.Count; i++)
        {
            var img = characterImages[i];
            var dtl = img.GetComponentInChildren<DragToRest>();
            var vb = img.GetComponentInChildren<RestVigourBar>();

            var idol = TeamDataUtility.IdolInstanceList[i];
            var restRecord = idol.restRecord;

            if (restRecord.zoneIndex == -1) // 只要有一人是 -1，就代表全員尚未初始化
            {
                restRecord.zoneIndex = i; // 預設分配到對應的圖片位置
                // 其他預設值就不特別碰了
            }

            // 正式還原＆分配位置資訊
            RestDropZone characterDropZone;
            if (restRecord.zoneType == RestDropZoneType.Rest)
            {
                characterDropZone = restDropZones.FirstOrDefault(zone => zone.zoneIndex == restRecord.zoneIndex);
            }
            else
            {
                characterDropZone = memberDropZones.FirstOrDefault(zone => zone.zoneIndex == restRecord.zoneIndex);
            }

            dtl.Initialize(idol.idolIndex, characterDropZone); // 初始化每個角色的拖曳功能
            vb.Initialize(idol.idolIndex); // 初始化每個角色的體力條
        }
    }

    private void ConfirmToAssign()
    {
        Debug.Log("指派休息");
        if (CheckAreAllGone())
        {
            // 若全員皆離開隊伍，觸發可通往電腦的提示 UI
            hintObj.SetActive(true);
        }
        else
        {
            // 正常處理指派事件
            AudioManager.Instance.PlaySFX(assignSound);
            OnRestConfirmed?.Invoke(false); // 拖曳時角色就會記錄休息，所以這裡不用再另外傳遞
            CloseRestUI();
        }
    }

    private bool CheckAreAllGone()
    {
        int goneIdolCount = 0;
        foreach (var idol in TeamDataUtility.IdolInstanceList)
        {
            if (!idol.CanShowInTheAction(AvailableAction.Rest))
            {
                goneIdolCount++;
            }
            else
            {
                if (idol.restRecord.zoneType == RestDropZoneType.Rest)
                {
                    goneIdolCount++;
                }
            }
        }
        return goneIdolCount == TeamDataUtility.idolCount; // 全員都不在隊伍裡才會回傳 true
    }

    private void CloseHint()
    {
        AudioManager.Instance.PlaySFX(cancelSound);
        hintObj.SetActive(false);
    }

    private void JumpToComputer()
    {
        Debug.Log("進入電腦介面");
        AudioManager.Instance.PlaySFX(assignSound);

        // 把該指派的都派一派
        OnRestConfirmed?.Invoke(true);

        // 前往電腦介面
        SceneTransitionManager.Instance.triggerComputerAfterLoad = true;
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.teleportByTargetSceneName("Floor_3");
        }
    }
}
