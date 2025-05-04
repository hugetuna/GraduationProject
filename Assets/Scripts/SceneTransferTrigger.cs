using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransferTrigger : MonoBehaviour
{
    public string targetSceneName;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("傳送!");
        if (other.CompareTag("Player"))
        {
            // 儲存土壤資料
            Soil[] allSoils = FindObjectsOfType<Soil>();
            Debug.Log(allSoils);
            GameManager.Instance.SaveSoilData(new List<Soil>(allSoils));

            // 傳送到指定場景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
