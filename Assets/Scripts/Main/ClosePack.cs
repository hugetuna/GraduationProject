using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* 掛在背包頁面的關閉按鈕上 */
public class ClosePack : MonoBehaviour
{
    public GameObject packToClose;
    public Button closeButton;

    void Start()
    {
        closeButton.onClick.AddListener(CloseThePack);
    }

    public void CloseThePack() // 使用 UI 上的叉叉關閉 UI
    {
        Debug.Log("關閉 UI");
        // ResetPackUI();
        packToClose.SetActive(false);
    }

    private void ResetPackUI()
    {
        // 重置背包內的道具資訊顯示
        ItemInfoUI itemInfoUI = packToClose.GetComponentInChildren<ItemInfoUI>();
        UseItem useItem = packToClose.GetComponentInChildren<UseItem>();
        ChangeTypeUI changeTypeUI = packToClose.GetComponentInChildren<ChangeTypeUI>();
        if (itemInfoUI != null && useItem != null)
        {
            itemInfoUI.ResetItemInfo();
            useItem.ResetDropdown();
            changeTypeUI.ResetTypeUI();
        }
        else
        {
            Debug.Log("背包 UI 重置失敗");
        }
    }
}
