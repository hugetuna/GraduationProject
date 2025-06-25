using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardFactory
{
    // 複製一張卡片的實例
    public static ActionCard CreateCardInstance(ActionCard originalCard)
    {
        if (originalCard == null)
        {
            Debug.LogError("CardFactory：你傳入了空的原始卡片！");
            return null;
        }

        // ScriptableObject.Instantiate 會複製出一份新的執行期實例
        return ScriptableObject.Instantiate(originalCard);
    }
}
