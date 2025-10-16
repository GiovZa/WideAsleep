using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float loadWaitTime = 1f;

    [Header("Scenes")]
    [SerializeField] private SceneField[] gameplayScenes;
    [SerializeField] private int levelToSelect;
    [SerializeField] private SceneField[] menuScenes;
    
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
