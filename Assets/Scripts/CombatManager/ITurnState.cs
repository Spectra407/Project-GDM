using UnityEngine;

// Based on this tutorial: https://howtomakeanrpg.com/r/a/state-machines.html

public interface ITurnState
{
    // Depending on how we organize things, this state machine might not have any behavior that needs frame-by-frame updating
    // It'll depend on how we manage animations
    void Update(float dt);
    void HandleInput();

    void Enter();
    void Exit();
    
}
