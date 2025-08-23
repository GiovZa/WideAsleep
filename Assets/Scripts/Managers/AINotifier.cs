using System;
using UnityEngine;

public static class AINotifier
{
    // Event fired when an enemy spots the player
    // Passes the GameObject of the enemy that spotted the player
    public static event Action<GameObject> OnEnemySpottedPlayer;

    // Event fired when an enemy loses track of the player
    // Passes the GameObject of the enemy that lost the player
    public static event Action<GameObject> OnEnemyLostPlayer;

    public static void NotifyEnemySpottedPlayer(GameObject enemy)
    {
        OnEnemySpottedPlayer?.Invoke(enemy);
    }

    public static void NotifyEnemyLostPlayer(GameObject enemy)
    {
        OnEnemyLostPlayer?.Invoke(enemy);
    }
}
