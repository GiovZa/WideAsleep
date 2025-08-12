using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnesideDoorController : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] bool isLocked;
    [SerializeField] bool isOpen;
    [SerializeField] bool IsOpenAtStart;

    [Header("Audio")]
    public AudioClip openSfx;
    public AudioClip lockedSfx;
    AudioSource audioSource;
    Animator doorAnimator;
    //OcclusionPortal occlusionPortal;

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        //occlusionPortal = GetComponentInChildren<OcclusionPortal>();
    }

    void Start()
    {
        //setup when the scene begins
        if (IsOpenAtStart == true)
        {
            doorAnimator.SetTrigger("Open");
            doorAnimator.SetBool("IsClosed", false);
            //occlusionPortal.open = true;
        }
        doorAnimator.SetBool("IsLocked", isLocked);
    }

    public void OpenFromBack() //open from the back, unlock the door
    {
        if (isOpen) return;
        isLocked = false;
        doorAnimator.SetBool("IsLocked", false);
        InteractWithDoor();
    }

    public void OpenFromFront() //open from the front, determine if the door is locked
    {
        if (isOpen) return;

        if (isLocked)
            PlayLockedFeedback();
        else
            InteractWithDoor();
    }

    private void InteractWithDoor()
    {
        if(doorAnimator.GetBool("IsClosed"))
        {
            doorAnimator.SetTrigger("Open");
            doorAnimator.SetBool("IsClosed", false);
            //occlusionPortal.open = true;
        }
        else
        {
            doorAnimator.SetTrigger("Close");
            doorAnimator.SetBool("IsClosed", true);
            //StartCoroutine(CloseOcclusionPortal());
        }

        PlaySfx(openSfx);
    }

    private void PlayLockedFeedback()
    {
        doorAnimator.SetTrigger("Open");
        PlaySfx(lockedSfx);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    // IEnumerator CloseOcclusionPortal()
    // {
    //     yield return new WaitForSecondsRealtime(1);
    //     occlusionPortal.open = false;
    // }
}