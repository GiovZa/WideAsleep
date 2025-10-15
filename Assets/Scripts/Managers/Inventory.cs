using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    
    public int ThrowableCount { get; private set; }
    public event Action<int> OnThrowableCountChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerSpawned += HandlePlayerSpawned;
        }
    }

    private void OnDisable()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerSpawned -= HandlePlayerSpawned;
        }
    }

    private void HandlePlayerSpawned()
    {
        ThrowableCount = 0;
        OnThrowableCountChanged?.Invoke(ThrowableCount);
    }

    public void AddThrowable()
    {
        ThrowableCount++;
        OnThrowableCountChanged?.Invoke(ThrowableCount);
        Debug.Log($"Inventory: Added throwable. Total: {ThrowableCount}");
    }

    public bool UseThrowable()
    {
        if (ThrowableCount > 0)
        {
            ThrowableCount--;
            OnThrowableCountChanged?.Invoke(ThrowableCount);
            Debug.Log($"Inventory: Used throwable. Remaining: {ThrowableCount}");
            return true;
        }
        return false;
    }

    public void Reset()
    {
        ThrowableCount = 0;
        OnThrowableCountChanged?.Invoke(ThrowableCount);
        Debug.Log("Inventory: Reset throwables to zero.");
    }
}
