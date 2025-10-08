using System;
using playerChar;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [Tooltip("For Testing Use")]
    public bool canDie = true;
    public bool IsHiding { get; set; }
    public bool IsDead { get; private set; }

    // Component References
    public PlayerCharacterController CharacterController { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerWarningSystem WarningSystem { get; private set; }

    public event Action OnPlayerSpawned;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Get component references
        CharacterController = GetComponent<PlayerCharacterController>();
        Interaction = GetComponent<PlayerInteraction>();
        WarningSystem = GetComponent<PlayerWarningSystem>();

        CharacterController.Initialize();
    }

    public void Die()
    {
        if (IsDead || !canDie) return;
        IsDead = true;

        // Play death animation, disable controls, fade to black, etc.
        Debug.Log("Player has been killed!");
        UIManager.Instance.ShowDeathScreen();
    }

    public void Respawn(Vector3 position, Quaternion rotation)
    {
        IsDead = false;

        // Call reset methods on all relevant components
        CharacterController.Respawn(position, rotation);
        WarningSystem.ResetState();
        Interaction.ResetInteraction();
        
        // Reset inventory
        Inventory.Instance.Reset();

        OnPlayerSpawned?.Invoke();
    }
}

