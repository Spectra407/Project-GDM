using UnityEngine;
using System.Collections.Generic;
using Systems; // Assuming your Singleton and AliceData are in this namespace

public class CombatManager : MonoBehaviour
{
    [Header("Player & Enemy Data")]
    public AliceData alice;
    public int currentHealth;
    public int enemyHealth = 50; 
    public int madness;

    [Header("State Tracking")]
    private ITurnState currentState; 
    private Stack<ITurnState> states = new Stack<ITurnState>();
    public string CurrentStateName { get; private set; }

    [Header("Active Card Data")]
    public CardData lastDrawnCard;

    // Singletons for easy access by states
    public DeckManager Deck => DeckManager.Instance;
    public HandView Hand => HandView.Instance;

    private bool _isTransitioning = false; // "Circuit breaker" to prevent infinite loops

    void Start()
    {
        // Initialize Alice's health from her ScriptableObject
        currentHealth = alice.currentHealth;
        madness = 0;

        // Kick off the game loop
        MoveToNewState("ChoosingAction");
    }

    void Update()
    {
        // Allow the active state to handle frame-by-frame logic (like Raycasting)
        if (currentState != null)
        {
            currentState.Update(); 
        }
    }

    // Standard entry point for UI Button clicks
    public void HandleInput(string input)
    {
        if (currentState != null)
        {
            currentState.HandleInput(input);
        }
    }

    /// <summary>
    /// REPLACES the current state. Use for most transitions (Hit -> Handling -> etc).
    /// This prevents the state stack from growing infinitely.
    /// </summary>
    public void MoveToNewState(string id)
    {
        // Remove the _isTransitioning check here if it's causing the freeze
    
        if (currentState != null)
        {
            currentState.Exit();
            if (states.Count > 0) states.Pop(); 
        }

        currentState = NewState(id);
        CurrentStateName = id;
        states.Push(currentState);
    
        Debug.Log($"CombatManager: Switched to {id}");
        currentState.Enter();
    }

    /// <summary>
    /// OVERLAYS a state. Use EXCLUSIVELY for the Peek mechanic.
    /// </summary>
    public void PushNewState(string id)
    {
        // We do NOT Exit or Pop the underlying state so we can return to it later
        currentState = NewState(id);
        CurrentStateName = id;
        states.Push(currentState);
        currentState.Enter();
        
        Debug.Log($"CombatManager: Pushed Overlay {id}");
    }

    /// <summary>
    /// REVERTS to the previous state. Called by PeekManager when a card is picked.
    /// </summary>
    public void ReturnToLastState()
    {
        Debug.Log($"CombatManager: ReturnToLastState called from {currentState.GetType().Name}");
        
        if (_isTransitioning || states.Count <= 1) return;

        _isTransitioning = true; 

        try 
        {
            if (currentState != null) currentState.Exit();
            states.Pop();
            
            currentState = states.Peek();
            CurrentStateName = currentState.GetType().Name; // Resuming the card check logic
            
            currentState.Enter(); 
        }
        finally 
        {
            _isTransitioning = false; 
        }
    }

    // The Factory: Maps strings to actual State Classes
    private ITurnState NewState(string id)
    {
        switch (id)
        {
            case "ChoosingAction":  return new ChoosingActionState(this);
            case "HandlingCard":   return new HandlingCardState(this);
            case "Peeking":        return new PeekingState(this);
            case "Shattering":     return new ShatteringState(this);
            case "EvaluatingCards": return new EvaluatingCardsState(this);
            case "EnemyTurn":       return new EnemyTurnState(this);
            default:
                Debug.LogError($"Unknown State ID: {id}");
                return new ChoosingActionState(this);
        }
    }
    
}