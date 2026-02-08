using UnityEngine;
using System.Collections.Generic;

// WIP
// Script to handle tracking and updating data during battles, including deck operations
// and evaluating card effects (not yet implemented).
// If anything seems missing or stupid say something! I have no idea what I'm doing <3 
// -Elle

public class CombatManager : MonoBehaviour // What is a MonoBehaviour? Should this be one?
{
    private TurnState turnState;
    private int playerHealth; // maybe this should be some sort of HealthManager
    private int enemyHealth;
    private int madness;
    private int maxMadness;

    private DeckManager deck;
    private List<Card> drawnCards;
    // private EnemyAI enemyAI;

    // It's probably gonna be a good idea to turn this into a full-fledged state machine
    // but for right now this can be an outline of states we want
    enum TurnState
    {
        DrawingCards,
        HandlingCardEffect, // if cards might require immediate action by the player
        ChoosingCardToPlay, // if madness threshold is passed
        EnemyTurn
    }

    // void InitBattle(List<Card> deck, Enemy enemy, OtherAliceStats aliceStats(?), idk what else)
    void InitBattle()
    {
        // set starting values
    }

    void BeginPlayerTurn()
    {
        turnState = TurnState.DrawingCards;

        // Do any other stuff that needs doing
    }

    void EvaluateAllCards()
    {
        // foreach (Card card in drawnCards)
        // {
        //     // Do stuff
        // }
    }

    void EvaluateChosenCard(Card card)
    {
        // Do stuff
    }

    void Reshuffle()
    {
        drawnCards.Clear();
        deck.ShuffleAll();
    }

    void DrawCard()
    {
        Card? maybeCard = deck.DrawCard();
        if (maybeCard is null) return;
        Card card = (Card) maybeCard;

        drawnCards.Add(card);

        // Do any other stuff that needs doing
    }

    void StopDrawing()
    {
        turnState = TurnState.EnemyTurn;

        EvaluateAllCards();

        // Do any other stuff that needs doing
    }

    void EndBattle()
    {
        // Probably broadcast the result to some state object idk
    }
}
