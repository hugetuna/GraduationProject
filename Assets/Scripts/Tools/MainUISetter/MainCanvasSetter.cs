using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvasSetter : MonoBehaviour
{
    [Header("主世界的介面")]
    public Canvas mainWorldCanvas;
    public List<GameObject> CharacterStatusUIs;
    //角色狀態面板
    public int characterStatusBoardUIIndex = 0;
    public List<GameObject> CharacterStatusBoardUIs;
    //資源狀態介面
    public GameObject resourceUI;
    void Start()
    {
        for (int i = 0; i < CharacterStatusUIs.Count; i++)
        {
            SetStatusBar statusBar = CharacterStatusUIs[i].GetComponent<SetStatusBar>();
            SetStatusBoard setStatusBoard = CharacterStatusBoardUIs[i].GetComponent<SetStatusBoard>();
            statusBar.setIdolInstance(i);
            setStatusBoard.setIdolInstance(i);
        }
        setResourceUI();
        setStatusBar();
        setStatusBoard();
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
    public void setStatusBoard()
    {
        for (int i = 0; i < CharacterStatusBoardUIs.Count; i++)
        {
            SetStatusBoard statusBoard = CharacterStatusBoardUIs[i].GetComponent<SetStatusBoard>();
            statusBoard.setByIdolInstance();
        }
    }
    public void FlipStatusBoard(int num)
    {
        characterStatusBoardUIIndex+= (num+ CharacterStatusBoardUIs.Count);
        characterStatusBoardUIIndex= characterStatusBoardUIIndex % CharacterStatusBoardUIs.Count;

        for (int i = 0; i < CharacterStatusBoardUIs.Count; i++)
        {
            CharacterStatusBoardUIs[i].SetActive(false);
        }
        CharacterStatusBoardUIs[characterStatusBoardUIIndex].SetActive(true);
    }
}
