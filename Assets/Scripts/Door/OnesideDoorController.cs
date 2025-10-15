using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;

public class OnesideDoorController : MonoBehaviour
{
    [Header("Status")]
    [SerializeField] bool isLocked;
    [SerializeField] bool isOpen;
    [SerializeField] bool IsOpenAtStart;

    [Header("Audio")]
    public AudioClip openSfx;
    public AudioClip closeSfx;
    public AudioClip lockedSfx;
    [SerializeField] float SfxRadius = 3f;
    AudioSource audioSource;
    Animator doorAnimator;
    [SerializeField] GraphUpdateScene graphUpdateScene;
    //OcclusionPortal occlusionPortal;

    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        graphUpdateScene = GetComponent<GraphUpdateScene>();
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

            if (graphUpdateScene != null)
            {
                graphUpdateScene.setWalkability = true;
                graphUpdateScene.Apply();
            }

            AudioManager.Instance.Play(openSfx, transform.position, 1f, SfxRadius, true, AudioManager.Instance.SFXMixerGroup);
        }
        else
        {
            doorAnimator.SetTrigger("Close");
            doorAnimator.SetBool("IsClosed", true);
            //StartCoroutine(CloseOcclusionPortal());
            
            if (graphUpdateScene != null)
            {
                graphUpdateScene.setWalkability = false;
                graphUpdateScene.Apply();
            }

            AudioManager.Instance.Play(closeSfx, transform.position, 1f, SfxRadius, true, AudioManager.Instance.SFXMixerGroup);
        }
    }

    private void PlayLockedFeedback()
    {
        doorAnimator.SetTrigger("Open");
        AudioManager.Instance.Play(lockedSfx, transform.position, 1f, SfxRadius, true, AudioManager.Instance.SFXMixerGroup);
    }

    // IEnumerator CloseOcclusionPortal()
    // {
    //     yield return new WaitForSecondsRealtime(1);
    //     occlusionPortal.open = false;
    // }
}