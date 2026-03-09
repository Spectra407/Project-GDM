using UnityEngine;
using System.Collections;
using Systems;

public class EvaluatingCardsState : ITurnState
{
    private CombatManager _cm;

    public EvaluatingCardsState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        _cm.pendingDraws = 0;      
        _cm.isDrawing = false;
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

        _cm.sem.ResolveOnStand();
        
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
        if (_cm.enemyCurrentHealth <= 0)
        {
            Debug.Log("Victory!"); // Victory logic would go here
            _cm.MoveToNewState("Victory");
        }
        else
        {
            _cm.MoveToNewState("EnemyTurn");
        }
    }
    
    private void EnemyTakeDamage(int damage)
    {
        if (_cm.enemyDefense >= damage)
        {
            // Defense big enough to tank full hit
            _cm.enemyDefense -= damage;
        }
        else
        {
            // Defense not big enough to tank full hit
            damage -= _cm.enemyDefense;
            _cm.enemyDefense = 0;
            _cm.enemyCurrentHealth -= damage;
        }
        // Stop negative health values
        _cm.enemyCurrentHealth = Mathf.Max(0, _cm.enemyCurrentHealth);
    }

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}