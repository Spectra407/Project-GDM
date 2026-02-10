using UnityEngine;

// Based on this tutorial: https://howtomakeanrpg.com/r/a/state-machines.html

public interface ITurnState
{
    

    void Enter(); // Runs when the state starts
    void HandleInput(string inputID); // Logic for mouse clicks or button presses
    void Update(); // Runs every frame during the state 
    void Exit(); // Cleanup logic when you exit the state

}
