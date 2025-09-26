using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float loadWaitTime = 1f;

    [Header("Scenes")]
    [SerializeField] private SceneField[] gameplayScenes;
    [SerializeField] private int levelToSelect;
    
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
    
    public void QuitGame()
    {
        Application.Quit();
    }
}
