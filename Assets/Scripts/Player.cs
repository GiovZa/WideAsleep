using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isDead = false;

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // Play death animation, disable controls, fade to black, etc.
        Debug.Log("Player has been killed!");

        // Optional: Reload scene
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

