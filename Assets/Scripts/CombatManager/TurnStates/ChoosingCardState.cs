using UnityEngine;

public class ChoosingCardState : ITurnState
{
    private CombatManager cm;

    public ChoosingCardState(CombatManager cm)
    {
        this.cm = cm;
    }

    public void Enter()
    {
        
    }

    public void HandleInput(string inputID)
    {
        
    }
}
