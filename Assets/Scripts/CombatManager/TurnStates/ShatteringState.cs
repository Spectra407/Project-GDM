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
        Debug.Log("MADNESS LIMIT REACHED! Alice must choose a survivor.");
        _hasSelected = false;
        
        // Optional: Trigger a visual "Danger" UI or sound
        // UIManager.Instance.ShowShatterPrompt(true);
        
        _cm.StartCoroutine(EnableSelectionDelay());
    }

    public void Update()
    {
        // 1. Listen for the mouse click to pick a survivor
        if (!_hasSelected && _canSelect && Input.GetMouseButtonDown(0))
        {
            DetectSurvivorClick();
        }
    }

    private void DetectSurvivorClick()
    {
        // 2. Raycast from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"Shatter Raycast hit: {hit.collider.gameObject.name}");
            // 3. Check if the hit object is a CardView
            CardView clickedCard = hit.collider.GetComponentInParent<CardView>();

            if (clickedCard != null)
            {
                _hasSelected = true;
                Debug.Log($"Survivor chosen: {clickedCard.data.cardName}");
                
                // 4. Start the physical destruction of the other cards
                _cm.Hand.StartCoroutine(_cm.Hand.ShatterSequence(clickedCard));
                
                // 5. Move to evaluation now that only 1 card remains
                _cm.MoveToNewState("EvaluatingCards");
            }
        }
    }

    public void HandleInput(string input) { } // Clicks are handled via Raycast in Update
    
    public void Exit() 
    {
        // UIManager.Instance.ShowShatterPrompt(false);
    }
}