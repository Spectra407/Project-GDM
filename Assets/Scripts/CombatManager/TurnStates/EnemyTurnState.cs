using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnemyTurnState : ITurnState
{
    private CombatManager _cm;

    public EnemyTurnState(CombatManager cm)
    {
        _cm = cm;
    }

    public void Enter()
    {
		_cm.turnBaseMadness = _cm.alice.startingMadness;	// Reset the external enemy madness at the start of the enemy's turn
        Debug.Log("Enemy is preparing to attack...");
        
        BannerManager.Instance.ShowBanner("Enemy Turn");
        
        _cm.StartCoroutine(ExecuteEnemyMove());
    }

    
    private IEnumerator ExecuteEnemyMove()
    {
        yield return new WaitForSeconds(1.5f);
        
        // ENEMY RESETS BLOCK
        _cm.enemyDefense = 0;
        
        // POISON TICK first, then pause before attack
        if (_cm.poison > 0)
        {
            AudioManager.instance.PlayPoisonDamage();
            
            _cm.enemyCurrentHealth -= _cm.poison;
            _cm.enemyCurrentHealth = Mathf.Max(0, _cm.enemyCurrentHealth);
            EnemyHealthBar.Instance.AnimateToCurrentHealth();
            Debug.Log($"Poison ticked for {_cm.poison}. Enemy health: {_cm.enemyCurrentHealth}");
            _cm.poison = (int)Math.Floor(_cm.poison / 2.0);
            Debug.Log($"Poison decayed to {_cm.poison}");

            // Wait for poison animation to finish before enemy attacks
            yield return new WaitForSeconds(1.0f);

            if (_cm.enemyCurrentHealth <= 0)
            {
                Debug.Log("Enemy defeated by poison!");
                yield return new WaitForSeconds(1.0f);
                _cm.MoveToNewState("Victory");
                yield break;
            }
        }
        
        // ENEMY ATTACKS: each move plays sequentially
        foreach (EnemyMove move in _cm.enemyChosenMoves)
        {
            PortraitAnimator.Instance.PlayEnemyAttack();
            yield return _cm.StartCoroutine(ExecuteMoveEffects(move));
            yield return new WaitForSeconds(0.8f);
        }
        
        yield return new WaitForSeconds(1.0f);

        if (_cm.currentHealth <= 0)
        {
            Debug.Log("Game Over: You died.");
			_cm.Deck._isInitialized = false;
			_cm.fadeScript.FadeOut();
            _cm.StartCoroutine(DelayedGameOverLoad("FirstFightCardSoldierScene"));
        }
        else
        {
            _cm.tempDefense = 0;
            _cm.MoveToNewState("EnemyChooseActionState");
        }
    }

    private IEnumerator ExecuteMoveEffects(EnemyMove move)
    {
        if (move.damage != 0)
        {
            Debug.Log($"The Card Soldier stabs Alice for {move.damage} + {_cm.enemyStrength} damage!");
            CombatAnimator.Instance.PlayEnemyDamageEffect();
            yield return new WaitForSeconds(0.35f); // wait for projectile to arrive
            AliceTakeDamage(move.damage + _cm.enemyStrength);
            AliceHealthBar.Instance.AnimateToCurrentHealth();
            Debug.Log($"Alice Health: {_cm.currentHealth}");
            yield return new WaitForSeconds(0.4f);
        }

        if (move.block != 0)
        {
            Debug.Log($"The Card Soldier gains {move.block} block!");
            _cm.enemyDefense += move.block;
            AudioManager.instance.PlayGainShield();
            CombatAnimator.Instance.PlayEnemyGainBlock(_cm.enemyDefense);
            yield return new WaitForSeconds(0.5f);
        }

        if (move.strength != 0)
        {
            Debug.Log($"The Card Soldier gains {move.strength} strength!");
            _cm.enemyStrength += move.strength;
            AudioManager.instance.PlayGainStrength();
            CombatAnimator.Instance.PlayEnemyGainStrength(_cm.enemyStrength);
            yield return new WaitForSeconds(0.5f);
        }

        if (move.madness != 0)
        {
            Debug.Log($"The Card Soldier inflicts {move.madness} madness!");
            _cm.turnBaseMadness += move.madness;
			_cm.madness += move.madness;
            _cm.OnMirrorCrack.Invoke();
            CombatAnimator.Instance.PlayMadnessEffect();
            yield return new WaitForSeconds(0.7f);
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

	private IEnumerator DelayedGameOverLoad(string sceneName)
	{
    	yield return new WaitForSeconds(3f); 
    	SceneManager.LoadScene(sceneName);
	}
    
    

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}