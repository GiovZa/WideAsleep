using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class SceneSwapManager : MonoBehaviour
{
    public static SceneSwapManager Instance { get; private set; }
    public static GameObject PlayerInstance { get; private set; }

    public static event Action<GameObject> OnPlayerSpawned;
    public static event Action OnPlayerWillBeDestroyed;

    private static bool _loadFromRespawn;
    private Vector3 _spwnplayerPosition;
    private Quaternion _spawnplayerRotation;

    [Header("Player Prefab")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float loadWaitTime;

    [Header("Scene Categories")]
    [SerializeField] private SceneField[] gameplayScenes;
    [SerializeField] private SceneField[] menuScenes;

    public HashSet<string> GameplaySceneNames { get; private set; }
    public HashSet<string> MenuSceneNames { get; private set; }

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

        GameplaySceneNames = new HashSet<string>(gameplayScenes.Select(s => s.SceneName));
        MenuSceneNames = new HashSet<string>(menuScenes.Select(s => s.SceneName));
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
        _loadFromRespawn = true; // The flag should only be set when we are explicitly respawning.
        StartCoroutine(FadeAndLoadInt(SceneManager.GetActiveScene().buildIndex));
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FadeIn();
        
        // Notify other managers that the scene has changed
        EffectsManager.Instance.InitializeForNewScene();

        HandlePlayerLifecycle(scene);
    }

    private void HandlePlayerLifecycle(Scene scene)
    {
        bool isGameplayScene = GameplaySceneNames.Contains(scene.name);
        bool isMenuScene = MenuSceneNames.Contains(scene.name);

        if (isMenuScene)
        {
            GameStateManager.Instance.SetState(GameState.InteractingWithUI);
            if (PlayerInstance != null)
            {
                OnPlayerWillBeDestroyed?.Invoke();
                Destroy(PlayerInstance);
                PlayerInstance = null;
            }
        }
        else if (isGameplayScene)
        {
            GameStateManager.Instance.SetState(GameState.Gameplay);
            if (PlayerInstance == null)
            {
                if (playerPrefab != null)
                {
                    PlayerInstance = Instantiate(playerPrefab);
                    OnPlayerSpawned?.Invoke(PlayerInstance);
                }
                else
                {
                    Debug.LogError("[SceneSwapManager] Player Prefab is not assigned!");
                    return;
                }
            }

            if (_loadFromRespawn)
            {
                // On a respawn, use the coordinates we stored before reloading.
                PlayerInstance.GetComponent<Player>().Respawn(_spwnplayerPosition, _spawnplayerRotation);
                _loadFromRespawn = false;
            }
            else
            {
                // On a normal scene load, find the spawn point in the new scene.
                GameObject spawnPoint = GameObject.FindWithTag("SpawnLocation");
                if (spawnPoint != null)
                {
                    PlayerInstance.GetComponent<Player>().Respawn(spawnPoint.transform.position, spawnPoint.transform.rotation);
                }
                else
                {
                    Debug.LogWarning("No 'SpawnLocation' tag found in this scene. Player will use its default position.");
                }
            }
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
    }
}
