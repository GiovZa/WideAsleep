using UnityEngine;

public class GenericUIPanel : MonoBehaviour, IGenericUI
{
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
