using UnityEngine;
using System.Collections;
using Unity.VisualScripting.Dependencies.Sqlite;

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

    // private (EnemyMove move, int indexMove) ChooseEnemyMove()
    // {
    //     EnemyMove[] moves = _cm.enemy.moves;
    //     int index = Random.Range(0, moves.Length);
    //     Debug.Log("Chosen move number " + index);
        
    //     return (moves[index], index);
    // }

    private IEnumerator ExecuteEnemyMove()
    {
        // Wait a moment for animations or whatever
        // CALL THE ANIMATION
        yield return new WaitForSeconds(1.5f);
        
        // ENEMY RESETS BLOCK
        _cm.enemyDefense = 0;
        
        // ENEMY CHOOSES ATTACK (PUT THIS INTO SEPARATE STATE LATER WITH ANIMATIONS)
        // var (move, index) = ChooseEnemyMove();
        // Debug.Log("Enemy has chosen a move.");
        
        // ENEMY ATTACKS
        ExecuteMoveEffects(_cm.enemyChosenMove);   // Replace move with _cm.enemyChosenMove
        
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
            _cm.tempDefense = 0;
            _cm.MoveToNewState("EnemyChooseActionState"); 
            // This should be updated to go to EnemyChooseAction
        }
    }
    
    private void ExecuteMoveEffects (EnemyMove move)
    {
        if (move.damage != 0)
        {
            Debug.Log($"The Card Soldier stabs Alice for {move.damage} + {_cm.enemyStrength} - {_cm.tempDefense} damage!");
            AliceTakeDamage(move.damage + _cm.enemyStrength);
            Debug.Log($"Alice Health: {_cm.currentHealth}");
        }
        
        if (move.block != 0)
        {
            Debug.Log($"The Card Soldier gains {move.block} block!");
            _cm.enemyDefense += move.block;
        }
        
        if (move.strength != 0)
        {
            Debug.Log($"The Card Soldier gains {move.strength} strength!");
            _cm.enemyStrength += move.strength;
        }
        
        if (move.madness != 0)
        {
            Debug.Log($"The Card Soldier inflicts {move.madness} madness upon you!");
            _cm.madness += move.madness;
            _cm.OnMirrorCrack.Invoke();     // Invoke mirror crack sfx
        }
    }

    private void AliceTakeDamage(int damage)
    {
        // Stop negative block values
        _cm.tempDefense = Mathf.Max(0, _cm.tempDefense);
        
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
        
        if (damage > 0) _cm.OnTakeDamage.Invoke();  // Invoke take damage sfx
    }

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}