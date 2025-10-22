using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SetInfoCard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    //public TextMeshProUGUI durationText;
    //public TextMeshProUGUI vigorCostText;
    public TextMeshProUGUI voGateText;
    public TextMeshProUGUI daGateText;
    public TextMeshProUGUI viGateText;
    public void SetInfo(ActionCard infoToSet)
    {
        nameText.text = infoToSet.cardName;
        descriptionText.text = infoToSet.effectString;
        //if (durationText != null) durationText.text = infoToSet.applyDuration.ToString();
        //if (vigorCostText != null) vigorCostText.text = infoToSet.staminaCost.ToString();
        voGateText.text = infoToSet.voGate.ToString();
        daGateText.text = infoToSet.daGate.ToString();
        viGateText.text = infoToSet.viGate.ToString();
    }
    public void ClearInfo()
    {
        nameText.text = "";
        descriptionText.text = "";
        //durationText.text = "";
        //vigorCostText.text = "";
        voGateText.text = "";
        daGateText.text = "";
        viGateText.text = "";
    }
}
