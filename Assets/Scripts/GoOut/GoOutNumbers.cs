using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;


/* 掛在角色 UI 上 */
public class GoOutNumbers : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI nameText; // 角色名稱
    [SerializeField] private TextMeshProUGUI performanceText; // 角色演技數值
    [SerializeField] private TextMeshProUGUI danceText; // 角色舞蹈數值
    [SerializeField] private TextMeshProUGUI vocalText; // 角色歌唱數值
    [SerializeField] private TextMeshProUGUI visualText; // 角色表現力數值
    [SerializeField] private List<BuffBar> buffBars; // 角色的增益效果顯示欄位
    //-----------------------------------------------------------------//
    private IdolInstance characterInfo; // 角色資料

    public void Initialize(IdolWho idolIndex)
    {
        characterInfo = TeamDataUtility.IdolDict[idolIndex]; // 尋找對應的角色資料
    }

    public void OnPointerEnter(PointerEventData eventData) // 滑鼠移入時顯示角色資料
    {
        nameText.text = TeamDataUtility.GetIdolNameTW(characterInfo.idolIndex);
        performanceText.text = $"{characterInfo.performance}";
        danceText.text = $"{characterInfo.dance}";
        vocalText.text = $"{characterInfo.vocal}";
        visualText.text = $"{characterInfo.visual}";

        var effects = ItemEffectUtility.GetGlobalEffectDisplayNames();
        for (int i = 0; i < buffBars.Count; i++)
        {
            if (i < effects.Count)
            {
                buffBars[i].gameObject.SetActive(true);
                buffBars[i].UpdateBuffBar(effects[i]);
            }
            else
            {
                buffBars[i].gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) // 滑鼠移開時清除資料顯示
    {
        nameText.text = "";
        performanceText.text = "";
        danceText.text = "";
        vocalText.text = "";
        visualText.text = "";
        foreach (var buffBar in buffBars)
        {
            buffBar.gameObject.SetActive(false);
        }
    }
}
