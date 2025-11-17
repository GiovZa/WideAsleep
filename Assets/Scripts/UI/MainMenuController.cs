using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject custSceneMenu;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float loadWaitTime = 1f;

    [Header("Scenes")]
    [SerializeField] private SceneField[] gameplayScenes;
    [SerializeField] private int levelToSelect;
    [SerializeField] private SceneField[] menuScenes;
    
    // Continue Button, Skip the cutscene
    public void PlayGame()
    {
        StartCoroutine(FadeAndLoadString(gameplayScenes[levelToSelect].SceneName));
    }

    private IEnumerator FadeAndLoadString(string sceneNameToLoad)
    {
        canvasGroup.DOFade(1, fadeTime);
        yield return new WaitForSeconds(loadWaitTime);
        SceneManager.LoadSceneAsync(sceneNameToLoad);
    }

    public void NewGame()
    {
        StartCoroutine(NewGameCoroutine());
    }

    private IEnumerator NewGameCoroutine()
    {
        StartCoroutine(SceneSwapManager.Instance.QuickFade(loadWaitTime));
        yield return new WaitForSeconds(loadWaitTime);
        mainMenu.SetActive(false);
        custSceneMenu.SetActive(true);
    }

    public void LoadMenuScene(int sceneIndex)
    {
        StartCoroutine(FadeAndLoadString(menuScenes[sceneIndex].SceneName));
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSubMenu(GameObject uiPanelObject)
    {
        UIManager.Instance.OpenUIPanel(uiPanelObject);
    }

    public void CloseSubMenu()
    {
        UIManager.Instance.CloseActiveUI();
    }
}
