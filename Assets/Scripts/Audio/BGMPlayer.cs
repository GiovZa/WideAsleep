using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip sceneBackgroundMusic;

    void Start()
    {
        if (sceneBackgroundMusic != null)
        {
            AudioManager.Instance.PlayBGM(sceneBackgroundMusic);
        }
    }
}
