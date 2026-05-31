using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolInClothChangeManager : MonoBehaviour
{
    public List<IdolInClothChange> idolsInClothChange;
    public List<DragableCloth> dragableCloths;
    public TeamManager teamManager;
    // Start is called before the first frame update
    void Start()
    {
        SetupIdolWhoInClothChange();
        SetupDragableCloth();
    }
    public void SetupIdolWhoInClothChange()
    {
        //Debug.Log("SetupIdolWhoInClothChange");
        teamManager = FindAnyObjectByType<TeamManager>();
        //在更衣界面上，根據隊伍設置介面偶像的身份
        for (int i = 0; i < idolsInClothChange.Count; i++)
        {
            if(teamManager.teamMembers[i] != null)
            {
                IdolInstance idolInstance = teamManager.teamMembers[i].GetComponent<IdolInstance>();
                idolsInClothChange[i].SetIdolWhoInClothChange(idolInstance.idolIndex);
                idolsInClothChange[i].ChangeCloth(idolInstance.currentClothIndex);
                idolsInClothChange[i].idolInstanceInClothChange = idolInstance;
            }
            else
            {
                idolsInClothChange[i].SetIdolWhoInClothChange(IdolWho.none);
            }
        }
    }
    public void SetupDragableCloth()
    {
        for (int i = 0; i < dragableCloths.Count; i++)
        {
            if (i == 0) continue; //第一個是空的，不用設置
            Item ClothPrefab = ResourceManager.Instance.findItemByID($"CC-{i}");
            //如果沒有這件衣服了，就把它隱藏
            if (ResourceManager.Instance.GetItemCount(ClothPrefab)<=0)
            {
                dragableCloths[i].gameObject.SetActive(false);
                dragableCloths[i].numText.gameObject.SetActive(false);
            }
            else
            {
                dragableCloths[i].gameObject.SetActive(true);
                dragableCloths[i].numText.gameObject.SetActive(true);
                dragableCloths[i].UpdateClothNum(ResourceManager.Instance.GetItemCount(ClothPrefab));
            }
        }
    }
}
