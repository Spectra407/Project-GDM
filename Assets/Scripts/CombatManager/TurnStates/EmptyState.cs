using UnityEngine;

public class EmptyState : ITurnState
{
    public void Enter()
    {
        Debug.LogError("Calling EmptyState.Enter()");
    }

    public void HandleInput(string _inputID)
    {
        Debug.LogError("Calling EmptyState.HandleInput()");
    }
}