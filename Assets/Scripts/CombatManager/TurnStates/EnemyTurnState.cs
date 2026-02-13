using UnityEngine;
using System.Collections;

public class EnemyTurnState : ITurnState
{
    private CombatManager _cm;

    public EnemyTurnState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
        Debug.Log("Enemy is preparing to attack...");
        
        // Start a Coroutine via the CombatManager to handle the "thinking" time
        _cm.StartCoroutine(ExecuteEnemyMove());
    }

    private IEnumerator ExecuteEnemyMove()
    {
        // Wait a moment for animations or whatever
        yield return new WaitForSeconds(1.5f);

        // Simple Enemy Logic: Deal a flat 10 damage for now
        // REPLACE WITH THE ENEMYDATA LATER
        int damageToAlice = 10;
        Debug.Log($"The Card Soldier stabs Alice for {damageToAlice} - {_cm.tempDefense} damage!");
        AliceTakeDamage(damageToAlice);
        Debug.Log($"Alice Health: {_cm.currentHealth}");
        
        // Wait another moment so the player sees the result
        yield return new WaitForSeconds(1.0f);

        // Check if Alice is defeated or move back to her turn
        if (_cm.currentHealth <= 0)
        {
            Debug.Log("Game Over: You died.");
            // Move to a GameOverState LATERRRR
        }
        else
        {
            // Reset the turn back to Alice
            _cm.MoveToNewState("ChoosingAction"); 
            // This should be updated to go to EnemyChooseAction
        }
    }
    
    private void AliceTakeDamage(int damage)
    {
        if (_cm.tempDefense >= damage)
        {
            // Defense big enough to tank full hit
            _cm.tempDefense -= damage;
        }
        else
        {
            // Defense not big enough to tank full hit
            damage -= _cm.tempDefense;
            _cm.tempDefense = 0;
            _cm.currentHealth -= damage;
        }
        // Stop negative health values
        _cm.currentHealth = Mathf.Max(0, _cm.currentHealth);
        
        // Update the AliceData ScriptableObject to keep health persistent
        _cm.alice.currentHealth = _cm.currentHealth;
    }

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}