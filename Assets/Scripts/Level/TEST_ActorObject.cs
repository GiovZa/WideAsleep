using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_ActorObject : MonoBehaviour
{
    Animator animator;
    [SerializeField] TEST_Trigger[] triggers;
    [HideInInspector]
    public int sequenceTriggered { get; set; } = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < triggers.Length; i++)
        {
            triggers[i].sequenceNumber = i;
        }
    }

    public void Trigger()
    {
        if (sequenceTriggered >= triggers.Length) return;
        Debug.Log("Triggering " + gameObject.name);
        animator.SetTrigger("Trigger");
        sequenceTriggered++;
    }
}
