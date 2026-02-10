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
        // 1. Enable UI Buttons (e.g., Hit/Stand buttons become interactable)
        // UIManager.Instance.ShowCombatButtons(true);
    }

    public void HandleInput(string inputID)
    {
        // 2. Handle either Hit or Stand choices
        // These strings must match exactly what Alice clicks in the Inspector
        if (inputID == "HitButton")
        {
            CardData drawnData = _cm.Deck.DrawCard();
            if (drawnData != null)
            {
                _cm.lastDrawnCard = drawnData;
                _cm.MoveToNewState("HandlingCard");
            }
        }
        else if (inputID == "StandButton")
        {
            _cm.MoveToNewState("EvaluatingCards");
        }
    }

    private void PerformHit()
    {
        // 3. Draw data from the deck
        CardData drawnData = _cm.Deck.DrawCard();

        if (drawnData != null)
        {
            // 4. Store the data and move to validation
            _cm.lastDrawnCard = drawnData;
            
            CardView cardView = CardViewCreator.Instance.CreateCardView(drawnData, _cm.transform.position, Quaternion.identity);
            _cm.StartCoroutine(HandView.Instance.AnimateCardToHand(cardView));
            
            _cm.MoveToNewState("HandlingCard");
        }
    }

    private void PerformStand()
    {
        // 5. End the turn and calculate damage
        _cm.MoveToNewState("EvaluatingCards");
    }

    public void Update()
    {
        // Optional: Keyboard shortcuts for testing
        if (Keyboard.current.spaceKey.wasPressedThisFrame) PerformHit();
        if (Keyboard.current.oKey.wasPressedThisFrame) PerformStand();
    }

    public void Exit()
    {
        // 6. Disable UI Buttons to prevent clicks during animations
        // UIManager.Instance.ShowCombatButtons(false);
    }
}