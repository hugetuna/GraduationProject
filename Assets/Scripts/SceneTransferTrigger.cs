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
            if (allSoils.Length != 0)
            {
                GameManager.Instance.SaveSoilData(new List<Soil>(allSoils));
            }
            //儲存偶像資料
            IdolInstance[] allIdolInstances= FindObjectsOfType<IdolInstance>();
            if (allIdolInstances.Length != 0)
            {
                GameManager.Instance.SaveIdolData(new List<IdolInstance>(allIdolInstances));
            }
            //儲存資源
            ResourceManager resourceManager= FindObjectOfType<ResourceManager>();
            if (resourceManager != null)
            {
                GameManager.Instance.SaveResourceData(resourceManager);
            }
            // 傳送到指定場景
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
