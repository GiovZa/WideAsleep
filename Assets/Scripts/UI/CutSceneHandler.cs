using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneHandler : MonoBehaviour
{
    private CutSceneElementBase[] cutSceneElements;
    private int index = -1; //put -1 bc the first is 0, and don't want to load the first one immediately
    public Button cutSceneButton;
    
    [Header("Scene Transition")]
    [SerializeField] private SceneField sceneToLoadOnEnd;

    public void Start()
    {
        cutSceneElements = GetComponents<CutSceneElementBase>();
    }

    private void ExecuteCurrentElement()
    {
        if (index >= 0 && index < cutSceneElements.Length)
        {
            var currentElement = cutSceneElements[index];
            currentElement.Execute();
            
            // Show or hide the 'next' button based on the element's type
            if (cutSceneButton != null)
            {
                cutSceneButton.gameObject.SetActive(currentElement.isClickToAdvance);
            }
        }
    }

    public void PlayNextElement()
    {
        if (index + 1 < cutSceneElements.Length)
        {
            index++;
            ExecuteCurrentElement();
        }
        else
        {
            // If there are no more elements, end the cutscene.
            EndCutscene();
        }
    }

    public bool IsLastElement(CutSceneElementBase element)
    {
        if (cutSceneElements == null || cutSceneElements.Length == 0)
            return false;
        
        return cutSceneElements[cutSceneElements.Length - 1] == element;
    }

    public void EndCutscene()
    {
        Debug.Log("Cutscene finished. Loading next scene.");
        if (cutSceneButton != null)
        {
            cutSceneButton.gameObject.SetActive(false);
        }
        
        if (sceneToLoadOnEnd != null && !string.IsNullOrEmpty(sceneToLoadOnEnd.SceneName))
        {
            SceneSwapManager.SwapScene(sceneToLoadOnEnd);
        }
        else
        {
            // Optional: Handle case where no scene is specified, maybe just hide the cutscene UI
            gameObject.SetActive(false);
        }
    }
}
