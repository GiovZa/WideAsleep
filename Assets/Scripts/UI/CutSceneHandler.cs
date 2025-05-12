using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneHandler : MonoBehaviour
{
    private CutSceneElementBase[] cutSceneElements;
    private int index = -1; //put -1 bc the first is 0, and don't want to load the first one immediately
    public Button cutSceneButton;
    public Button EndCutSceneButton;

    public void Start()
    {
        cutSceneElements = GetComponents<CutSceneElementBase>();
    }

    private void ExecuteCurrentElement()
    {
        if (index >= 0 && index < cutSceneElements.Length)
            cutSceneElements[index].Execute();
        else if(index == cutSceneElements.Length)
        {
            cutSceneButton.gameObject.SetActive(false);
            EndCutSceneButton.gameObject.SetActive(true);
        }
    }

    public void PlayNextElement()
    {
        index++;
        ExecuteCurrentElement();
    }
}
