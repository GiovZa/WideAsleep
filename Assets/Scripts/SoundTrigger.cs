using UnityEngine;
using UnityEngine.Audio;

public class SoundTrigger : MonoBehaviour
{
    [Header("Global Settings")]
    public float volume = 1f;
    public bool spatial = true;
    public AudioMixerGroup mixerGroup;

    [Header("Step Trigger")]
    public AudioClip stepClip;
    public string[] stepTags = { "Player", "Enemy" };

    [Header("Interact Trigger")]
    public AudioClip interactClip;
    public KeyCode interactKey = KeyCode.E;
    public float interactRadius = 3f;

    [Header("Timer Trigger")]
    public AudioClip timerClip;
    public float timerDelay = 5f;
    public bool timerLoop = false;
    private float timer;
    private bool hasPlayedTimer = false;

    [Header("Repeat Trigger")]
    public AudioClip repeatClip;
    public float repeatInterval = 5f;
    private float repeatTimer;

    [Header("State Change Trigger")]
    public AudioClip stateChangeClip;
    public bool currentState;
    private bool lastState;

    [Header("Animation Event")]
    public AudioClip animationEventClip;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        HandleInteract();
        HandleTimer();
        HandleRepeat();
        HandleStateChange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (stepClip == null) return;

        foreach (var tag in stepTags)
        {
            if (other.CompareTag(tag))
            {
                PlayClip(stepClip);
                break;
            }
        }
    }

    private void HandleInteract()
    {
        if (interactClip == null || player == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            Debug.Log("E pressesd");
            Debug.Log(Vector3.Distance(player.position, gameObject.transform.position));
            if (Vector3.Distance(player.position, gameObject.transform.position) <= interactRadius)
                PlayClip(interactClip);
        }
    }

    private void HandleTimer()
    {
        if (timerClip == null) return;

        if (!hasPlayedTimer || timerLoop)
        {
            timer += Time.deltaTime;
            if (timer >= timerDelay)
            {
                PlayClip(timerClip);
                hasPlayedTimer = true;
                if (timerLoop) timer = 0f;
            }
        }
    }

    private void HandleRepeat()
    {
        if (repeatClip == null) return;

        repeatTimer += Time.deltaTime;
        if (repeatTimer >= repeatInterval)
        {
            PlayClip(repeatClip);
            repeatTimer = 0f;
        }
    }

    private void HandleStateChange()
    {
        if (stateChangeClip == null) return;

        if (currentState != lastState)
        {
            PlayClip(stateChangeClip);
            lastState = currentState;
        }
    }

    public void PlayAnimationEventSound()
    {
        if (animationEventClip != null)
            PlayClip(animationEventClip);
    }

    public void SetState(bool state)
    {
        currentState = state;
    }

    private void PlayClip(AudioClip clip)
    {
        AudioManager.Instance.Play(clip, gameObject.transform.position, volume, spatial, mixerGroup);
    }
}
