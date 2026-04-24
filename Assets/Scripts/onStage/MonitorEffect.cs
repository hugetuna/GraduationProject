using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonitorEffect : MonoBehaviour
{
    public Image monitorImage;
    public IdolWho idolInMonitor;
    public List<Sprite> AllIdolSprites;
    public Vector3 startPos;
    public Vector3 endPos;
    private bool isAnimating = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetIdolInMonitor(IdolWho idol)
    {
        if(idol== idolInMonitor||isAnimating==true) return;
        idolInMonitor = idol;
        monitorImage.sprite = AllIdolSprites[(int)idolInMonitor];
        StartCoroutine(CharacterSlideIn());
    }
    private IEnumerator CharacterSlideIn()
    {
        isAnimating = true;
        float duration = 0.5f; // 動畫持續時間
        float elapsedTime = 0f;
        //Vector3 startPos = new Vector3(-monitorImage.rectTransform.rect.width, 0, 0); // 從左側開始
        //Vector3 endPos = Vector3.zero; // 結束位置（中心）
        monitorImage.rectTransform.localPosition = startPos; // 設置初始位置
        while (elapsedTime < duration)
        {
            monitorImage.rectTransform.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        monitorImage.rectTransform.localPosition = endPos; // 確保最終位置正確
        isAnimating = false;
    }
    
}
