using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Tooltip("For Testing Use")]
    public bool canDie = true;
    public bool IsHiding { get; set; }
    private bool isDead = false;
    private int currentSceneIndex;

    public void Die()
    {
        if (isDead || !canDie) return;
        isDead = true;

        // Play death animation, disable controls, fade to black, etc.
        Debug.Log("Player has been killed!");
        UIManager.Instance.ShowDeathScreen();
        
        // Optional: Reload scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

