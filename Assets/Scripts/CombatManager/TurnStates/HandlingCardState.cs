public class HandlingCardState : ITurnState
{
    
    private CombatManager cm;
    private Card drawnCard;
    private Phase phase;

    public HandlingCardState(CombatManager cm)
    {
        this.cm = cm;
        phase = Phase.Start;
    }

    public void Enter()
    {
        if (phase == Phase.Start)
        {
            phase = Phase.CheckingPeek;
            drawnCard = cm.lastDrawnCard;

            cm.madness += drawnCard.madness;
            if (cm.madness > cm.maxMadness)
            {
                cm.MoveToNewState("ChoosingCard");
                return;
            }
        }

        if (phase == Phase.CheckingPeek)
        {
            phase = Phase.ProcessingCardFurther;

            bool hasPeek = true; // todo: replace w/ real check
            if (hasPeek)
            {
                cm.MoveToNewState("Peeking");
                return;
            }
        }

        if (phase == Phase.ProcessingCardFurther)
        {
            phase = Phase.Done;

            cm.ReturnToLastState();
        }
    }

    public void HandleInput(string inputID)
    {
        
    }

    enum Phase
    {
        Start,
        CheckingPeek,
        ProcessingCardFurther,
        Done
    }
}