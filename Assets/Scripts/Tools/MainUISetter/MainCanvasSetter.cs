using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasSetter : MonoBehaviour
{
    [Header("主世界的介面")]
    public Canvas mainWorldCanvas;
    public List<GameObject> CharacterStatusUIs;
    public GameObject resourceUI;
    void Start()
    {
        for (int i = 0; i < CharacterStatusUIs.Count; i++)
        {
            SetStatusBar statusBar = CharacterStatusUIs[i].GetComponent<SetStatusBar>();
            statusBar.setIdolInstance(i);
        }
        setResourceUI();
        setStatusBar();
    }
    public void setStatusBar()
    {
        for (int i = 0; i < CharacterStatusUIs.Count; i++)
        {
            SetStatusBar statusBar = CharacterStatusUIs[i].GetComponent<SetStatusBar>();
            statusBar.setByIdolInstance();
        }
    }
    public void setResourceUI()
    {
        ResourceStatusSetter resourceStatusSetter = resourceUI.GetComponent<ResourceStatusSetter>();
        resourceStatusSetter.setByResourceAndDay();
    }
}
