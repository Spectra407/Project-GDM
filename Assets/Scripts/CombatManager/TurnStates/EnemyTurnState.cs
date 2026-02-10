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
        // 1. Wait a moment for visual pacing
        yield return new WaitForSeconds(1.5f);

        // 2. Simple AI Logic: Deal a flat 10 damage for now
        int damageToAlice = 10;
        _cm.currentHealth -= damageToAlice;
        
        Debug.Log($"The Jabberwock bites Alice for {damageToAlice} damage!");
        Debug.Log($"Alice Health: {_cm.currentHealth}");

        // 3. Update the AliceData ScriptableObject to keep health persistent
        _cm.alice.currentHealth = _cm.currentHealth;

        // 4. Wait another moment so the player sees the result
        yield return new WaitForSeconds(1.0f);

        // 5. Check if Alice is defeated or move back to her turn
        if (_cm.currentHealth <= 0)
        {
            Debug.Log("Game Over: You died.");
            // Move to a GameOverState if you have one
        }
        else
        {
            // Reset the cycle back to Alice
            _cm.MoveToNewState("ChoosingAction"); 
        }
    }

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}