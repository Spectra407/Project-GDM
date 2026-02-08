using UnityEngine;

// Based on this tutorial: https://howtomakeanrpg.com/r/a/state-machines.html

public interface ITurnState
{
    void HandleInput(string inputID);

    void Enter();
    
}
