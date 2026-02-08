using UnityEngine;
using System.Collections.Generic;

public class PeekingState : ITurnState
{
    private CombatManager cm;

    private int numCards;
    private List<Card> peeked;

    public PeekingState(CombatManager cm)
    {
        this.cm = cm;
    }

    public void Enter()
    {
        numCards = cm.deck.CardCount(); // min with peek value
    }

    public void Exit()
    {
        cm.deck.ShuffleUndrawn();
    }

    public void Update(float dt)
    {
        
    }

    public void HandleInput()
    {
        
    }

    private void SelectCard(int n)
    {
        
    }
}
