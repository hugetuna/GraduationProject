using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    public FadeObject[] objectsToFade;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var obj in objectsToFade)
                obj.FadeOut();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var obj in objectsToFade)
                obj.FadeIn();
        }
    }
}
