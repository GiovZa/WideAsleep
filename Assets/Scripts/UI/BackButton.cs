using UnityEngine;

public class BackButton : MonoBehaviour
{
    public void CloseActiveUI()
    {
        UIManager.Instance.CloseActiveUI();
    }
}
