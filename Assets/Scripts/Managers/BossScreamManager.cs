using System;
using UnityEngine;

public static class BossScreamManager
{
    public static event Action OnScreamStarted;
    public static event Action OnScreamEnded;

    public static bool IsScreaming { get; private set; }

    public static void TriggerScream()
    {
        if (IsScreaming) return;
        IsScreaming = true;
        Debug.Log("[BossScreamManager] Boss is screaming!");
        OnScreamStarted?.Invoke();
    }

    public static void EndScream()
    {
        if (!IsScreaming) return;
        IsScreaming = false;
        Debug.Log("[BossScreamManager] Boss scream ended.");
        OnScreamEnded?.Invoke();
    }
}
