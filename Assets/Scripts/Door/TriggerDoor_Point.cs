using System.Collections;
using UnityEngine;

public class TriggerDoor_Point : Interactable
{
    Animator doorAnimator;
    OcclusionPortal occlusionPortal;

    [Header("Initialization Setting")]
    [SerializeField] bool IsOpenAtStart;
    [SerializeField] bool IsLocked;

    void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        occlusionPortal = GetComponent<OcclusionPortal>();
        if (occlusionPortal == null)
        {
            occlusionPortal = GetComponentInChildren<OcclusionPortal>();
        }
    }

    public override void Start()
    {
        base.Start();

        if (IsOpenAtStart == true)
        {
            doorAnimator.SetTrigger("Open");
            doorAnimator.SetBool("IsClosed", false);
        }
        doorAnimator.SetBool("IsLocked", IsLocked);
    }

    public override CrosshairType GetCrosshairType()
    {
        return CrosshairType.Door;
    }

    //Door Control Methods
    public override void Interact()
    {
        base.Interact();
        if (doorAnimator.GetBool("IsClosed"))
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        doorAnimator.SetTrigger("Open");
        if (!IsLocked)
        {
            doorAnimator.SetBool("IsClosed", false);
            occlusionPortal.open = true;
        }
    }
    
    private void CloseDoor()   
    {
        doorAnimator.SetTrigger("Close");
        doorAnimator.SetBool("IsClosed", true);
        StartCoroutine(CloseOcclusionPortal());
    }

    //Visual Effects
    IEnumerator CloseOcclusionPortal()
    {
        yield return new WaitForSecondsRealtime(1);
        occlusionPortal.open = false;
    }
}

