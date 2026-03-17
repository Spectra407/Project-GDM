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
        
        BannerManager.Instance.ShowBanner("Enemy Turn");
        
        _cm.StartCoroutine(ExecuteEnemyMove());
    }

    
    private IEnumerator ExecuteEnemyMove()
    {
        // Wait a moment for animations or whatever
        yield return new WaitForSeconds(1.5f);
        
        // ENEMY RESETS BLOCK
        _cm.enemyDefense = 0;
        
        
        // ENEMY ATTACKS
        foreach (EnemyMove move in _cm.enemyChosenMoves)
        {
            ExecuteMoveEffects(move);
            PortraitAnimator.Instance.PlayEnemyAttack();
            yield return new WaitForSeconds(0.5f);
        }
        
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
            AudioManager.instance.PlayGainShield();
        }
        
        if (move.strength != 0)
        {
            Debug.Log($"The Card Soldier gains {move.strength} strength!");
            _cm.enemyStrength += move.strength;
            AudioManager.instance.PlayGainStrength();
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
            AudioManager.instance.PlayBluntDamage();
        }
        else
        {
            // Defense not big enough to tank full hit
            damage -= _cm.tempDefense;
            _cm.tempDefense = 0;
            _cm.currentHealth -= damage;
            _cm.OnTakeDamage.Invoke();  // Invoke take damage sfx, sharper sound
        }
        // Stop negative health values
        _cm.currentHealth = Mathf.Max(0, _cm.currentHealth);
        
        // Update the AliceData ScriptableObject to keep health persistent
        _cm.alice.currentHealth = _cm.currentHealth;
        
        if (damage > 0)
        {
            PortraitAnimator.Instance.PlayAliceHit();
        }
    }

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}