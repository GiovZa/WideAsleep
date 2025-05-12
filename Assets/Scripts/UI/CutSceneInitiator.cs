using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneInitiator : MonoBehaviour
{
    private CutSceneHandler cutSceneHandler;

    public void Start()
    {
        cutSceneHandler = GetComponent<CutSceneHandler>();
    }

    private void OTriggerEnter(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            cutSceneHandler.PlayNextElement();
    }

    public void PlayNext()
    {
        cutSceneHandler.PlayNextElement();
    }
}
