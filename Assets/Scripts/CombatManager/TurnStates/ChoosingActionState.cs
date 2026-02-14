using UnityEngine;
using UnityEngine.InputSystem;

public class ChoosingActionState : ITurnState
{
    private CombatManager _cm;

    public ChoosingActionState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        Debug.Log("Alice's Turn: Choose to Hit or Stand.");
        // TURN ON UI buttons to hit or stand.
    }

    public void HandleInput(string inputID)
    {
        // Handle either Hit or Stand choices
        if (inputID == "HitButton") PerformHit();
        else if (inputID == "StandButton") PerformStand();
    }

    private void PerformHit()
    {
        // Yoink data from the deck
        CardData drawnData = _cm.Deck.DrawCard();

        if (drawnData != null)
        {
            // Store the data and create the card in your hand.
            _cm.lastDrawnCard = drawnData;
            
            CardView cardView = CardViewCreator.Instance.CreateCardView(drawnData, _cm.transform.position, Quaternion.identity);
            _cm.StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));
            
            _cm.MoveToNewState("HandlingCard");
        }
    }

    private void PerformStand()
    {
        // End turn and calculate damage
        _cm.MoveToNewState("EvaluatingCards");
    }

    public void Update()
    {
        // Keyboard shortcuts for testing
        // REPLACE WITH BUTTONS LATER ON
        if (Keyboard.current.spaceKey.wasPressedThisFrame) PerformHit();
        if (Keyboard.current.oKey.wasPressedThisFrame) PerformStand();
    }

    public void Exit()
    {
        // TURN OFF UI Buttons to prevent clicks when you're not choosing an action
    }
}