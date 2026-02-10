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
        // Get the peek count from the card Alice just drew
        int peekCount = _cm.lastDrawnCard.peek;
        
        Debug.Log($"Peeking State Entered: Showing top {peekCount} cards.");

        // Tell the PeekManager to open the UI and instantiate it
        PeekManager.Instance.ShowPeek(peekCount); 
    }

    public void HandleInput(string inputID)
    {
        
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
        // Close the Peek UI when we leave this state
        PeekManager.Instance.ClosePeek(); 
        Debug.Log("Exiting Peeking State.");
    }
}