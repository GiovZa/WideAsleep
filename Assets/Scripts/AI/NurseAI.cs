using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NurseAI : MonoBehaviour
{
    public IState currentState;
    [HideInInspector]
    public bool hasSpottedPlayer = false;

    public PatrolState patrolState;
    public AlertState alertState;
    public ChaseState chaseState;
    public KillState killState;

    public Search search;

    public float killRange = 1.5f;

    void Start()
    {
        patrolState = new PatrolState(this);
        alertState = new AlertState(this);
        chaseState = new ChaseState(this);
        killState = new KillState(this);

        search = GetComponent<Search>();

        TransitionToState(patrolState);
    }

    void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(IState newState)
    {
        Debug.Log($"[NurseAI] Transitioning from {currentState?.GetType().Name} to {newState.GetType().Name}");

        // Notify that the player is safe if transitioning from a danger state to patrol
        if ((currentState == alertState || currentState == chaseState) && newState == patrolState)
        {
            AINotifier.NotifyEnemyLostPlayer(this.gameObject);
            hasSpottedPlayer = false; // Reset the flag when the AI loses the player
        }

        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }
    
}
