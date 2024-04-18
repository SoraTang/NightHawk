using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine: MonoBehaviour
{
    public PlayerState currentPlayerState
    {
        get; set;
    }

    public void ChangeState(PlayerState newState)
    {
        currentPlayerState.ExitState();
        currentPlayerState = newState;
        currentPlayerState.EnterState();
    }
    
    void Start()
    {
        currentPlayerState = GetInitialState();
        if(currentPlayerState != null) currentPlayerState.EnterState();
    }

    void Update()
    {
        if(currentPlayerState != null) currentPlayerState.UpdateLogic();
    }

    void LateUpdate()
    {
        if(currentPlayerState != null) currentPlayerState.UpdatePhysics();
    }

    protected virtual PlayerState GetInitialState()
    {
        return null;
    }

}
