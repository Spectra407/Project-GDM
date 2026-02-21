using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyChooseActionState : ITurnState
{
    private CombatManager _cm;
    private DiceManager _diceManager;


    public EnemyChooseActionState(CombatManager cm, DiceManager diceManager)
    {
        _cm = cm;
        _diceManager = diceManager;
    }

    public void Enter()
    {
        Debug.Log("Enemy is choosing an action...");
        
        // Start a Coroutine via the CombatManager to handle the "thinking" time
        _cm.StartCoroutine(EnemyChoose());
    }

    private IEnumerator EnemyChoose()
    {
        // Wait a moment for animations or whatever
        yield return new WaitForSeconds(1.5f);
        
        yield return _cm.StartCoroutine(
            _diceManager.RollDice(result =>
            {
            _cm.enemyIndexMove = result;
            })
        );
    

        _cm.enemyChosenMove = _cm.enemy.moves[_cm.enemyIndexMove];
        Debug.Log("Chosen move number" + _cm.enemyIndexMove);

        // CALL var (_cm.enemyChosenMove, _cm.enemyIndexMove) = ChooseEnemyMove();
        // UPDATE enemyChosenMove to this new move and enemyIndexMove to this new index, used to easily call PlayDiceAnimation(2) or smtn for all the UI elements.
        
        // PLAY THE CHOSEN INTEGER'S DICE ANIMATION, for now use a placeholder video or smtn
        // UPDATE THE TOP LEFT UI WITH THE CORRECT DICE IMAGE
        // DISPLAY THE DESCRIPTION OF THE CURRENT DICE ATTACK ON THE RIGHT OF THE DICE IMAGE
        
        // Wait another moment so the player sees the result
        yield return new WaitForSeconds(1.0f);

        // Give the turn back to Alice
        _cm.MoveToNewState("ChoosingAction"); 
    }


    
    

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}