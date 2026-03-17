using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

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
        
        BannerManager.Instance.ShowBanner("Enemy Choosing New Action ->");
        
        // Start a Coroutine via the CombatManager to handle the "thinking" time
        _cm.StartCoroutine(EnemyChoose());
    }

    private IEnumerator EnemyChoose()
    {
        // Wait a moment for animations or whatever
        yield return new WaitForSeconds(1.5f);

        _cm.enemyChosenMoves.Clear();
        string combinedText = "";

        for (int i = 0; i < _cm.enemy.attackCount; i++)
        {
            if (i > 0) yield return new WaitForSeconds(0.5f);

            int rolledIndex = 0;
            yield return _cm.StartCoroutine(
                _diceManager.RollDice(result => rolledIndex = result)
            );

            _cm.enemyChosenMoves.Add(_cm.enemy.moves[rolledIndex]);
            Debug.Log("Chosen move number " + rolledIndex);

            combinedText += (i > 0 ? "\n" : "") + _cm.enemy.moveText[rolledIndex];
        }
        
        // DISPLAY THE DESCRIPTION OF THE ATTACK
        _cm.moveText.text = combinedText;
        
        // Wait another moment so the player sees the result
        yield return new WaitForSeconds(1.0f);
        
        BannerManager.Instance.ShowBanner("Your Turn");
        yield return new WaitForSeconds(1.5f);

        // Give the turn back to Alice
        _cm.MoveToNewState("ChoosingAction"); 
    }


    
    

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}