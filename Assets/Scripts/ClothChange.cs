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
        Transform leaderTransform = leader?.transform.Find("KumaQ2.0");
        if (leaderTransform == null) {
            leaderTransform= leader?.transform.Find("KaroQ2.0");
        }
        if (leaderTransform == null)
        {
            leaderTransform= leader?.transform.Find("SiriusQ2.0");
        }
        resolvers = new Dictionary<string, SpriteResolver>()
    {
        { "Body", leaderTransform.Find("Body").GetComponent<SpriteResolver>() },
        { "LHand", leaderTransform.Find("LHand").GetComponent<SpriteResolver>() },
        { "RHand", leaderTransform.Find("RHand").GetComponent<SpriteResolver>() },
        { "LLeg", leaderTransform.Find("LLeg").GetComponent<SpriteResolver>() },
        { "RLeg", leaderTransform.Find("RLeg").GetComponent<SpriteResolver>() },
        { "OnHead", leaderTransform.Find("OnHead").GetComponent<SpriteResolver>() },
    };
        foreach (var resolver in resolvers)
        {
            resolver.Value.SetCategoryAndLabel(resolver.Key, "clo1");
        }
    }
}
