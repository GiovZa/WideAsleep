using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the player's "Vision Sense" ability, which clarifies the screen by disabling Depth of Field.
/// </summary>
public class VisionSense : SenseBase
{
    [Header("Effect Settings")]
    [Tooltip("How long the effect should last in seconds")]
    [SerializeField] private float effectDuration = 2.5f;
    [SerializeField] private float FadeInTime = 0.5f;
    [SerializeField] private float FadeOutTime = 0.5f;

    [Tooltip("The Gaussian Start to use for clear vision.")]
    [Range(0f, 50f)]
    public float clearStart = 20f;

    [Tooltip("The Gaussian End to use for clear vision.")]
    [Range(0f, 50f)]
    public float clearEnd = 50f;
 
    [Tooltip("The Gaussian Max Radius to use for clear vision.")]
    [Range(0.5f, 1.5f)]
    public float clearMaxRadius = 0.5f;

    [Tooltip("The maximum intensity of the vignette (0 to 1).")]
    [Range(0, 1)]
    public float vignetteIntensity = 0.15f;

    [Header("Highlight Settings")]
    [SerializeField] private float highlightDistance = 20f;
    private List<Interactable> highlightableObjects = new List<Interactable>();
    private List<Interactable> highlightedObjects = new List<Interactable>();

    protected override float EffectDuration => effectDuration;

    protected override void Start()
    {
        base.Start();
        
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
            effectsManager.TriggerVisionSense(effectDuration, FadeInTime, clearStart, clearEnd, clearMaxRadius);
            effectsManager.PulseVignette(FadeInTime, effectDuration, FadeOutTime, vignetteIntensity);
        }

        Vector3 playerPosition = transform.position;
        highlightedObjects.Clear();

        foreach (var interactable in highlightableObjects)
        {
            if (interactable != null && Vector3.Distance(playerPosition, interactable.transform.position) <= highlightDistance)
            {
                interactable.RequestOutline();
                highlightedObjects.Add(interactable);
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
