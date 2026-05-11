using UnityEngine;

public class InteractableHint : MonoBehaviour
{
    public GameObject hintPrefab;
    //UI出現地點
    public Vector3 SpwanPoint;
    public float XDeg = 0;
    private GameObject hintInstance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowHint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HideHint();
        }
    }

    void ShowHint()
    {
        if (hintPrefab != null && hintInstance == null)
        {
            hintInstance = Instantiate(hintPrefab, transform);
            hintInstance.transform.localPosition = SpwanPoint;
            hintInstance.transform.localEulerAngles = new Vector3(XDeg, 0, 0);
        }
    }

    void HideHint()
    {
        if (hintInstance != null)
        {
            Destroy(hintInstance);
            hintInstance = null;
        }
    }
}

