using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using playerChar;

/// <summary>
/// Manages the player's "Vision Sense" ability, which clarifies the screen by disabling Depth of Field.
/// </summary>
public class VisionSense : SenseBase
{
    [Header("Effect Settings")]
    [Tooltip("How long the effect should last in seconds")]
    [SerializeField] private float effectDuration = 2.5f;
    [SerializeField] private float effectFadeTime = 0.5f;

    // [Tooltip("The Gaussian Start to use for clear vision.")]
    // [Range(0f, 50f)]
    // public float clearStart = 20f;

    // [Tooltip("The Gaussian End to use for clear vision.")]
    // [Range(0f, 50f)]
    // public float clearEnd = 50f;
 
    // [Tooltip("The Gaussian Max Radius to use for clear vision.")]
    // [Range(0.5f, 1.5f)]
    // public float clearMaxRadius = 0.5f;

    [Tooltip("The maximum intensity of the vignette outer ring (-2 to 1).")]
    [Range(-2, 1)]
    public float vignetteOuterRing = 0f;
    [Tooltip("The maximum intensity of the vignette inner ring (0 to 1).")]
    [Range(0, 1)]
    public float vignetteInnerRing = 0f;
    [Tooltip("The brightness to add to the original brightness.")]
    public float adjustPostBrightnessIntensity = 1.5f;

    [Header("Highlight Settings")]
    [SerializeField] private float highlightDistance = 20f;
    [SerializeField] private LayerMask obstructionMask;
    private Camera playerCamera;
    private PlayerCharacterController m_PlayerCharacterController;
    private List<Interactable> highlightableObjects = new List<Interactable>();
    private List<Interactable> highlightedObjects = new List<Interactable>();

    protected override float EffectDuration => effectDuration;
    protected override InputAction ActivationAction => m_Input.Player.VisionSense;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void Start()
    {
        base.Start();

        m_PlayerCharacterController = GetComponent<PlayerCharacterController>(); 
        if (m_PlayerCharacterController != null)
        {
            playerCamera = m_PlayerCharacterController.PlayerCamera;
        }
        
        VisionSenseHighlight[] targets = FindObjectsOfType<VisionSenseHighlight>();
        foreach (var target in targets)
        {
            Interactable interactable = target.GetComponent<Interactable>();
            if (interactable != null)
            {
                highlightableObjects.Add(interactable);
            }
        }
    }

    protected override void ActivateSense()
    {
        // Trigger the effect via the EffectsManager
        if (effectsManager != null)
        {
            effectsManager.TriggerBlur(effectDuration, effectFadeTime, 0);
            effectsManager.PulseVignette(effectFadeTime, effectDuration, vignetteOuterRing, vignetteInnerRing);
            effectsManager.TriggerPostBrightnessAdjustment(effectDuration, effectFadeTime, adjustPostBrightnessIntensity);
            effectsManager.ToggleFilmGrain(effectDuration, effectFadeTime, false);
        }

        Vector3 playerPosition = transform.position;
        highlightedObjects.Clear();

        foreach (var interactable in highlightableObjects)
        {
            if (interactable != null && Vector3.Distance(playerPosition, interactable.transform.position) <= highlightDistance)
            {
                Vector3 directionToObject = interactable.transform.position - playerCamera.transform.position;
                float distanceToObject = directionToObject.magnitude;

                // Check for a clear line of sight
                if (!Physics.Raycast(playerCamera.transform.position, directionToObject.normalized, distanceToObject, obstructionMask))
                {
                    interactable.RequestOutline();
                    highlightedObjects.Add(interactable);
                }
            }
        }

        StartCoroutine(StopHighlightAfterDelay(effectDuration));
    }

    private System.Collections.IEnumerator StopHighlightAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var interactable in highlightedObjects)
        {
            if (interactable != null)
            {
                interactable.ReleaseOutline();
            }
        }
    }
}
