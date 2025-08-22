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
        DisableOutline();
    }

    public override void Interact()
    {
        base.Interact();
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
