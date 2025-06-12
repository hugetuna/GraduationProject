using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemAndWeight
{
    public Item item;
    public int weight;
}
[CreateAssetMenu(menuName = "ItemEffects/GainItem")]
public class GainItemEffect : ItemEffect
{
    public List<ItemAndWeight> PossibleItems;
    public int weightLimit;
    public override void Apply(IdolInstance target, ResourceManager manager)
    {
        List<Item> resultItems = new List<Item>();
        int totalWeight;
        for (totalWeight = 0; totalWeight < weightLimit;)
        {
            int index = Random.Range(0, PossibleItems.Count);
            totalWeight += PossibleItems[index].weight;
            resultItems.Add(PossibleItems[index].item);
        }
        manager.AddItem(resultItems);
    }
}
