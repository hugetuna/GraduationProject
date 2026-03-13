using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpcialCard
{
    none,
    SP102,
}
[CreateAssetMenu(menuName = "Stage/CardEffects/PointAmountUp")]
public class PointAmountUpEffect : CardEffectBase
{
    public int PointAmount;//加分量
    public bool selfPointUp = true;//自己加分，預設值
    public bool allPointUp = false;//全體加分
    public bool spesificPointUp = false;//特定卡片加分
    public string spesificCardId;//特定卡片編號
    public SpcialCard spcialCard;//特殊卡片類型
    public override void OnApply(IdolOnStage target, OnStageManager onStageManager)
    {
        //Debug.Log("開始演出動作...");
        //獲取加分量

        // 動作開始時播放動畫或特效
    }

    public override void OnEnd(IdolOnStage target, OnStageManager onStageManager)
    {
        if (selfPointUp)
        {
            if (parentCard != null)
            {
                parentCard.point += PointAmount;
            }
        }
        if (allPointUp)//所有卡牌的分數上升量增加
        {
            foreach (var deckCard in onStageManager.deck)
            {
                if (deckCard != null)
                {
                    deckCard.point += PointAmount;
                }
            }
            foreach (var GraveCard in onStageManager.Grave)
            {
                if (GraveCard != null)
                {
                    GraveCard.point += PointAmount;
                }
            }
            foreach (var handsCard in onStageManager.hands)
            {
                if (handsCard != null)
                {
                    handsCard.GetComponent<SetCardUI>().cardData.point += PointAmount;
                }
            }
        }

        if (spesificPointUp)//特定卡牌的分數上升量增加
        {
            foreach (var deckCard in onStageManager.deck)
            {
                if (deckCard != null && deckCard.cardId == spesificCardId)
                {
                    deckCard.point += PointAmount;
                }
            }
            foreach (var GraveCard in onStageManager.Grave)
            {
                if (GraveCard != null && GraveCard.cardId == spesificCardId)
                {
                    GraveCard.point += PointAmount;
                }
            }
            foreach (var handsCard in onStageManager.hands)
            {
                if (handsCard != null && handsCard.GetComponent<SetCardUI>().cardData.cardId == spesificCardId)
                {
                    handsCard.GetComponent<SetCardUI>().cardData.point += PointAmount;
                }
            }
        }

        if (spcialCard == SpcialCard.SP102)//特殊卡片加分
        {
            int sp102PointUpTimes = 0;//SP102的加分幅度
            foreach (var idol in onStageManager.onStageIdols)
            {
                //檢查每個偶像使用的卡牌中是否有SP102(最大3)
                foreach (var card in idol.GetComponent<IdolOnStage>().usedCards)
                {
                    if (card != null && card.cardId == "SP102")
                    {
                        sp102PointUpTimes++;
                        break;
                    }
                }
            }
            int totalPointUp = 0;
            
            if (sp102PointUpTimes==1) totalPointUp = 200;
            else if (sp102PointUpTimes == 2) totalPointUp = 500;
            else if (sp102PointUpTimes >= 3) totalPointUp = 1000;
            parentCard.point += totalPointUp;
        }
    }
}
