using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneElementBase : MonoBehaviour
{
    public float duration;
    [Tooltip("If true, this element will wait for a button click to advance. If false, it will advance automatically after its duration.")]
    public bool isClickToAdvance = true;
    private CutSceneHandler cutSceneHandler;

    void Start()
    {
        cutSceneHandler = GetComponent<CutSceneHandler>();
    }

    public virtual void Execute()
    {
        // If this element is timed, start the coroutine to advance automatically.
        if (!isClickToAdvance)
        {
            StartCoroutine(WaitAndAdvance());
        }
    }

    public virtual void FadeOut()
    {
        
    }

    protected IEnumerator WaitAndAdvance()
    {
        yield return new WaitForSeconds(duration);
        
        // After waiting, check if this is the last element.
        if (cutSceneHandler.IsLastElement(this))
        {
            cutSceneHandler.EndCutscene();
        }
        else
        {
            cutSceneHandler.PlayNextElement();
        }
    }
}
