using UnityEngine;

public class PeekingState : ITurnState
{
    private CombatManager _cm;

    public PeekingState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        // 1. Get the peek count from the card Alice just drew
        int peekCount = _cm.lastDrawnCard.peek;
        
        Debug.Log($"Peeking State Entered: Showing top {peekCount} cards.");

        // 2. Tell the PeekManager to open the UI and populate it
        // PeekManager is likely a Singleton based on your previous logs
        PeekManager.Instance.ShowPeek(peekCount); 
    }

    public void HandleInput(string inputID)
    {
        // 3. Usually, the PeekManager's UI buttons handle the selection.
        // However, if Alice clicks a 'Cancel' button, we return here.
        if (inputID == "ClosePeek")
        {
            _cm.ReturnToLastState(); 
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Alice clicks
        {
            DetectPeekClick();
        }
    }
    
    private void DetectPeekClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
        // Ensure we only hit the 'Ignore Raycast' layer if that's where your UI is
        // OR just use a standard raycast if they are on the Default layer
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Find the CardView on the object we hit
            CardView clickedCard = hit.collider.GetComponentInParent<CardView>();
        
            if (clickedCard != null)
            {
                // Find which index this card represents in the PeekManager
                int index = PeekManager.Instance.GetIndexOfCard(clickedCard);
                if (index != -1)
                {
                    PeekManager.Instance.OnCardSelected(index);
                }
            }
        }
    }

    public void Exit()
    {
        // 4. Ensure the UI is hidden when we leave this state
        PeekManager.Instance.ClosePeek(); 
        Debug.Log("Exiting Peeking State.");
    }
}