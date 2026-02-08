using UnityEngine;

public class EvaluatingCardsState : ITurnState
{
    private readonly CombatManager cm;

    public EvaluatingCardsState(CombatManager cm)
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
