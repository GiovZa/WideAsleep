using System.Collections;
using UnityEngine;

public class TriggerDoor_Point : Interactable
{
    Animator doorAnimator;
    OcclusionPortal occlusionPortal;

    [Header("Initialization Setting")]
    [SerializeField] bool IsOpenAtStart;
    [SerializeField] bool IsLocked;

    [Header("Audio")]
    public AudioClip openSfx;
    public AudioClip closeSfx;
    public AudioClip lockedSfx;
    [SerializeField] float SfxRadius = 3f;
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
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
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
        StopAllCoroutines();
        doorAnimator.SetTrigger("Open");
        if (!IsLocked)
        {
            doorAnimator.SetBool("IsClosed", false);
            occlusionPortal.open = true;
            AudioManager.Instance.Play(openSfx, transform.position, 1f, SfxRadius);
        }
        else
        {
            AudioManager.Instance.Play(lockedSfx, transform.position, 1f, SfxRadius);
        }
    }
    
    private void CloseDoor()   
    {
        StopAllCoroutines();
        doorAnimator.SetTrigger("Close");
        doorAnimator.SetBool("IsClosed", true);
        StartCoroutine(CloseOcclusionPortal());
        AudioManager.Instance.Play(closeSfx, transform.position, 1f, SfxRadius);
    }

    //Visual Effects
    IEnumerator CloseOcclusionPortal()
    {
        yield return new WaitForSecondsRealtime(1);
        occlusionPortal.open = false;
    }
}

