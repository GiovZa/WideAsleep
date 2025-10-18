using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Trigger : MonoBehaviour
{
    bool hasTriggered = false;
    BoxCollider boxCollider;
    [SerializeField] TEST_ActorObject objectToTrigger;
    [SerializeField] AudioClip triggerSound;
    [HideInInspector]
    public int sequenceNumber { get; set; } = 0;
    
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (sequenceNumber != objectToTrigger.sequenceTriggered) return;
        
        if (other.gameObject.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            objectToTrigger.Trigger();
            if (triggerSound != null)
            {
                AudioManager.Instance.Play(triggerSound, transform.position, 1f, 5f, true, AudioManager.Instance.SFXMixerGroup);
            }
        }
    }
}
