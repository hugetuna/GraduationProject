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
    //儲存隊伍成員
    [SerializeField]
    private IdolInstance idol = null;
    public TeamManager teamManager;
    public void setIdolInstance(int memberNum)
    {
        teamManager = FindAnyObjectByType<TeamManager>();
        idol = teamManager.teamMembers[memberNum].GetComponent<IdolInstance>();
    }
    public void setByIdolInstance()
    {
        Head.sprite = HeadList[(int)idol.idolIndex];
        Name.text = idol.basicStatus.idolName;
        Fans.text = idol.fans.ToString();
        VoNum.text = idol.vocal.ToString();
        DaNum.text = idol.dance.ToString();
        ViNum.text = idol.visual.ToString();
        VigorBar.fillAmount = (float)idol.vigour / idol.vigourMax;
    }
}
