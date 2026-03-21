using UnityEngine;
using UnityEngine.InputSystem;

public class ChoosingActionState : ITurnState
{
    private CombatManager _cm;
    private bool _canDraw = true;
    private const float DrawCooldown = 0.8f;

    public ChoosingActionState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        _canDraw = true;
        Debug.Log("Alice's Turn: Choose to Hit or Stand.");
        // TURN ON UI buttons to hit or stand.
        CombatButton.SetDrawInteractable(true);
        CombatButton.SetStandInteractable(true);
    }

    public void HandleInput(string inputID)
    {
        // Handle either Hit or Stand choices
        if (inputID == "HitButton") PerformHit();
        else if (inputID == "StandButton") PerformStand();
    }

    private void PerformHit()
    {
        if (!_canDraw) return;
        _canDraw = false;
        CombatButton.SetDrawInteractable(false);
        
        
        _cm.pendingDraws += 1;
        DrawEffect.DrawNext(_cm);   
        // Reusing DrawEffect.cs to simulate a Draw 1, ported over old system here into DrawEffect.cs
    }

    private void PerformStand()
    {
        CombatButton.SetStandInteractable(false);
        CombatButton.SetDrawInteractable(false);
        
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
        CombatButton.SetDrawInteractable(false);
        CombatButton.SetStandInteractable(false);
    }
}