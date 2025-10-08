using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
public class CameraFollow : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;
    public TeamManager teamManager;
    
    void Update()
    {
        if (teamManager != null && cinemachineCamera != null)
        {
            cinemachineCamera.Follow = teamManager.teamMembers[teamManager.currentLeaderIndex].transform;
        }
    }
}
