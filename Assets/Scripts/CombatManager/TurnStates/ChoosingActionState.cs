using UnityEngine;
using System.Collections.Generic;

public class ChoosingActionState : ITurnState
{
    private CombatManager cm;

    public ChoosingActionState(CombatManager cm)
    {
        this.cm = cm;
    }

    public void HandleInput(string inputID)
    {
        if (inputID == "HitButton")
        {
            Debug.Log("drawing card");
            DrawCard();
        }
        else if (inputID == "StandButton")
        {
            Debug.Log("stoppig drawing");
            StopDrawing();
        }
    }

    public void Enter()
    {
        
    }

    // public void Exit()
    // {
        
    // }

    private void DrawCard()
    {
        Card? maybeCard = cm.deck.DrawTopCard();
        if (maybeCard is null) return;
        Card card = (Card) maybeCard;

        Debug.Log("drew card");


        cm.drawnCards.Add(card);
        cm.lastDrawnCard = card;

        cm.MoveToNewState("HandlingCard");
    }

    private void StopDrawing()
    {
        cm.MoveToNewState("Evaluating");
    }
}
