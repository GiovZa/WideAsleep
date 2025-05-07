using System.Collections;
using System.Collections.Generic;
using playerChar;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    private PlayerCharacterController player;
    private bool isGamePaused;

    void Start()
    {
        player = FindObjectOfType<PlayerCharacterController>();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused == false)
                CallPauseMenu();
            else
                ExitPauseMenu();
        }
    }

    public void CallPauseMenu()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        isGamePaused = true;
        player.isGamePaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPauseMenu()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        isGamePaused = false;
        player.isGamePaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
