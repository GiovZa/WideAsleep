using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PianoDoorController : MonoBehaviour
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

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        //setup when the scene begins
        if (IsOpenAtStart == true)
        {
            doorAnimator.SetTrigger("Open");
            doorAnimator.SetBool("IsClosed", false);
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
        }
        else
        {
            doorAnimator.SetTrigger("Close");
            doorAnimator.SetBool("IsClosed", true);
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
}