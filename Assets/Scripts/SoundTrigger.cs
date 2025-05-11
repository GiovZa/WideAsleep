using UnityEngine;
using UnityEngine.Audio;
using SoundSystem;

public class SoundTrigger : MonoBehaviour
{
    [Header("Global Settings")]
    public float volume = 1f;
    public bool spatial = true;
    public AudioMixerGroup mixerGroup;

    [Header("Step Trigger")]
    public AudioClip stepClip;
    [Tooltip("How many seconds of the step sound to play (<=0 = full clip)")]
    public float stepPlayLength = 1f;
    public string[] stepTags = { "Player", "Enemy" };

    [Header("Interact Trigger")]
    public AudioClip interactClip;
    [Tooltip("How many seconds of the interact sound to play (<=0 = full clip)")]
    public float interactPlayLength = 0f;
    public KeyCode interactKey = KeyCode.E;
    public float interactRadius = 3f;

    [Header("Timer Trigger")]
    public AudioClip timerClip;
    [Tooltip("How many seconds of the timer sound to play (<=0 = full clip)")]
    public float timerPlayLength = 0f;
    public float timerDelay = 5f;
    public bool timerLoop = false;
    private float timer;
    private bool hasPlayedTimer = false;

    [Header("Repeat Trigger")]
    public AudioClip repeatClip;
    [Tooltip("How many seconds of the repeat sound to play (<=0 = full clip)")]
    public float repeatPlayLength = 0f;
    public float repeatInterval = 5f;
    private float repeatTimer;

    [Header("State Change Trigger")]
    public AudioClip stateChangeClip;
    [Tooltip("How many seconds of the state change sound to play (<=0 = full clip)")]
    public float stateChangePlayLength = 0f;
    public bool currentState;
    private bool lastState;

    [Header("Animation Event")]
    public AudioClip animationEventClip;
    [Tooltip("How many seconds of the animation event sound to play (<=0 = full clip)")]
    public float animationEventPlayLength = 0f;

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
                PlayClip(stepClip, stepPlayLength);
                break;
            }
        }
    }

    private void HandleInteract()
    {
        if (interactClip == null || player == null) return;

        if (Input.GetKeyDown(interactKey) &&
            Vector3.Distance(player.position, transform.position) <= interactRadius)
        {
            PlayClip(interactClip, interactPlayLength);
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
                PlayClip(timerClip, timerPlayLength);
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
            PlayClip(repeatClip, repeatPlayLength);
            repeatTimer = 0f;
        }
    }

    private void HandleStateChange()
    {
        if (stateChangeClip == null) return;

        if (currentState != lastState)
        {
            PlayClip(stateChangeClip, stateChangePlayLength);
            lastState = currentState;
        }
    }

    public void PlayAnimationEventSound()
    {
        if (animationEventClip != null)
            PlayClip(animationEventClip, animationEventPlayLength);
    }

    public void SetState(bool state)
    {
        currentState = state;
    }

    private void PlayClip(AudioClip clip, float playDuration)
    {
        AudioManager.Instance.Play(
            clip,
            transform.position,
            volume,
            spatial,
            mixerGroup,
            playDuration
        );
    }
}
