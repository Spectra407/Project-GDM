using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
    public class DeckManagerKenny : PersistentSingleton<DeckManagerKenny>
    {
        public List<CardData> rewardDeck;   // Deck of possible reward cards
        public List<CardData> currentDeck;  // Current player deck, start with the Starter Deck
    
        // In game combat list of peeked cards, and draw pile
        public List<CardData> peekList;
        public List<CardData> drawPile;
    
        // Start() method to test
        void Start()
        {
            SetDeck(currentDeck);
        }
    
    
        // Set up the draw pile and copy the current deck at the start of combat
        public void SetDeck(List<CardData> deck)
        {
            drawPile = new List<CardData>(deck);
            ShuffleAll(drawPile);
        
            Debug.Log("Deck initialized with this amount of cards: " + drawPile.Count);
        }
    
        // Reshuffle all cards in the deck
        private void ShuffleAll(List<CardData> deck)
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
    
        // Cleanup function after peeking resolves
        // MIGHT REMOVE THIS
        // public void PeekCleanup()
        // {
        //     if (peekList.Count != 0)
        //     {
        //         for (int i = 0; i < peekList.Count; i++)
        //         {
        //             CardData card = peekList[i];
        //             drawPile.Add(card);     // Add the peeked card back in the draw pile
        //         }
        //     }
        //     peekList = new List<CardData>();    // Reset the peek list
        //     ShuffleAll(drawPile);       // Shuffle the draw pile
        // }

        public void RemoveCardFromDrawPile(CardData card)
        {
            drawPile.Remove(card);
        }
    
    
    }
}
