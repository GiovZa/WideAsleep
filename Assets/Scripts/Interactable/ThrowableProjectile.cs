using UnityEngine;
using SoundSystem;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableProjectile : MonoBehaviour
{
    [Header("Distraction")]
    public AudioClip[] hitSounds;
    public float volume = 1.0f;
    public float noiseRadius = 15f;
    private bool hasHitSurface = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Ensure we only trigger the noise once.
        if (hasHitSurface)
        {
            return;
        }

        hasHitSurface = true;
        
        // Use the existing SoundSystem to broadcast the noise event.
        AudioManager.Instance.PlayRandomSound(hitSounds, transform.position, volume, noiseRadius);
        
        // Optional: Destroy the projectile after a short delay to clean up the scene.
        Destroy(gameObject, 2f); 
    }
}
