using UnityEngine;
using System.Collections.Generic;

public struct Card {} // Dummy name to compile

public class DeckManager : MonoBehaviour
{
    private List<Card> deck;
    private int depth;

    public void SetDeck(List<Card> deck)
    {
        this.deck = new List<Card>(deck);
    }

    public bool CanDrawCard()
    {
        return depth < deck.Count;
    }

    public Card? DrawCard()
    {
        if (!CanDrawCard()) {
            Debug.LogWarning("Tried to draw a card while unbable to do so");
            return null;
        }

        Card top = deck[depth];
        this.depth++;
        return top;
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
