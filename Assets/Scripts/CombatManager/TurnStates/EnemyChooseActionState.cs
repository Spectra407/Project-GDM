using UnityEngine;
using System.Collections;

public class EnemyChooseActionState : ITurnState
{
    private CombatManager _cm;

    public EnemyChooseActionState(CombatManager cm)
    {
        _cm = cm;
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
        
        // RANDOMLY CHOOSE AN INTEGER BETWEEN 1-6
        // PLAY THE CHOSEN INTEGER'S DICE ANIMATION
        // UPDATE THE TOP LEFT UI WITH THE CORRECT DICE IMAGE
        
        // Wait another moment so the player sees the result
        yield return new WaitForSeconds(1.0f);

        // Give the turn back to Alice
        _cm.MoveToNewState("ChoosingAction"); 
    }
    
    

    public void HandleInput(string input) { } 
    public void Update() { }
    public void Exit() { }
}