using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ClothChange : MonoBehaviour,IInteractable
{
    public string InteractionKey => "ClothChange"; // 這個字串用來指定動畫 key
    public TeamManager teamManager;
    public Dictionary<string, SpriteResolver> resolvers = new Dictionary<string, SpriteResolver>();
    public void Interact(int tool)
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        PlayerControlMainWorld leader = teamManager.teamMembers[teamManager.currentLeaderIndex];
        IdolInstance idol = leader.GetComponent<IdolInstance>();
        idol.ChangeCloth(1);
    }
}
