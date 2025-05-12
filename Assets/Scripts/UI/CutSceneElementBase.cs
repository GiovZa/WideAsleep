using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneElementBase : MonoBehaviour
{
    public float duration;
    private CutSceneHandler cutSceneHandler;

    void Start()
    {
        cutSceneHandler = GetComponent<CutSceneHandler>();
    }

    public virtual void Execute()
    {

    }

    public virtual void FadeOut()
    {
        
    }

    protected IEnumerator WaitAndAdvance()
    {
        yield return new WaitForSeconds(duration);
        cutSceneHandler.PlayNextElement();
    }
}
