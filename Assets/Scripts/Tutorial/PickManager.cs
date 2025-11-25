using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public struct WhoPicked
{
    public IdolWho pickedIdol;
    public bool isPicked;
    public void set(IdolWho setPickedIdol,bool setIsPicked)
    {
        pickedIdol = setPickedIdol;
        isPicked = setIsPicked;
    }
}
public class PickManager : MonoBehaviour
{
    public bool isDone=false;
    public List<GameObject> PickIdolPrefabs = new List<GameObject>();//可選取偶像的預製物列表
    public List<WhoPicked> whoPickedList = new List<WhoPicked>();//紀錄每個偶像是否被選取
    public int pickedIdolCount=0;
    [Header("調控UI")]
    public List<GameObject> PhotoList= new List<GameObject>();
    public List<GameObject> FileList = new List<GameObject>();
    public Button confirmButton;
    public TextMeshProUGUI countText;
    [Header("設定傳送對話")]
    public DialogueSaveData DialogueSaveData;
    void Start()
    {
        for (int i = 0; i < PickIdolPrefabs.Count; i++)
        {
            WhoPicked PI= new WhoPicked();
            PI.set((IdolWho)i,false);
            whoPickedList.Add(PI);
        }
    }
    public void ShowFileUI(int whoToShow)
    {
        foreach (var File in FileList)
        {
            File.SetActive(false);
        }
        for (int i = 0; i < whoPickedList.Count; i++)
        {
            if (i == whoToShow)
            {
                FileList[i].SetActive(true);
                break;
            }
        }
    }
    public void pick(int whoToPick)
    {
        for (int i = 0; i < whoPickedList.Count; i++)
        {
            if((int)whoPickedList[i].pickedIdol==whoToPick)
            {
                //取得副本
                WhoPicked tempPicked = whoPickedList[i];
                //更改選取狀態
                if (!tempPicked.isPicked&&pickedIdolCount!=3)//未選取時且未選滿三人
                {
                    pickedIdolCount++;
                    GameObject photoMask= PhotoList[i].transform.Find("PickMask").gameObject;
                    photoMask.SetActive(false);
                    GameObject fileInMark= FileList[i].transform.Find("InMark").gameObject;
                    fileInMark.SetActive(true);
                }
                else if(tempPicked.isPicked)//已選取時
                {
                    pickedIdolCount--;
                    GameObject photoMask = PhotoList[i].transform.Find("PickMask").gameObject;
                    photoMask.SetActive(true);
                    GameObject fileInMark = FileList[i].transform.Find("InMark").gameObject;
                    fileInMark.SetActive(false);
                }
                else
                {
                    //已選滿三人時不做任何事
                    break;
                }
                countText.text = $"( {pickedIdolCount} / 3 )";
                tempPicked.isPicked = !tempPicked.isPicked;
                //存回列表
                whoPickedList[i] = tempPicked;
                break;
            }
        }
        
    }
    public void ConfirmPick()
    {
        if (isDone == true)
        {
            return;
        }
        if (pickedIdolCount == 3)
        {
            isDone = true;
            //實例化所選取的偶像
            foreach (var whoPicked in whoPickedList)
            {
                if (whoPicked.isPicked == true)
                {
                    GameObject newIdol = Instantiate(PickIdolPrefabs[(int)whoPicked.pickedIdol]);
                    newIdol.GetComponent<IdolInstance>().IdolSetUp();
                }
            }
            //直接進入主場景，SceneTransitionManager會處理好場景轉換與資料保存
            GameManager.Instance.SaveInkJSONAssetData(DialogueSaveData);
            SceneTransitionManager.Instance.teleportByTargetSceneName("Dialogue Scene");
        }
        
    }
}
