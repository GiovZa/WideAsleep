using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_ActorObject : MonoBehaviour
{
    bool hasTriggered = false;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Trigger()
    {
        //if (hasTriggered) return;
        Debug.Log("Triggering " + gameObject.name);
        hasTriggered = true;
        animator.SetTrigger("Trigger");
    }
}
