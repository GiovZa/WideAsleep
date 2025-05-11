using System.Collections;
using playerChar;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set;}
    [Header("Pause Menu")]
    [SerializeField] GameObject pauseMenu;
    private PlayerCharacterController player;
    private bool isGamePaused;

    [Header("Fade In and Out")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = 0.5f;
    [SerializeField] float loadWaitTime;
    
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
        canvasGroup = FindAnyObjectByType<CanvasGroup>();
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

    /// <summary>
    /// Notes Collection
    /// </summary>
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

    /// <summary>
    /// Fade in and
    /// </summary>
    public void FadeOut()
    {
       canvasGroup.DOFade(1,fadeTime);
    }

    IEnumerator FadeAndLoad(int scneneIndexToLoad)
    {
        Debug.Log(scneneIndexToLoad);
        FadeOut();
        yield return new WaitForSeconds(loadWaitTime);
        SceneManager.LoadSceneAsync(scneneIndexToLoad);
    }

    /// <summary>
    /// Main Menu
    /// </summary>
    public void PlayGame()
    {
        StartCoroutine(FadeAndLoad(1));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Pause Menu
    /// </summary>
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

    public void ExitToMainMenu()
    {
        Time.timeScale = 1.0f; //have to make the game keep running in order for coroutine to work
        StartCoroutine(FadeAndLoad(0));
    }

}
