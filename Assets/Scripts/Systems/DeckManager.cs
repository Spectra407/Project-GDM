using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Systems
{
    public class DeckManager : PersistentSingleton<DeckManager>
    {
        [SerializeField] private List<CardData> rewardDeck;   // Deck of possible reward cards
        [SerializeField] private List<CardData> currentDeck;  // Current player deck, start with the Starter Deck

        public CardDB cardDB; // Has all cards loaded in from database
    
        // In game combat list of peeked cards, and draw pile
        public List<CardData> peekList;
        public List<CardData> drawPile;
        [Header("Events")]
        public UnityEvent OnShuffle;
        public UnityEvent OnDraw;
    
        // Start() method to test
        void Start()
        {
            //for now, have currentDeck initialized with list of cards here (by cardID). Can store this in separate file later or have it as card metadata from csv.
            int[] cards = {0, 1, 2, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16,
            17, 18, 19, 20, 21, 22, 23, 24, 25, 27, 28, 29, 30};
            //to test out:
            //draw
            //shuffles
            for (int i = 0; i < cards.Length; i++)
            {
                currentDeck.Add(cardDB.cards[cards[i]]);
                Debug.Log("Added card" + i + " to deck");
            }
            SetDeck(currentDeck);
            Debug.Log("Finished setting up deck.");
        }
    
    
        // Set up the draw pile and copy the current deck at the start of combat
        public void SetDeck(List<CardData> deck)
        {
            drawPile = new List<CardData>(deck);
            ShuffleAll(drawPile);
        
            Debug.Log("Deck initialized with this amount of cards: " + drawPile.Count);
            OnShuffle.Invoke();
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

            OnDraw.Invoke();
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
