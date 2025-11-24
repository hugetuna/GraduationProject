using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetStatusBoard : MonoBehaviour
{
    public Image Head;
    public List<Sprite> HeadList;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Fans;
    public TextMeshProUGUI VoNum;
    public TextMeshProUGUI DaNum;
    public TextMeshProUGUI ViNum;
    public Image VoBar;
    public Image DaBar;
    public Image ViBar;
    public TextMeshProUGUI VigorNum;
    //儲存隊伍成員
    [SerializeField]
    private IdolInstance idol = null;
    public TeamManager teamManager;
    public void setIdolInstance(int memberNum)
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        if (teamManager.teamMembers.Count > memberNum)
        {
            idol = teamManager.teamMembers[memberNum].GetComponent<IdolInstance>();
        }
        else
        {
            idol = teamManager.busyMembers[memberNum - teamManager.teamMembers.Count].GetComponent<IdolInstance>();
        }
    }
    public void setByIdolInstance()
    {
        Head.sprite = HeadList[(int)idol.idolIndex];
        Name.text = idol.basicStatus.idolName;
        Fans.text = idol.fans.ToString();
        VoNum.text = idol.vocal.ToString();
        DaNum.text = idol.dance.ToString();
        ViNum.text = idol.visual.ToString();
        VoBar.fillAmount = (float)idol.vocal / 2500;
        DaBar.fillAmount = (float)idol.dance / 2500;
        ViBar.fillAmount = (float)idol.visual / 2500;
        VigorNum.text = idol.vigour.ToString() + "/" + idol.vigourMax.ToString();
    }
}
