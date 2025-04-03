// using UnityEngine;

// public class SoundTrigger : MonoBehaviour
// {
//     [Header("Step Trigger")]
//     public SoundEvent stepSound;
//     public string[] stepTags = { "Player", "Enemy" };

//     [Header("Interact Trigger")]
//     public SoundEvent interactSound;
//     public KeyCode interactKey = KeyCode.E;
//     public float interactRadius = 3f;

//     [Header("Timer Trigger")]
//     public SoundEvent timerSound;
//     public float timerDelay = 5f;
//     public bool timerLoop = false;
//     private float timer;
//     private bool hasPlayedTimer = false;

//     [Header("Repeat Trigger")]
//     public SoundEvent repeatSound;
//     public float repeatInterval = 5f;
//     private float repeatTimer;

//     [Header("State Change Trigger")]
//     public SoundEvent stateChangeSound;
//     public bool currentState;
//     private bool lastState;

//     [Header("Animation Event")]
//     public SoundEvent animationEventSound;

//     private Transform player;

//     void Start()
//     {
//         player = GameObject.FindGameObjectWithTag("Player")?.transform;
//     }

//     void Update()
//     {
//         HandleInteract();
//         HandleTimer();
//         HandleRepeat();
//         HandleStateChange();
//     }

//     // -------- OnTriggerEnter (Step) --------
//     private void OnTriggerEnter(Collider other)
//     {
//         if (stepSound == null) return;

//         foreach (var tag in stepTags)
//         {
//             if (other.CompareTag(tag))
//             {
//                 AudioManager.Instance.Play(stepSound, transform.position);
//                 break;
//             }
//         }
//     }

//     // -------- Interact Key --------
//     private void HandleInteract()
//     {
//         if (interactSound == null || player == null) return;

//         if (Input.GetKeyDown(interactKey))
//         {
//             float distance = Vector3.Distance(player.position, transform.position);
//             if (distance <= interactRadius)
//             {
//                 AudioManager.Instance.Play(interactSound, transform.position);
//             }
//         }
//     }

//     // -------- Timer Trigger --------
//     private void HandleTimer()
//     {
//         if (timerSound == null) return;

//         if (!hasPlayedTimer || timerLoop)
//         {
//             timer += Time.deltaTime;
//             if (timer >= timerDelay)
//             {
//                 AudioManager.Instance.Play(timerSound, transform.position);
//                 hasPlayedTimer = true;
//                 if (timerLoop) timer = 0f;
//             }
//         }
//     }

//     // -------- Repeat Trigger --------
//     private void HandleRepeat()
//     {
//         if (repeatSound == null) return;

//         repeatTimer += Time.deltaTime;
//         if (repeatTimer >= repeatInterval)
//         {
//             AudioManager.Instance.Play(repeatSound, transform.position);
//             repeatTimer = 0f;
//         }
//     }

//     // -------- State Change Trigger --------
//     private void HandleStateChange()
//     {
//         if (stateChangeSound == null) return;

//         if (currentState != lastState)
//         {
//             AudioManager.Instance.Play(stateChangeSound, transform.position);
//             lastState = currentState;
//         }
//     }

//     // -------- Animation Event Trigger --------
//     public void PlayAnimationSound()
//     {
//         if (animationEventSound != null)
//             AudioManager.Instance.Play(animationEventSound, transform.position);
//     }

//     // -------- Public Setter for State --------
//     public void SetState(bool state)
//     {
//         currentState = state;
//     }
// }
