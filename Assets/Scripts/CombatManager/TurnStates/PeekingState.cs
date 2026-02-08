using UnityEngine;
using System.Collections.Generic;

public class PeekingState : ITurnState
{
    private CombatManager cm;

    private List<Card> peeked;

    public PeekingState(CombatManager cm)
    {
        this.cm = cm;
    }

    public void Enter()
    {
        peeked = cm.deck.PeekNCards(2);
        Debug.Log("Peeked! Selecting 0th card...");
        SelectCard(0);
    }

    public void HandleInput(string inputID)
    {
        
    }

    private void SelectCard(int n)
    {
        Card? shouldBeCard = cm.deck.DrawNthCard(n);
        if (shouldBeCard == null) return;
        Card card = (Card) shouldBeCard;

        cm.drawnCards.Add(card);
        cm.lastDrawnCard = card;

        cm.ReturnToLastState();
    }
}
