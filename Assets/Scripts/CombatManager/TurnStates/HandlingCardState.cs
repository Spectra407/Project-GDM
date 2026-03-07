using System.Collections;
using UnityEngine;

public class HandlingCardState : ITurnState
{
    private CombatManager _cm;
    public bool isShufflePending = false;
    
    // Tracks where we are in the processing of a single card
    private enum Phase { Start, CheckingPeek, Done }
    private Phase _currentPhase = Phase.Start;

    public HandlingCardState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        _currentPhase = Phase.Start;
        Debug.Log("Current phase is " + _currentPhase);
        _cm.StartCoroutine(EnterNextFrame());
    }

    private IEnumerator EnterNextFrame()
    {
        yield return null; // wait one frame for lastDrawnCard to be set by DrawOne
    
        if (_cm.lastDrawnCard == null)
        {
            _cm.MoveToNewState("ChoosingAction");
            yield break;
        }
        ProcessDraw();
    }

    private void ProcessDraw()
    {
        int madness = _cm.lastDrawnCard.madness;
        _cm.madness += madness;   // Increment Madness
        if (madness > 0) _cm.OnMirrorCrack.Invoke();
        
        if (_cm.madness > _cm.alice.maxMadness)     // Check for Shatter
        {
            // Trigger Shatter.
            _cm.dem.ResetForStand();    // Reset all pending. We will "redo" the resolve on draw for the survivor inside ShatteringState. 
            _cm.StartCoroutine(DelayedShatter());
        }
        else
        {
            _cm.dem.ProcessJackpot();
            // Resolve normal On Draw effects
            _cm.dem.ResolveOnDraw(_cm.lastDrawnCard);
            
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
        if (isShufflePending) return;
        
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

    public void FinishTurn()
    {
        Debug.Log($"FinishTurn called. pendingDraws = {_cm.pendingDraws}, isDrawing = {_cm.isDrawing}");
        _currentPhase = Phase.Done;
        _cm.lastDrawnCard = null;
    
        if (_cm.pendingDraws > 0)
            DrawEffect.DrawNext(_cm); // Draw the next queued card
        else
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