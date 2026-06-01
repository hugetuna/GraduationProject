using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

/* 掛在 RestManager 底下 */
public class RestUIHandler : MonoBehaviour
{
    public static event Action<bool> OnRestUIConfirmed; // 定義休息室 UI 確認事件
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

    void Start()
    {
        LoungeInteraction.OnLoungeInteracted += ShowRestUI; // 訂閱開啟休息室 UI 的事件

        closeButton.onClick.AddListener(ConfirmToAssign); // 設定關閉按鈕的監聽事件

        restUI.SetActive(false); // 預設關閉休息室 UI
    }

    void OnDestroy()
    {
        LoungeInteraction.OnLoungeInteracted -= ShowRestUI; // 取消訂閱事件
    }

    public void ShowRestUI()
    {
        restUI.SetActive(true);

        Debug.Log("開啟休息室 UI");
        AudioManager.Instance.PlaySFX(openSound);

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
        // if (TrainingUIManager.Instance.GetMembers().Count == 0)
        // {
        //     // 若全員皆去訓練，觸發可通往電腦場景的 UI
        //     var hintObj = Instantiate(hintPrefab, trainingUI.transform.parent); // 在 TrainingUI 的父物件下生成提示 UI
        //     hintObj.transform.SetAsLastSibling(); // 確保提示 UI 在最上層
        //     hintObj.GetComponent<GoToComputerHint>().SetTrainingUIData(trainingUIData); // 若確定前往電腦介面可先進行訓練結算
        // }
        // else
        // {
        //     // 若無人去訓練，就什麼也不做（交由 TraineeAssignment 處理）
        //     // 有任何人去訓練，即可觸發指派訓練成員事件
        //     if (assignSound != null) AudioManager.Instance.PlaySFX(assignSound);
        //     OnTrainingUIConfirmed?.Invoke(trainingUIData, false);
        //     CloseTrainingUI();
        // }

        if (assignSound != null) AudioManager.Instance.PlaySFX(assignSound);
        OnRestUIConfirmed?.Invoke(false);
        CloseRestUI();
    }
}
