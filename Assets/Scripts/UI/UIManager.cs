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
    private PlayerInteraction playerInteraction;

    [Header("Fade In and Out")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeTime = 0.5f;
    [SerializeField] float loadWaitTime;
    
    [Header("Notes")]
    [SerializeField] int notesCollected = 0;
    [SerializeField] int totalNotes = 5;
    public Sprite paperNote;
    public Sprite emptyPaperNote;
    public Image[] notes;

    void Start()
    {
        Instance = this;

        notesCollected = NoteManager.Instance.GetNoteCount();
        totalNotes = NoteManager.Instance.requiredNotes;
        UpdateNotesHUD();

        player = FindObjectOfType<PlayerCharacterController>();
        if (player != null)
        {
            playerInteraction = player.GetComponent<PlayerInteraction>();
        }
        canvasGroup = FindAnyObjectByType<CanvasGroup>();
    }

    private void Update() 
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateManager.Instance.CurrentState == GameState.Paused)
            {
                ExitPauseMenu();
            }
            else if (GameStateManager.Instance.CurrentState == GameState.Gameplay && !GameStateManager.Instance.IsEscapeKeyConsumed())
            {
                CallPauseMenu();
            }
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
        for (int i = 0; i < notes.Length; i++)
        {
            if (i < notesCollected)
                notes[i].sprite = paperNote;
            else
                notes[i].sprite = emptyPaperNote;
            
            if (i < totalNotes)
                notes[i].enabled = true;
            else
                notes[i].enabled = false;
        }
    }

    /// <summary>
    /// Fade in and
    /// </summary>
    public void FadeOut()
    {
       canvasGroup.DOFade(1,fadeTime);
    }

    public void FadeIn()
    {
        canvasGroup.DOFade(0,fadeTime);
    }

    IEnumerator FadeOutAndFadeIn()
    {
        FadeOut();
        yield return new WaitForSeconds(1);
        FadeIn();
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
        GameStateManager.Instance.SetState(GameState.Paused);
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        playerInteraction.SetCrosshairVisible(false);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPauseMenu()
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
        playerInteraction.SetCrosshairVisible(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1.0f; //have to make the game keep running in order for coroutine to work
        StartCoroutine(FadeAndLoad(0));
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadSceneAsync(SceneManager.loadedSceneCount);
    }
}
