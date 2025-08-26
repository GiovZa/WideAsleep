using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerWarningSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image detectionIcon;

    [Header("Audio")]
    [SerializeField] private AudioSource heartbeatAudioSource;
    [SerializeField] private AudioClip heartbeatSound;
    [SerializeField] private AudioClip aiSpottedSound;

    private List<GameObject> spottingEnemies = new List<GameObject>();

    void Start()
    {
        if (detectionIcon != null)
        {
            detectionIcon.enabled = false;
        }
    }

    void OnEnable()
    {
        AINotifier.OnEnemySpottedPlayer += HandleEnemySpottedPlayer;
        AINotifier.OnEnemyLostPlayer += HandleEnemyLostPlayer;
    }

    void OnDisable()
    {
        AINotifier.OnEnemySpottedPlayer -= HandleEnemySpottedPlayer;
        AINotifier.OnEnemyLostPlayer -= HandleEnemyLostPlayer;
    }

    private void HandleEnemySpottedPlayer(GameObject enemy)
    {
        if (!spottingEnemies.Contains(enemy))
        {
            spottingEnemies.Add(enemy);
        }

        // Play vocalization from the specific enemy who spotted us
        if (aiSpottedSound != null)
        {
            AudioManager.Instance.PlaySoundForPlayerOnly(aiSpottedSound, enemy.transform.position, 0.5f);
        }

        // Start heartbeat and show icon only when the first enemy spots us
        if (spottingEnemies.Count == 1)
        {
            if (detectionIcon != null)
            {
                detectionIcon.enabled = true;
            }

            if (heartbeatAudioSource != null && heartbeatSound != null)
            {
                heartbeatAudioSource.clip = heartbeatSound;
                heartbeatAudioSource.loop = true;
                heartbeatAudioSource.Play();
            }
        }
    }

    private void HandleEnemyLostPlayer(GameObject enemy)
    {
        if (spottingEnemies.Contains(enemy))
        {
            spottingEnemies.Remove(enemy);
        }

        // Stop heartbeat and hide icon only when the last enemy has lost us
        if (spottingEnemies.Count == 0)
        {
            if (detectionIcon != null)
            {
                detectionIcon.enabled = false;
            }

            if (heartbeatAudioSource != null && heartbeatAudioSource.isPlaying)
            {
                heartbeatAudioSource.Stop();
            }
        }
    }
}
