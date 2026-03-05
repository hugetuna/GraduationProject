using UnityEngine;
using UnityEngine.UI;

/* 掛在需要灰階效果的 UI 元素上 */
public class UIGrayEffect : MonoBehaviour
{
    [SerializeField] private Material grayMaterial;
    private Graphic graphic;

    void Awake()
    {
        graphic = GetComponent<Graphic>();
    }

    public void SetGrayScale(bool shouldBeGray)
    {
        if (graphic == null) return;
        graphic.material = shouldBeGray ? grayMaterial : null;
    }
}
