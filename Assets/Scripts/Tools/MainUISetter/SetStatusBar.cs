using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetStatusBar : MonoBehaviour
{
    public Image Head;
    public List<Sprite> HeadList;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Fans;
    public TextMeshProUGUI VoNum;
    public TextMeshProUGUI DaNum;
    public TextMeshProUGUI ViNum;
    public Image VigorBar;
    public TeamManager teamManager;
    public void setByTeamManager(int memberNum)
    {
        teamManager= FindAnyObjectByType<TeamManager>();
        if (teamManager == null) return;
        IdolInstance idol = teamManager.teamMembers[memberNum].GetComponent<IdolInstance>();
        Head.sprite = HeadList[(int)idol.idolIndex];
        Name.text = idol.basicStatus.idolName;
        Fans.text = idol.fans.ToString();
        VoNum.text = idol.vocal.ToString();
        DaNum.text = idol.dance.ToString();
        ViNum.text = idol.visual.ToString();
        VigorBar.fillAmount = (float)idol.vigour / idol.vigourMax;
    }
}
