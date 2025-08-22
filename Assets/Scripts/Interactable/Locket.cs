using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locket : Interactable
{
    public override void Interact()
    {
        base.Interact();

        gameObject.SetActive(false); // Disables the locket
    }
}
