using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    public UnityEvent Event;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("傳送!");
        if (other.CompareTag("Player"))
        {
            Event.Invoke();
        }
    }

}
