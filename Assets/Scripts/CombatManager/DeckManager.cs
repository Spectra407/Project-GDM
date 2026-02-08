using UnityEngine;
using System.Collections.Generic;

public struct Card
{
    public int madness;
} // Dummy name to compile

public class DeckManager : MonoBehaviour
{
    private List<Card> deck;
    private int depth;

    public void SetDeck(List<Card> deck)
    {
        this.deck = new List<Card>(deck);
    }

    public int CardCount()
    {
        return deck.Count - depth;
    }

    public Card? DrawTopCard()
    {
        if (CardCount() == 0)
        {
            Debug.LogWarning("Tried to draw top card while not enough cards to do so");
            return null;
        }

        Card top = deck[depth];
        depth++;
        return top;
    }

    // Draws nth card by swapping nth card with the current top card
    // Does not preserve order of the remaining cards
    public Card? DrawNthCard(int n)
    {
        if (CardCount() < n)
        {
            Debug.LogWarning("Tried to draw card #" + n + " while " + CardCount() + " cards in deck");
            return null;
        }

        // Swap nth card to top of deck
        Card nth = deck[depth + n];
        deck[depth + n] = deck[depth];
        deck[depth] = nth;

        depth++;
        return nth;
    }

    public List<Card> PeekNCards(int n)
    {
        int count = System.Math.Min(CardCount(), n);

        List<Card> peeked = new();
        for (int i = 0; i < count; i++)
        {
            peeked.Add(deck[depth + i]);
        }
        return peeked;
    }

    // Reshuffle all cards, including drawn cards, into the deck
    public void ShuffleAll()
    {
        depth = 0;
        ShuffleUndrawn();
    }

    // Shuffle undrawn cards, keeping already-drawn cards in a drawn state
    public void ShuffleUndrawn()
    {
        for (int i = deck.Count - 1; i > depth; i--)
        {
            int j = Random.Range(depth, i + 1);
            var tmp = deck[i];
            deck[i] = deck[j];
            deck[j] = tmp;
        }
    }
}
