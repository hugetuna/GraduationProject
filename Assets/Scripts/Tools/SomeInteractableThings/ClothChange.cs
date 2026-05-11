using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ClothChange : MonoBehaviour,IInteractable
{
    public string InteractionKey => "ClothChange"; // 這個字串用來指定動畫 key
    public TeamManager teamManager;
    public Dictionary<string, SpriteResolver> resolvers = new Dictionary<string, SpriteResolver>();
    public bool isUsed = false;
    public void Interact(int tool)
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        PlayerControlMainWorld leader = teamManager.teamMembers[teamManager.currentLeaderIndex];
        IdolInstance idol = leader.GetComponent<IdolInstance>();
        if(!isUsed)
        {
            idol.ChangeCloth(1);
            isUsed=!isUsed;
        }
        else
        {
            idol.ChangeCloth(0);
            isUsed=!isUsed;
        }
    }
}
