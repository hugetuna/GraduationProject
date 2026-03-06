using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/* 掛在需要灰階效果的 UI 元素上 */
public class UIGrayEffect : MonoBehaviour
{
    [SerializeField] private Material grayMaterial;
    [SerializeField] private Image graphic;
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetGrayScale(bool shouldBeGray, bool shouldBeDragged)
    {
        graphic.material = shouldBeGray ? grayMaterial : null;
        canvasGroup.blocksRaycasts = shouldBeDragged;
    }
}
