using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OrderSet : MonoBehaviour
{
    public SortingGroup sortingGroup;
    // Start is called before the first frame update
    void Start()
    {
        UpdateSortingOrder();
    }
    public void UpdateSortingOrder()
    {
        sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.z * 100);
    }
}
