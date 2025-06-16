using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PriceType {Fans=0,Money=1,Item=2 }//0.1.2
[CreateAssetMenu(fileName = "New FanItem", menuName = "Inventory/Fans")]
public class FansItem : Item
{
    public PriceType priceType=PriceType.Fans;

    public IdolWho harvester = IdolWho.none;
    public List<ItemEffect> effects;//效果列表
    public void SetPriceType(PriceType Type)
    {
        priceType = Type;
    }
    public void SetHarvester(IdolWho target)
    {
        harvester = target;
    }
    public override void Use()
    {
        ResourceManager resourceManager = FindObjectOfType<ResourceManager>();
        TeamManager teamManager = FindObjectOfType<TeamManager>();
        if (harvester == IdolWho.none)
        {
            Debug.LogError("找不到 IdolInstance！");
            return;
        }
        if (effects.Count == 3)
        {
            IdolInstance whoToApply;
            foreach(var teamMember in teamManager.teamMembers)
            {
                if(teamMember.GetComponent<IdolInstance>().idolIndex== harvester)
                {
                    whoToApply = teamMember.GetComponent<IdolInstance>();
                    effects[(int)priceType].Apply(whoToApply, resourceManager);
                }
            }
            Debug.LogError("找不到正確的 IdolInstance！");
        }
        else
        {
            Debug.LogError("效果種類不完善(需要Fans,Money,Item三種)");
        }
            
        Debug.Log($"{itemName} 被使用！");
    }
    public override Item Clone()
    {
        FansItem copy = Instantiate(this);
        copy.harvester = IdolWho.none; // 預設為 null，由外部指派
        return copy;
    }
}
