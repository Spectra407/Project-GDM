using UnityEngine;

public class PeekingState : ITurnState
{
    private CombatManager _cm;

    public PeekingState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        // 1. Get the peek count from the card Alice just drew
        int peekCount = _cm.lastDrawnCard.peek;
        
        Debug.Log($"Peeking State Entered: Showing top {peekCount} cards.");

        // 2. Tell the PeekManager to open the UI and populate it
        // PeekManager is likely a Singleton based on your previous logs
        PeekManager.Instance.ShowPeek(peekCount); 
    }

    public void HandleInput(string inputID)
    {
        // 3. Usually, the PeekManager's UI buttons handle the selection.
        // However, if Alice clicks a 'Cancel' button, we return here.
        if (inputID == "ClosePeek")
        {
            _cm.ReturnToLastState(); 
        }
    }

    public void Update()
    {
        // No Raycasting needed here if you are using a UI-based Peek menu
    }

    public void Exit()
    {
        // 4. Ensure the UI is hidden when we leave this state
        PeekManager.Instance.ClosePeek(); 
        Debug.Log("Exiting Peeking State.");
    }
}