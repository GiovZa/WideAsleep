using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Trigger : MonoBehaviour
{
    bool hasTriggered = false;
    BoxCollider boxCollider;
    [SerializeField] TEST_ActorObject objectToTrigger;
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            objectToTrigger.Trigger();
        }
    }
}
