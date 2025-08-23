using UnityEngine;

/// <summary>
/// Attach this component to any object that should make a sound when touched.
/// </summary>
public class NoisySurface : MonoBehaviour
{
    public SurfaceType surfaceType;

    [Header("Overrides")]
    [Tooltip("Check this to use the volume and radius settings below instead of the defaults in the AudioManager.")]
    public bool overrideDefaults = false;
    [Range(0, 2)] public float volume = 1f;
    [Range(0, 30)] public float soundRadius = 5f;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        float radiusToDraw = soundRadius;
        Color gizmoColor = Color.yellow; // Yellow for override

        // If not overriding, try to get the default value from the AudioManager
        if (!overrideDefaults)
        {
            gizmoColor = Color.cyan; // Cyan for default
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            if (audioManager != null && audioManager.noisySurfaceSounds != null)
            {
                // In the editor, the dictionary isn't initialized, so we have to search the list.
                foreach (var sound in audioManager.noisySurfaceSounds)
                {
                    if (sound != null && sound.surfaceType == this.surfaceType)
                    {
                        radiusToDraw = sound.soundRadius;
                        break;
                    }
                }
            }
        }

        // Draw the sound radius sphere
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radiusToDraw);
    }
#endif
}
