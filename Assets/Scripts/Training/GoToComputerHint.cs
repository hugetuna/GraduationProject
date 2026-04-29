using UnityEngine;
using UnityEngine.UI;
using System;

/* 掛在詢問要不要跳到電腦介面的提示 prefab 上 */
public class GoToComputerHint : MonoBehaviour
{
    [SerializeField] private Button yesButton; // 按下後會跳轉到電腦介面
    [SerializeField] private Button noButton; // 按下後會關閉提示
    public static event Action<TrainingUIData> OnGoToComputer; // 確定前往電腦介面事件（須先進行訓練指派）
    private TrainingUIData trainingUIData; // 可能會用到的訓練 UI 資料
    //-----------------------------------------------------------------//
    [SerializeField] private AudioClip cancelSound; // 按下"否"按鈕的音效
    [SerializeField] private AudioClip assignSound; // 按下"是"按鈕的音效

    void Start()
    {
        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);
    }

    public void SetTrainingUIData(TrainingUIData data)
    {
        trainingUIData = data;
    }

    private void OnYesButtonClicked()
    {
        Debug.Log("好的，跳轉到電腦介面");

        if (trainingUIData == null)
        {
            Debug.LogWarning("尚未 SetTrainingUIData，跳轉失敗");
            return;
        }

        if (assignSound != null) AudioManager.Instance.PlaySFX(assignSound);
        OnGoToComputer?.Invoke(trainingUIData); // 記得先記下全員的訓練資料

        // 實際跳轉
        SceneTransitionManager.Instance.triggerComputerAfterLoad = true; // 跳轉後即開啟電腦
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.teleportByTargetSceneName("Floor_4");
        }
    }

    private void OnNoButtonClicked()
    {
        Debug.Log("先不要，關閉提示");
        if (cancelSound != null) AudioManager.Instance.PlaySFX(cancelSound);
        Destroy(gameObject, 0.1f); // 延遲 0.1 秒後銷毀提示物件
    }
}