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
            phase = Phase.CheckingMadness;
            drawnCard = cm.lastDrawnCard;
        }

        if (phase == Phase.CheckingMadness)
        {
            phase = Phase.CheckingPeek;

            if (cm.madness > cm.maxMadness)
            {
                cm.ChangeState("ChoosingCard");
                return;
            }
        }

        if (phase == Phase.CheckingPeek)
        {
            phase = Phase.Start;

            bool hasPeek = true; // todo: replace w/ real check
            if (hasPeek)
            {
                cm.ChangeState("Peeking");
                return;
            }
        }

        // Animations here?
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