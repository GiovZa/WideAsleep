using UnityEngine;

public class OnesideDoorTrigger : Interactable
{
    OnesideDoorController doorController;
    [SerializeField] bool unlockFromThisSide;
    [SerializeField] QuickOutline doorOutline;

    public override void Start()
    {
        base.Start();
        doorController = GetComponentInParent<OnesideDoorController>();
        
        outline = doorOutline;
        ReleaseOutline();
    }

    public override void Interact(GameObject interactor)
    {
        base.Interact(interactor);
        if (unlockFromThisSide)
            doorController.OpenFromBack();
        else
            doorController.OpenFromFront();
    }

    public override CrosshairType GetCrosshairType()
    {
        return CrosshairType.Door;
    }
}
