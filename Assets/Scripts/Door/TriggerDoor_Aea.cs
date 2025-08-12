using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TriggerDoor_Area : Interactable
{
    Animator doorAnimator;
    OcclusionPortal occlusionPortal;
    private bool inTriggerArea;
    [SerializeField] bool IsOpenAtStart;
    [SerializeField] bool IsLocked;

    void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        occlusionPortal = GetComponentInChildren<OcclusionPortal>();
    }

    public override void Start()
    {
        if (IsOpenAtStart == true)
        {
            doorAnimator.SetTrigger("Open");
            doorAnimator.SetBool("IsClosed", false);
        }
        doorAnimator.SetBool("IsLocked", IsLocked);
    }

    void Update()
    {
        if (Input.GetKeyDown("e") && inTriggerArea)
        {
            if (doorAnimator.GetBool("IsClosed"))
            {
                doorAnimator.SetTrigger("Open");
                if (doorAnimator.GetBool("IsLocked") == false)
                {
                    doorAnimator.SetBool("IsClosed", false);
                    occlusionPortal.open = true;
                }
            }
            else
            {
                doorAnimator.SetTrigger("Close");
                doorAnimator.SetBool("IsClosed", true);
                StartCoroutine(CloseOcclusionPortal());
            }
        }
    }

    IEnumerator CloseOcclusionPortal()
    {
        yield return new WaitForSecondsRealtime(1);
        occlusionPortal.open = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inTriggerArea = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inTriggerArea = false;
        }
    }
}

