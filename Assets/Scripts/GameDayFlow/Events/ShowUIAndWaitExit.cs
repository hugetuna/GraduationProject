using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;
using UnityEngine.UI;

public class ShowUIAndWaitExit : MonoBehaviour
{
    private System.Action onFinish;
    public Button closeButton;
    public TeamManager teamManager;
    public void StartEvent(System.Action finish)
    {
        onFinish = finish;
        closeButton.onClick.AddListener(() => 
        {
            FinishEvent();
        });
        
        teamManager = FindAnyObjectByType<TeamManager>();
        teamManager.teamMembers[
            teamManager.currentLeaderIndex].enabled = false;
        enabled = true;
    }
    public void FinishEvent()
    {
        teamManager.teamMembers[
            teamManager.currentLeaderIndex].enabled = true;
        enabled = false;
        onFinish?.Invoke();
    }
}
