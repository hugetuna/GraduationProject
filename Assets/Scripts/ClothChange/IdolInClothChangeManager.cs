using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdolInClothChangeManager : MonoBehaviour
{
    public List<IdolInClothChange> idolsInClothChange;
    public TeamManager teamManager;
    // Start is called before the first frame update
    void Start()
    {
        
        SetupIdolWhoInClothChange();
    }
    public void SetupIdolWhoInClothChange()
    {
        Debug.Log("SetupIdolWhoInClothChange");
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
}
