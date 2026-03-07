using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteringState : ITurnState
{
    private CombatManager _cm;
    private bool _hasSelected = false;
    private bool _canSelect = false;

    public ShatteringState(CombatManager cm)
    {
        _cm = cm;
    }
    
    private IEnumerator EnableSelectionDelay()
    {
        yield return new WaitForSeconds(0.5f); // Half-second safety buffer
        _canSelect = true;
    }

    public void Enter()
    {
        _cm.pendingDraws = 0;     
        _cm.isDrawing = false;
        
        Debug.Log("MADNESS LIMIT REACHED! Alice must choose a survivor.");
        _hasSelected = false;
        
        // TURN ON UI FOR SHATTER EFFECTS HERE
        
        _cm.StartCoroutine(EnableSelectionDelay());
    }

    public void Update()
    {
        // Listen for the mouse click to pick a survivor
        if (!_hasSelected && _canSelect && Input.GetMouseButtonDown(0))
        {
            DetectSurvivorClick();
        }
    }

    private void DetectSurvivorClick()
    {
        // Raycast from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 2f);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Shatter Raycast hit: {hit.collider.gameObject.name}");
            // Check if the hit object is a CardView
            CardView clickedCard = hit.collider.GetComponentInParent<CardView>();

            if (clickedCard != null)
            {
                _hasSelected = true;
                _cm.lastDrawnCard = null; // Clear last drawn card so it doesn't retrigger again in the next state.
                Debug.Log($"Survivor chosen: {clickedCard.data.cardName}");
                
                // Byebye other cards
                _cm.Hand.StartCoroutine(_cm.Hand.ShatterSequence(clickedCard));
                
                // Move to EvaluatingCards
                _cm.dem.ResolveOnDraw(clickedCard.data);
                _cm.MoveToNewState("EvaluatingCards");
            }
        }
        else
        {
            Debug.Log("Raycast hit nothing.");
        }
    }

    public void HandleInput(string input) { } 
    
    public void Exit() 
    {
        // TURN OFF THE UI POPUPS OR WHATEVER FOR SHATTER HERE
    }
}