using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickPhotoResize : MonoBehaviour
{
    public RectTransform photoRectTransform;
    public float scaleFactor = 1.2f;
    public void EnlargePhoto()
    {
        photoRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }
    public void ResetPhotoSize()
    {
        photoRectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
}
