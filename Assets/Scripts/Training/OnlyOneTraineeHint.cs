using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 掛在第一天防止玩家變動訓練角色的提示物件上 */
public class OnlyOneTraineeHint : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button okayButton; // 按下後會關閉提示
    [SerializeField] private AudioClip cancelSound; // 按下按鈕的音效

    void Start()
    {
        okayButton.onClick.AddListener(OnOkayButtonClicked);

        // 可以先設定好，不過只有第一天才會用到
        IdolWho whoCannotTrain = CheckWhoCannotTrain();
        if (whoCannotTrain != IdolWho.none)
        {
            SetHintText(TeamDataUtility.GetIdolNameTW(whoCannotTrain));
        }
    }

    private void SetHintText(string traineeName)
    {
        titleText.text = $"今天就先讓{traineeName}\n去訓練吧";
    }

    private void OnOkayButtonClicked()
    {
        Debug.Log("那我就不打擾她了");
        if (cancelSound != null) AudioManager.Instance.PlaySFX(cancelSound);
        UIAndPlayerInput.EnableAllPlayerInputs();
        gameObject.SetActive(false); // 隱藏提示物件
    }

    private IdolWho CheckWhoCannotTrain()
    {
        IdolInstance whoGoesToTeain = null;
        // 根據優先順序選擇偶像：Sirius > Aicor > Kuma
        foreach (var idol in TeamDataUtility.IdolInstanceList)
        {
            if (idol.idolIndex == IdolWho.Sirius)
            {
                whoGoesToTeain = idol;
                break;
            }
            else if (idol.idolIndex == IdolWho.Aicor && whoGoesToTeain?.idolIndex != IdolWho.Sirius)
            {
                whoGoesToTeain = idol;
            }
            else if (idol.idolIndex == IdolWho.Kuma && whoGoesToTeain?.idolIndex != IdolWho.Sirius && whoGoesToTeain?.idolIndex != IdolWho.Aicor)
            {
                whoGoesToTeain = idol;
            }
        }
        return whoGoesToTeain != null ? whoGoesToTeain.idolIndex : IdolWho.none;
    }
}