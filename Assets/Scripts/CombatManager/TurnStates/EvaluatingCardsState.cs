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
        _cm.StartCoroutine(EvaluationSequence());
    }

    private IEnumerator EvaluationSequence()
    {
        // Short delay before resolution starts
        yield return new WaitForSeconds(0.5f);
    
        Debug.Log("Calculating total damage...");
    
        // Wait for the full card-by-card animation to complete
        yield return _cm.StartCoroutine(_cm.sem.ResolveOnStandAnimated());
    
        // Only runs after every card has animated and resolved
        yield return new WaitForSeconds(0.5f);
    
        _cm.Hand.ClearHand();
        _cm.madness = 0;

        if (_cm.enemyCurrentHealth <= 0)
        {
            Debug.Log("Victory!");
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