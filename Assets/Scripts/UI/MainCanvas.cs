using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void OnEnable()
    {
        SceneSwapManager.OnPlayerSpawned += SetWorldCamera;
    }

    void OnDisable()
    {
        SceneSwapManager.OnPlayerSpawned -= SetWorldCamera;
    }

    public void SetWorldCamera(GameObject player)
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.worldCamera = player.GetComponent<playerChar.PlayerCharacterController>().PlayerCamera;
        }
    }
}
