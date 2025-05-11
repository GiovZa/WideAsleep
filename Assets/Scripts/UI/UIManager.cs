using System.Collections;
using System.Collections.Generic;
using playerChar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    private PlayerCharacterController player;
    private bool isGamePaused;
    
    [Header("Notes")]
    [SerializeField] int notesCollected = 0;
    [SerializeField] int totalNotes = 5;
    public Image[] notes;

    void Start()
    {
        Instance = this;

        notesCollected = NoteManager.Instance.GetNoteCount();
        totalNotes = NoteManager.Instance.requiredNotes;
        UpdateNotesHUD();

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

    public void OnNoteCollected()
    {
        notesCollected ++;
        UpdateNotesHUD();
    }

    void UpdateNotesHUD()
    {
        for (int i = 0; i < totalNotes; i++)
        {
            if (i < notesCollected)
            {
                notes[i].gameObject.SetActive(true);
            }
            else
            {
                notes[i].gameObject.SetActive(false);
            }
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
