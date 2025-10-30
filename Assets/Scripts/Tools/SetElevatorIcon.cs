using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetElevatorIcon : MonoBehaviour
{
    public GameObject elevatorIcon;
    public GameObject elevatorIconPeoples;
    public List<Transform> elevatorIconPositions; // 電梯圖示位置列表
    public List<Sprite> peopleSprites; // 電梯內人數對應的圖片列表
    // Start is called before the first frame update
    public void SetElevatorIconPos(int placeCode)
    {
        // 根據 placeCode 設定電梯圖示位置(0代表地下一樓，其餘為樓層)
        elevatorIcon.transform.position = elevatorIconPositions[placeCode].position;
    }
    public void SetElevatorIconPeople(int peopleCount)
    {
        elevatorIconPeoples.GetComponent<UnityEngine.UI.Image>().sprite = peopleSprites[peopleCount];
    }
}
