using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events; // 引入此命名空間，用於 UnityAction

public enum TutorialState
{
    Inactive, // 未啟動
    Running,  // 正在運行中
    Paused,   // 暫停，等待玩家手動恢復
    Finished  // 已完成
}
public class TutorialStageManager : MonoBehaviour
{
    // 當前教學狀態
    public TutorialState currentState = TutorialState.Inactive;
    // 步驟的容器，每個步驟可能是一個包含 UI 和邏輯的 GameObject
    public List<GameObject> tutorialSteps;
    // 遮罩控制器
    public TutorialMaskController mask;
    public OnStageManager onStageManager;

    [SerializeField]
    private int currentStepIndex = 0;

    void Start()
    {
        // 從第一個步驟開始
        currentStepIndex = 0;
        StartCoroutine(TutorialFlow());
    }

    // 核心流程控制協程：負責連續執行每個教學步驟
    private IEnumerator TutorialFlow()
    {
        currentState = TutorialState.Running;

        while (currentStepIndex < tutorialSteps.Count)
        {
            // 檢查是否處於暫停狀態
            if (currentState == TutorialState.Paused)
            {
                // 暫停協程，等待一幀後再次檢查狀態
                yield return null;
                continue; // 跳過本次迴圈的剩餘部分，再次檢查狀態
            }
            // 執行當前步驟的邏輯...
            Button targetButton = ShowTutorialByStep(currentStepIndex);
            if (targetButton != null)
            {
                yield return StartCoroutine(WaitPlayerClick(targetButton));
            }

            currentStepIndex++;
        }
        // 教學結束
        currentState = TutorialState.Finished;
        foreach (var step in tutorialSteps)
        {
            if (step != null) step.SetActive(false);
        }
        onStageManager.ResumeGame();
        Debug.Log("教學流程結束！");
    }

    // 調整為返回當前步驟需要監聽的按鈕
    public Button ShowTutorialByStep(int stepIndex)
    {
        // 隱藏所有步驟
        foreach (var step in tutorialSteps)
        {
            if (step != null) step.SetActive(false);
        }

        // 檢查索引是否有效
        if (stepIndex >= 0 && stepIndex < tutorialSteps.Count && tutorialSteps[stepIndex] != null)
        {
            GameObject currentStep = tutorialSteps[stepIndex];
            currentStep.SetActive(true);

            // 執行步驟邏輯
            return RunStep(stepIndex, currentStep);
        }
        return null; // 無效的步驟
    }

    // 修訂後的 WaitPlayerClick，確保移除 Listener
    IEnumerator WaitPlayerClick(Button btn)
    {
        // 確保按鈕存在
        if (btn == null) yield break;

        bool clicked = false;
        UnityAction listener = null; // 使用 UnityAction 來確保我們可以移除正確的引用

        listener = () =>
        {
            clicked = true;
            // 一旦點擊，立即移除 Listener
            btn.onClick.RemoveListener(listener);
        };

        btn.onClick.AddListener(listener);

        yield return new WaitUntil(() => clicked);

        // 協程結束時再次確保移除（安全措施）
        btn.onClick.RemoveListener(listener);
    }
    // 用於在特定步驟暫停教學（例如玩家點擊 "離開教學" 按鈕）
    public void PauseTutorial()
    {
        if (currentState == TutorialState.Running)
        {
            currentState = TutorialState.Paused;

            // 隱藏當前教學步驟 UI
            if (currentStepIndex < tutorialSteps.Count && tutorialSteps[currentStepIndex] != null)
            {
                tutorialSteps[currentStepIndex].SetActive(false);
            }

            Debug.Log("教學已暫停，進度：" + currentStepIndex);
        }
    }

    // 用於恢復教學（例如玩家點擊 "繼續教學" 按鈕）
    public void ResumeTutorial()
    {
        if (currentState == TutorialState.Paused)
        {
            currentState = TutorialState.Running;

            // 由於 TutorialFlow 協程一直在運行（只是被 null 延遲），
            // 只要狀態變為 Running，它就會在下一幀繼續執行當前的步驟。

            Debug.Log("教學已恢復，將繼續步驟：" + currentStepIndex);

            // 注意：我們不需要重新調用 ShowTutorialByStep，因為協程會自己處理。
            // 但如果您的遊戲邏輯需要，您也可以在此處重新啟用教學 UI：
            // ShowTutorialByStep(currentStepIndex);
        }
    }
    // 調整 RunStep 返回需要等待點擊的 Button
    public Button RunStep(int step, GameObject currentStep)
    {
        Button stepButton = null;

        // 嘗試從當前步驟中找到名為 "Button" 的元件
        Transform buttonTransform = currentStep.transform.Find("Button");
        if (buttonTransform != null)
        {
            stepButton = buttonTransform.GetComponent<Button>();
        }

        if (step == 0)
        {
            // Step 0: 點擊後推進。遮罩定位在 Button。
            mask.Focus(new Vector2(0.415f, 0.5f), new Vector2(0.085f, 0.165f));
            // 返回要監聽的按鈕
            return stepButton;
        }
        else if (step == 1)
        {
            // Step 1: 點擊後推進。遮罩定位在另一處，且在該外部按鈕暫停教學。
            mask.Focus(new Vector2(0.643f, 0.38f), new Vector2(0.04f, 0.05f));
            return stepButton;
        }
        else if (step == 2)
        {
            // Step 2: 重啟教學，遮罩定位在計分板。
            onStageManager.DrawCards(3);
            mask.Focus(new Vector2(0.5f, 0.625f), new Vector2(0.045f, 0.05f));
            return stepButton;
        }
        else if (step == 3)
        {
            // Step 3: 計分板
            onStageManager.PauseGame();
            mask.Focus(new Vector2(0.5f, 0.625f), new Vector2(0.045f, 0.05f));
            return stepButton;
        }
        else if (step == 4)
        {
            // Step 4: 計分板
            mask.Focus(new Vector2(0.5f, 0.625f), new Vector2(0.045f, 0.05f));
            
            return stepButton;
        }
        else if (step == 5)
        {
            // Step 5:回合推進條
            mask.Focus(new Vector2(0.45f, 0.3f), new Vector2(0.16f, 0.03f));
            return stepButton;
        }
        else if (step == 6)
        {
            // Step 6:手牌區
            mask.Focus(new Vector2(0.51f, 0.35f), new Vector2(0.1f, 0.05f));
            return stepButton;
        }
        else if (step == 7)
        {
            // Step 7:門檻版
            mask.Focus(new Vector2(0.38f, 0.4f), new Vector2(0.04f, 0.08f));
            return stepButton;
        }
        else if (step == 8)
        {
            // Step 8:門檻版
            mask.Focus(new Vector2(0.38f, 0.4f), new Vector2(0.04f, 0.08f));
            return stepButton;
        }
        else if (step == 9)
        {
            // Step 9:垃圾桶
            mask.Focus(new Vector2(0.332f, 0.365f), new Vector2(0.015f, 0.03f));
            return stepButton;
        }
        else if (step == 10)
        {
            // Step 10:抽卡區
            mask.Focus(new Vector2(0.655f, 0.38f), new Vector2(0.03f, 0.07f));
            return stepButton;
        }
        else if (step == 11)
        {
            // Step 11:休息與準備
            mask.Focus(new Vector2(0.665f, 0.47f), new Vector2(0.02f, 0.035f));
            return stepButton;
        }
        return null; // 該步驟不需要等待按鈕點擊
    }
}
