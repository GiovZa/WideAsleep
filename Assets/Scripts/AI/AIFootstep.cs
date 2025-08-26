using UnityEngine;

public class AIFootstep : MonoBehaviour
{
    [System.Serializable]
    public class SurfaceSound
    {
        public PhysicMaterial physicMaterial;
        public AudioClip[] audioClips;
        [Range(0, 2)]
        public float volume = 1f;
    }

    public SurfaceSound[] surfaceSounds;
    public float raycastDistance = 1.2f;

    private Transform footstepSource; // The point from which the raycast originates

    void Start()
    {
        // Use the object's transform as the source of the raycast
        footstepSource = transform; 
    }

    // This method will be called by animation events
    public void PlayFootstep()
    {
        // Raycast down to find the ground surface
        if (Physics.Raycast(footstepSource.position, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            PhysicMaterial surfaceMaterial = hit.collider.sharedMaterial;

            if (surfaceMaterial == null) return;

            // Find the corresponding sound for the detected surface material
            foreach (var surfaceSound in surfaceSounds)
            {
                if (surfaceSound.physicMaterial == surfaceMaterial)
                {
                    // Pick a random clip from the array
                    if (surfaceSound.audioClips.Length > 0)
                    {
                            AudioManager.Instance.PlayRandomSoundForPlayerOnly(surfaceSound.audioClips, footstepSource.position, surfaceSound.volume);
                    }
                    break;
                }
            }
        }
    }
}
