using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public GameObject panelToClose;

    public void CloseThisPanel()
    {
        // 可使用 UI 上的叉叉關閉 UI
        Debug.Log("關閉 UI");
        panelToClose.SetActive(false);
    }
}
