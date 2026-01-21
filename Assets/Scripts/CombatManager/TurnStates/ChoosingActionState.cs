using UnityEngine;
using System.Collections.Generic;

public class ChoosingActionState : ITurnState
{
    private CombatManager cm;

    public ChoosingActionState(CombatManager cm)
    {
        this.cm = cm;
    }

    public void Update(float dt)
    {
        
    }

    public void HandleInput()
    {
        
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    private void DrawCard()
    {
        Card? maybeCard = cm.deck.DrawTopCard();
        if (maybeCard is null) return;
        Card card = (Card) maybeCard;

        cm.drawnCards.Add(card);
        cm.lastDrawnCard = card;
        cm.madness += card.madness;

        cm.ChangeState("HandlingCard");
    }

    private void StopDrawing()
    {
        cm.ChangeState("Evaluating");
    }
}
