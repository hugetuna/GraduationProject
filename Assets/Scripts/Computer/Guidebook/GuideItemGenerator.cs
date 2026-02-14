using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 放在圖鑑視窗根部以生成不同分類 ScrollView 中的粉絲卡片 */
public class GuideItemGenerator : MonoBehaviour
{
    [Header("粉絲資料")]
    public List<FansItem> fansList = new(); // 儲存粉絲資訊的清單
    //-----------------------------------------------------------------//
    public GameObject fansPrefab; // 用於生成粉絲卡片的預製件
    public Transform fansContent; // 用於放置生成的粉絲物件的容器
    public GuideInfoUI guideInfoUI; // 用於顯示粉絲詳細資訊的腳本

    void Start()
    {
        // 生成粉絲卡片
        foreach (FansItem fans in fansList) // 按清單生成初始的粉絲卡片
        {
            GameObject fansObject = Instantiate(fansPrefab, fansContent);
            if (fansObject == null)
            {
                Debug.Log("粉絲卡片生成失敗！");
                continue;
            }

            GameObject btn = fansObject.transform.Find("Button").gameObject; // Wrapper + "Button"
            // 設定粉絲卡片的 UI 資料
            btn.GetComponent<SetGuideItemUI>().SetFansItem(fans);
            guideInfoUI.AddToFansButtons(btn.GetComponent<Button>()); 
        }
        guideInfoUI.UpdateCollectNumber();  // 設定粉絲卡片點擊事件＆更新顯示的收集數量
    }
}
