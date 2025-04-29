using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerDoor : MonoBehaviour
{
    Animator doorAnimator;
    private bool inTriggerArea;
    [SerializeField] bool IsOpenAtStart;

    void Awake()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        if (IsOpenAtStart == true)
        {
            doorAnimator.SetTrigger("Open");
            doorAnimator.SetBool("IsClosed", false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("e") && inTriggerArea)
        {
            if(doorAnimator.GetBool("IsClosed"))
                {
                    doorAnimator.SetTrigger("Open");
                    doorAnimator.SetBool("IsClosed", false);
                }
                else
                {
                    doorAnimator.SetTrigger("Close");
                    doorAnimator.SetBool("IsClosed", true);
                }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            {
                inTriggerArea = true;
            }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
            {
                inTriggerArea = false;
            }
    }
}

