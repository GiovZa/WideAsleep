using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locket : Interactable
{
    [SerializeField] SceneField sceneToLoad;
    
    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        SceneSwapManager.SwapScene(sceneToLoad);
    }
}
