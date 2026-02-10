using UnityEngine;
using System.Collections;

public class EvaluatingCardsState : ITurnState
{
    private CombatManager _cm;

    public EvaluatingCardsState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        // Evaluate damage and defense of cards in hand
        _cm.StartCoroutine(DelayedEvaluationSequence());

        // Clear the hand and move to the next turn
        _cm.StartCoroutine(FinishEvaluationSequence());
    }

    private IEnumerator DelayedEvaluationSequence()
    {
        // Delay so that all cards are destroyed correctly (ex: during Shatter) before evaluating damage
        yield return new WaitForSeconds(2.0f);
        
        Debug.Log("Calculating total damage...");
        
        // Calculate damage from the remaining cards in hand
        // IMPLEMENT POISON CALC LATER
        int totalDamage = 0;
        int totalDefense = 0;
        foreach (var cardView in _cm.Hand.handCardViews)
        {
            if (cardView != null && cardView.data != null)
            {
                totalDamage += cardView.data.damage; // Summing the card values
                totalDefense += cardView.data.defense;
            }
        }

        // Apply damage to the enemy
        _cm.enemyHealth -= totalDamage;
        _cm.tempDefense += totalDefense;
        Debug.Log($"Dealt {totalDamage} damage! Enemy Health: {_cm.enemyHealth}. Gained {totalDefense} defense!");
    }

    private IEnumerator FinishEvaluationSequence()
    {
        // Give the player a moment to see the final cards
        // Be careful of reducing this time too much because DOTween won't have the time to animate the cards before you destroy the cards!
        yield return new WaitForSeconds(2.0f);

        // Recycle cards back to the deck and clear visuals
        _cm.Hand.ClearHand(); 
        
        // Reset Madness for the next turn
        _cm.madness = 0;

        // Check if the enemy is dead or move to Enemy Turn
        if (_cm.enemyHealth <= 0)
        {
            Debug.Log("Victory!"); // Victory logic would go here
        }
        else
        {
            _cm.MoveToNewState("EnemyTurn");
        }
    }

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}