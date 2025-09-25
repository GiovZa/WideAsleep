using UnityEngine;
using System;

public enum GameState
{
    Gameplay,
    Paused,
    InteractingWithUI
}

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public event Action<GameState> OnGameStateChanged;
    private bool escapeConsumed = false;

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

        CurrentState = GameState.Gameplay;
    }
    
    private void LateUpdate()
    {
        escapeConsumed = false;
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");
        OnGameStateChanged?.Invoke(newState);
    }
    
    public void ConsumeEscapeKeyForThisFrame()
    {
        escapeConsumed = true;
    }

    public bool IsEscapeKeyConsumed()
    {
        return escapeConsumed;
    }
}
