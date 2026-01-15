using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchOrganization : MonoBehaviour
{
    //統一管理開關
    public List<SwitchSetter> switchSetters;
    public List<GameObject> ObjectsInSwitches;
    public void SetAllSwitchText(int i)
    {
        for (int j = 0; j < switchSetters.Count; j++)
        {
            if (j == i)
            {
                switchSetters[j].SetSwitchText(true);
            }
            else
            {
                switchSetters[j].SetSwitchText(false);
            }
        }
    }
    public void SetAllObjectsInSwitchesActive(int i)
    {
        for (int j = 0; j < ObjectsInSwitches.Count; j++)
        {
            if (j == i)
            {
                ObjectsInSwitches[j].gameObject.SetActive(true);
            }
            else
            {
                ObjectsInSwitches[j].gameObject.SetActive(false);
            }
        }
    }
}
