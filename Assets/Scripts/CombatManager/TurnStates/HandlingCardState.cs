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
        // 1. Safety Guard: Exit if there's no card data to process
        if (_cm.lastDrawnCard == null)
        {
            _cm.MoveToNewState("ChoosingAction");
            return;
        }

        // 2. Logic Branching
        if (_currentPhase == Phase.Start)
        {
            ProcessInitialMadness();
        }
        else if (_currentPhase == Phase.CheckingPeek)
        {
            ProcessPeekOrFinish();
        }
    }

    private void ProcessInitialMadness()
    {
        _cm.madness += _cm.lastDrawnCard.madness;

        if (_cm.madness >= _cm.alice.maxMadness)
        {
            // We do NOT clear lastDrawnCard here yet; let the Shatter state handle it
            _cm.StartCoroutine(DelayedShatter());
        }
        else
        {
            // Ensure this phase change is explicit
            _currentPhase = Phase.CheckingPeek;
            ProcessPeekOrFinish();
        }
    }

    private IEnumerator DelayedShatter()
    {
        yield return new WaitForEndOfFrame(); // A safer wait than null
        _cm.MoveToNewState("Shattering");
    }

    private void ProcessPeekOrFinish()
    {
        // 3. Check if card has a Peek value
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
    
    

    public void HandleInput(string inputID) { } // Handled by ChoosingAction or UI
    public void Update() { }      // No frame logic needed here
    public void Exit() 
    {
        // If we are moving back to ChoosingAction, ensure the card data is wiped
        if (_currentPhase == Phase.Done)
        {
            _cm.lastDrawnCard = null; // Prevent double-processing
        }
    }
}