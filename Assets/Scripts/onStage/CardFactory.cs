using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    // 複製一張卡片的實例
    public static ActionCard CreateCardInstance(ActionCard original)
    {
        ActionCard instance = ScriptableObject.Instantiate(original);

        // 將 effects 深複製並指定 parentCard
        instance.effects = new List<CardEffectBase>();
        foreach (var effect in original.effects)
        {
            //替每一個效果創建一個複製體，並將其主體卡片設置為新生產的卡片
            //隨後再將該效果塞進卡片中
            var copied = ScriptableObject.Instantiate(effect);
            copied.parentCard = instance;
            instance.effects.Add(copied);
        }

        return instance;
    }
}
