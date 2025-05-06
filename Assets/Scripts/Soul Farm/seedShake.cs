using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedAnimationTrigger : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("TShake");
            // 如果只要晃一下就停，可以用 coroutine 關掉
            //StartCoroutine(StopShakeAfterDelay(0.5f)); // 0.5秒後停止
        }
    }

    //IEnumerator StopShakeAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    animator.SetBool("isShaking", false);
    //}
}

