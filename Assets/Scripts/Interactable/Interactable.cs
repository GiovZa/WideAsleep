using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    protected QuickOutline outline;
    public string message;

    public UnityEvent onInteraction;

    private int outlineRequestCount = 0;

    // Start is called before the first frame update
    public virtual void Start()
    {
        outline = GetComponent<QuickOutline>();
        if (outline == null)
        {
            outline = GetComponentInChildren<QuickOutline>();
        }
        ReleaseOutline();
    }

    public virtual void Interact(GameObject interactor)
    {
        onInteraction.Invoke();
        Debug.Log("[" + gameObject.name + "] " + message);
    }

    public void RequestOutline()
    {
        outlineRequestCount++;
        if (outline != null && outlineRequestCount > 0)
        {
            outline.enabled = true;
        }
    }

    public void ReleaseOutline()
    {
        outlineRequestCount--;
        if (outline != null && outlineRequestCount <= 0)
        {
            outline.enabled = false;
            outlineRequestCount = 0; // Prevent it from going negative
        }
    }

    [System.Obsolete("Use RequestOutline() and ReleaseOutline() instead.")]
    public void DisableOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
    
    [System.Obsolete("Use RequestOutline() and ReleaseOutline() instead.")]
    public void EnableOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public virtual CrosshairType GetCrosshairType()
    {
        return CrosshairType.Interact; //default interact type
    }
}
