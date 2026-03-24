using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Systems
{
    public class DeckManager : PersistentSingleton<DeckManager>
    {
        public bool _isInitialized = false;    // Turns to true during the first enemy encounter. First time setup of the starter deck.
        [SerializeField] public List<CardData> rewardDeck;   // Deck of possible reward cards
        [SerializeField] public List<CardData> currentDeck;  // Current player deck, start with the Starter Deck

        public CardDB cardDB; // Has all cards loaded in from database
    
        // In game combat list of peeked cards, and draw pile
        public List<CardData> peekList;
        public List<CardData> drawPile;
        
        [Header("Events")]
        public UnityEvent OnShuffle;
        public UnityEvent OnDraw;
        
        /*
        card effect testing summary:
        poison - GOOD
        strength - GOOD
        draw - GOOD
        copy - Copy card has to be unique! Can't have 2 copy cards in the same deck, logic too wonky to implement otherwise
        copy - GOOD
        per - GOOD, Have to double-check calculator visuals not updating properly on per card effects later -Kenny
        mult - GOOD
        madness - GOOD
        shuffle 1 card - GOOD
        shuffle hand - GOOD, Consider making the card exhaust itself, this is too strong of an effect by itself -Kenny
        disable - GOOD
        equal - GOOD
        */

        public void SetupDecks(List<CardData> allCards, List<int> bombIDs)
        {
            Debug.Log($"SetupDecks called. _isInitialized={_isInitialized}, currentDeck count={currentDeck.Count}");

            
            // Initialize starter deck only on first fight
            if (!_isInitialized)
            {
                // Reward deck setup
                foreach (CardData card in allCards)
                    if (card.cardID >= 6 && card.cardID < 31)   
                        // cardID 6 is right after the starter deck and bombs, card ID 31 is right before the Jackpot version of cards
                        rewardDeck.Add(card);
                ShuffleAll(rewardDeck);
                
                int[] starterIDs = {0, 1, 2};   // Player starter deck without bombs
                currentDeck = new List<CardData>();
                foreach (int id in starterIDs)
                    currentDeck.Add(allCards[id]);
        
                _isInitialized = true;
            }

            // Build the draw pile from currentDeck + this fight's bombs
            List<CardData> fightDeck = new List<CardData>(currentDeck);
            foreach (int id in bombIDs)
                fightDeck.Add(allCards[id]);

            SetDeck(fightDeck); // drawPile gets bombs, but currentDeck stays clean
            Debug.Log($"Fight deck: {fightDeck.Count} cards ({currentDeck.Count} permanent + {bombIDs.Count} bombs).");
        }
        
        // Set up the draw pile and copy the current deck at the start of combat
        public void SetDeck(List<CardData> deck)
        {
            drawPile = new List<CardData>(deck);
            ShuffleAll(drawPile);
        
            Debug.Log("Deck initialized with this amount of cards: " + drawPile.Count);
            OnShuffle.Invoke();     // Invoke SFX for deck shuffle
        }
    
        // Reshuffle all cards in the deck
        public void ShuffleAll(List<CardData> deck)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i+1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }
    
        // Draw the first card
        public CardData DrawCard()
        {
            if (drawPile.Count == 0)
            {
                Debug.Log("Tried to draw top card while not enough cards to do so.");
                return null;
            }
        
            CardData drawnCard = drawPile[0];
            drawPile.RemoveAt(0);   // Remove the card that was drawn from the draw pile
            
            OnDraw.Invoke();    // Invoke SFX for draw
            return drawnCard;
        }
    
        // Peek at n cards and return the list of peeked cards
        public List<CardData> PeekCards(int n)
        {
            peekList.Clear();
            
            // Adjust how many cards are peeked depending on how many cards are left in the deck
            int nCorrected = System.Math.Min(drawPile.Count, n);
        
            for (int i = 0; i < nCorrected; i++)
            {
                CardData peekedCard = drawPile[i];
                peekList.Add(peekedCard);
            }
            // Now that we have grabbed all the cards that will be peeked, we can preemptively shuffle the draw pile!
            ShuffleAll(drawPile);
            
            return peekList;
        }
        
        // Essentially peek 3 cards from the reward deck
        public List<CardData> GetRewardOffers(int count)
        {
            ShuffleAll(rewardDeck);
            
            // Reset reward deck if not enough cards
            if (rewardDeck.Count < count)
            {
                Debug.Log("Reward deck reset.");   // Doesn't really work rn, placeholder since game isn't long enough for this to matter.
            }

            List<CardData> offers = new List<CardData>();
            for (int i = 0; i < Mathf.Min(count, rewardDeck.Count); i++)
            {
                offers.Add(rewardDeck[i]);
            }

            return offers;
        }
        

        public void RemoveCardFromDrawPile(CardData card)
        {
            drawPile.Remove(card);
        }

        public void RecycleToDrawPile(CardData card)
        {
            drawPile.Add(card);
        }
    }
}
