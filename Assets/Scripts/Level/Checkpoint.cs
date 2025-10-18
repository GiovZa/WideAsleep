using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    private bool hasBeenActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenActivated || !other.CompareTag("Player"))
        {
            return;
        }

        // Check if the SceneSwapManager exists
        if (SceneSwapManager.Instance != null)
        {
            Debug.Log($"Checkpoint activated at {transform.position}");
            
            // Tell the manager to update the spawn point to this checkpoint's location
            SceneSwapManager.Instance.UpdateSpawnPoint(transform.position, transform.rotation);

            //Prevent reentering the checkpoint
            hasBeenActivated = true;
            GetComponent<Collider>().enabled = false;
        }
    }
}
