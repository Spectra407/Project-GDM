using System.Collections;
using UnityEngine;

public class HandlingCardState : ITurnState
{
    private CombatManager _cm;
    
    // Tracks where we are in the processing of a single card
    private enum Phase { Start, CheckingPeek, Done }
    private Phase _currentPhase = Phase.Start;

    public HandlingCardState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        // Exit if there's no card data to process (ex: Draw an empty deck)
        if (_cm.lastDrawnCard == null)
        {
            _cm.MoveToNewState("ChoosingAction");
            return;
        }
        
        if (_currentPhase == Phase.Start)
        {
            _cm.dem.ResolveOnDraw();
            FinishTurn();
            // peek, checking madness, etc should happen here so might just simplify the rest
            // then need to finish turn and stuff?    
            // ProcessInitialMadness();
        }
        else if (_currentPhase == Phase.CheckingPeek)
        {
            ProcessPeekOrFinish();
        }
    }

    private void ProcessInitialMadness()
    {
        _cm.madness += _cm.lastDrawnCard.madness;

        if (_cm.madness > _cm.alice.maxMadness)
        {
            // Trigger Shatter.
            _cm.StartCoroutine(DelayedShatter());
        }
        else
        {
            // Check if we peek
            _currentPhase = Phase.CheckingPeek;
            ProcessPeekOrFinish();
        }
    }

    private IEnumerator DelayedShatter()
    {
        yield return new WaitForEndOfFrame(); 
        _cm.MoveToNewState("Shattering");
    }

    private void ProcessPeekOrFinish()
    {
        // Check if card has a Peek value
        if (_cm.lastDrawnCard.peek > 0)
        {
            // Use Push instead of Move so we return here after the Peek
            _cm.PushNewState("Peeking");
        }
        else
        {
            FinishTurn();
        }
    }

    private void FinishTurn()
    {
        _currentPhase = Phase.Done;
        _cm.lastDrawnCard = null; // Clear the data for the next draw
        _cm.MoveToNewState("ChoosingAction");
    }
    
    

    public void HandleInput(string inputID) { } 
    public void Update() { }      
    public void Exit() 
    {
        // If we are moving back to ChoosingAction, ensure the card data is wiped
        if (_currentPhase == Phase.Done)
        {
            _cm.lastDrawnCard = null; 
        }
    }
}