using UnityEngine;
using SoundSystem;

[RequireComponent(typeof(Rigidbody))]
public class ThrowableProjectile : MonoBehaviour
{
    [Header("Distraction")]
    public float noiseRadius = 10f;

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
        SoundEvents.EmitSound(transform.position, noiseRadius);
        
        // Optional: Destroy the projectile after a short delay to clean up the scene.
        Destroy(gameObject, 3f); 
    }
}
