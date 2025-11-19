using UnityEngine;
using UnityEngine.UI;

public class TutorialMaskController : MonoBehaviour
{
    public Material maskMat;
    public Canvas TutorialCanvas;
    public void Focus(Vector2 center ,Vector2 size)
    {
        //丟給 Shader
        maskMat.SetVector("_HoleCenter", center);
        maskMat.SetVector("_HoleSize", size);
    }



    private Vector2 WorldToCanvas(Vector2 pos, RectTransform canvas)
    {
        Vector2 viewport = Camera.main.WorldToViewportPoint(pos);
        return viewport;
    }
}
