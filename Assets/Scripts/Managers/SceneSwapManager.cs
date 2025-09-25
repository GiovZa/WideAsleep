using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance { get; private set; }

    private static bool _loadFromRespawn;
    private GameObject _player;
    private Vector3 _spwnplayerPosition;
    private Quaternion _spawnplayerRotation;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float loadWaitTime;

    [Header("Scene Categories")]
    [SerializeField] private SceneField[] gameplayScenes;
    [SerializeField] private SceneField[] menuuScenes;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = FindObjectOfType<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("[SceneSwapManager] Could not find a CanvasGroup in the scene. Fading will not work.");
                return;
            }
        }
        _player = GameObject.FindWithTag("Player");
        FadeIn();
    }

    void FadeOut()
    {
        canvasGroup.DOFade(1, fadeTime);
    }

    void FadeIn()
    {
        canvasGroup.DOFade(0, fadeTime);
    }

    private IEnumerator FadeAndLoadInt(int sceneIndexToLoad)
    {
        FadeOut();
        yield return new WaitForSeconds(loadWaitTime);
        SceneManager.LoadSceneAsync(sceneIndexToLoad);
    }

    private IEnumerator FadeAndLoadString(string sceneNameToLoad)
    {
        FadeOut();
        yield return new WaitForSeconds(loadWaitTime);
        SceneManager.LoadSceneAsync(sceneNameToLoad);
    }

    public void PlayGame()
    {
        StartCoroutine(FadeAndLoadInt(1));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {
        StartCoroutine(FadeAndLoadInt(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public static void SwapScene(SceneField sceneToLoad)
    {
        Instance.StartCoroutine(Instance.FadeAndLoadString(sceneToLoad.SceneName));
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1.0f; // Ensure time is not paused
        GameStateManager.Instance.SetState(GameState.InteractingWithUI);
        UIManager.Instance.DisablePauseMenu();
        StartCoroutine(FadeAndLoadInt(0));
    }

    public void RespawnAndReloadScene()
    {
        FindSpawnPoint();
        StartCoroutine(FadeAndLoadInt(SceneManager.GetActiveScene().buildIndex));
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeIn();
        
        // Notify other managers that the scene has changed
        EffectsManager.Instance.InitializeForNewScene();


        if (_loadFromRespawn)
        {
            if (_player != null)
            {
                var playerHub = _player.GetComponent<Player>();
                if (playerHub != null)
                {
                    playerHub.Respawn(_spwnplayerPosition, _spawnplayerRotation);
                }
                else
                {
                    Debug.LogError("[SceneSwapManager] Player script not found on the player object. Cannot respawn.");
                }
            }
            else
            {
                Debug.LogError("[SceneSwapManager] Player reference was lost. Cannot respawn.");
            }
            
            _loadFromRespawn = false;
        }
    }

    private void FindSpawnPoint()
    {
        GameObject respawnPoint = GameObject.FindWithTag("SpawnLocation");
        if (respawnPoint != null)
        {
            _spwnplayerPosition = respawnPoint.transform.position;
            _spawnplayerRotation = respawnPoint.transform.rotation;
        }
        else
        {
            Debug.LogWarning("No 'Respawn' tag found in the scene. Storing world origin (0,0,0) for respawn.");
            _spwnplayerPosition = Vector3.zero;
            _spawnplayerRotation = Quaternion.identity;
        }
        
        _loadFromRespawn = true;
    }
}
