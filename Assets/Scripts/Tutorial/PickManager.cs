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
    public List<GameObject> PickedMemberList = new List<GameObject>();
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
                //更新視覺
                UpdatePickedMemberUI();
                break;
            }
        }
        
    }
    public void  UpdatePickedMemberUI()
    {
        //先清空已顯示的成員
        foreach (var member in PickedMemberList)
        {
            member.GetComponent<MemberConfig>()?.SetMemberPhoto(-1);
        }
        //顯示已選取的成員
        int displayIndex = 0;
        foreach (var whoPicked in whoPickedList)
        {
            if (whoPicked.isPicked == true)
            {
                PickedMemberList[displayIndex].GetComponent<MemberConfig>()?.SetMemberPhoto((int)whoPicked.pickedIdol);
                Debug.Log("更新已選取成員UI: " + (int)whoPicked.pickedIdol);
                displayIndex++;
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
            //根據隊伍配置設定隊伍編號
            List<int> pickedIds = new List<int>();
            foreach (var whoPicked in whoPickedList)
            {
                if (whoPicked.isPicked == true) pickedIds.Add((int)whoPicked.pickedIdol);
            }
            GameManager.Instance.teamIndex = (int)CalculateTeamIndex(pickedIds);
            //實例化所選取的偶像
            int newIdolCount = 0; // 記錄目前已生成幾個角色（用於設定在訓練等介面裡的初始位置）
            foreach (var whoPicked in whoPickedList)
            {
                if (whoPicked.isPicked == true)
                {
                    GameObject newIdol = Instantiate(PickIdolPrefabs[(int)whoPicked.pickedIdol]);
                    var newIdolInstance = newIdol.GetComponent<IdolInstance>();
                    newIdolInstance.IdolSetUp();
                    newIdolInstance.trainRecord.droppedZoneIndex = newIdolCount; // 覆蓋 BasicTrainRecord 給的值
                    newIdolCount++;
                }
            }
            //直接進入主場景，SceneTransitionManager會處理好場景轉換與資料保存
            GameManager.Instance.SaveInkJSONAssetData(DialogueSaveData);
            SceneTransitionManager.Instance.teleportByTargetSceneName("Dialogue Scene");
        }
    }
    private IdolTeamIndex CalculateTeamIndex(List<int> pickedIds)
    {
        pickedIds.Sort(); // 重要：確保順序一致
        string key = string.Join("", pickedIds); // 變成 "012" 這樣的字串
        Debug.Log("計算隊伍編號，選取的偶像ID組合: " + key);
        // 對照你在 GameManager 定義的 IdolTeamIndex 順序
        return key switch
        {
            "012" => IdolTeamIndex.Kuma_Sirius_Karo,
            "024" => IdolTeamIndex.Kuma_Sirius_Aicor,
            "023" => IdolTeamIndex.Kuma_Sirius_Mizar,
            "014" => IdolTeamIndex.Kuma_Karo_Aicor,
            "013" => IdolTeamIndex.Kuma_Karo_Mizar,
            "034" => IdolTeamIndex.Kuma_Aicor_Mizar,
            "124" => IdolTeamIndex.Sirius_Karo_Aicor,
            "123" => IdolTeamIndex.Sirius_Karo_Mizar,
            "234" => IdolTeamIndex.Sirius_Aicor_Mizar,
            "134" => IdolTeamIndex.Karo_Aicor_Mizar,
            _ => IdolTeamIndex.None
        };
    }
}
