public class HandlingCardState : ITurnState
{
    
    private CombatManager cm;
    private Card drawnCard;
    private Phase phase;

    public HandlingCardState(CombatManager cm)
    {
        this.cm = cm;
    }

    public void Enter()
    {
        if (phase == Phase.Start)
        {
            phase = Phase.CheckingPeek;
            drawnCard = cm.drawnCards[-1];
        }

        if (phase == Phase.CheckingPeek)
        {
            phase = Phase.CheckingMadness;
            // Check peek
        }

        if (phase == Phase.CheckingMadness)
        {
            phase = Phase.Start;

            if (cm.madness > cm.maxMadness)
            {
                cm.ChangeState("ChoosingCard");
                return;
            }
        }
    }

    public void Exit()
    {
        
    }

    public void Update(float dt)
    {
        
    }

    public void HandleInput()
    {
        
    }

    enum Phase
    {
        Start,
        CheckingMadness,
        CheckingPeek,
    }
}