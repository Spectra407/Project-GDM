using UnityEngine;
using System.Collections.Generic;

// WIP
// Script to handle tracking and updating data during battles, including deck operations
// and evaluating card effects (not yet implemented).
// If anything seems missing or stupid say something! I have no idea what I'm doing <3 
// -Elle

public class CombatManager : MonoBehaviour // What is a MonoBehaviour? Should this be one?
{
    public int playerHealth;
    public int enemyHealth;
    public int madness;
    public int maxMadness;
    public DeckManager deck;
    public List<Card> drawnCards;
    public Card lastDrawnCard;

    private Dictionary<string, ITurnState> states;
    private ITurnState currentState;
    // private EnemyAI enemyAI;

    // It's probably gonna be a good idea to turn this into a full-fledged state machine
    // but for right now this can be an outline of states we want
    // enum TurnState
    // {
    //     DrawingCards,
    //     HandlingCardEffect, // if cards might require immediate action by the player
    //     ChoosingCardToPlay, // if madness threshold is passed
    //     InAnimation,
    //     EnemyTurn
    // }

    void Start()
    {
        states = new();
        states.Add("ChoosingAction", new ChoosingActionState(this));
        states.Add("HandlingCard", new HandlingCardState(this));
        states.Add("Peeking", new PeekingState(this));
        states.Add("ChoosingCard", new ChoosingCardState(this));
    }

    void Update()
    {
        // Send input to current state if available
        // Call Update in current state
    }

    public void ChangeState(string id)
    {
        currentState.Exit();
        currentState = states[id];
        currentState.Enter();
    }

    // void InitBattle(List<Card> deck, Enemy enemy, OtherAliceStats aliceStats(?), idk what else)
    void InitBattle(int playerHealth, List<Card> deck)
    {
        this.playerHealth = playerHealth;
    }

    void EndBattle()
    {
        // Probably broadcast the result to some state object idk
    }
}