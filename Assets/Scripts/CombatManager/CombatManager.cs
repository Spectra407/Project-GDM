using UnityEngine;
using System.Collections.Generic;

// WIP
// Script to handle tracking and updating data during battles, including deck operations
// and evaluating card effects (latter not yet implemented).
// If anything seems missing or stupid say something! I have no idea what I'm doing <3 
// -Elle

public class CombatManager : MonoBehaviour
{
    public int playerHealth;
    public int enemyHealth;
    public int madness;
    public int maxMadness;
    public Card lastDrawnCard;

    private Stack<ITurnState> states;
    private ITurnState currentState;
    // private EnemyAI enemyAI;

    void Start()
    {
        drawnCards = new();

        states = new();
        states.Push(new ChoosingActionState(this));
    }

    void Update()
    {
        
    }

    public void HandleInput(string input)
    {
        Debug.Log("CombatManager received input \"" + input + "\"");
        states.Peek().HandleInput(input);
    }

    public void MoveToNewState(string id)
    {
        Debug.Log("CombatManager moving to state " + id);
        states.Push(NewState(id));
        states.Peek().Enter();
    }

    public void ReturnToLastState()
    {
        Debug.Log("CombatManager returning to last state");
        states.Pop();
        states.Peek().Enter();
    }

    private ITurnState NewState(string id)
    {
        switch (id)
        {
            case "ChoosingAction": return new ChoosingActionState(this);
            case "HandlingCard": return new HandlingCardState(this);
            case "Peeking": return new PeekingState(this);
            case "ChoosingCard": return new ChoosingCardState(this);
            case "EvaluatingCards": return new EvaluatingCardsState(this);
            default: throw new System.Exception();
        }
    }

    void EndBattle()
    {
        // Probably broadcast the result to some state object idk
    }
}