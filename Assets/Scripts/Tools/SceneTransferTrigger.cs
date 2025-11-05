using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransferTrigger : MonoBehaviour
{
    //public string targetSceneName;

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("傳送!");
    //    if (other.CompareTag("Player"))
    //    {

    //    }
    //}
    public void teleportByTargetSceneName(string targetSceneName)
    {
        // 儲存土壤資料
        Soil[] allSoils = FindObjectsByType<Soil>(FindObjectsSortMode.None);
        if (allSoils.Length != 0)
        {
            GameManager.Instance.SaveSoilData(new List<Soil>(allSoils));
        }
        //儲存偶像資料
        IdolInstance[] allIdolInstances = FindObjectsByType<IdolInstance>(FindObjectsSortMode.None);
        if (allIdolInstances.Length != 0)
        {
            var sortedIdols = allIdolInstances.OrderBy(i => i.positionInTeam).ToList();
            GameManager.Instance.SaveIdolData(sortedIdols);
        }
        //儲存資源
        ResourceManager resourceManager = FindAnyObjectByType<ResourceManager>();
        if (resourceManager != null)
        {
            GameManager.Instance.SaveResourceData(resourceManager);
        }
        // 傳送到指定場景
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene(targetSceneName);
    }

}
