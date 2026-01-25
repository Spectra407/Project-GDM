using UnityEngine;

// This script will contain all the On Draw effects for every card, ex: peek and what not.
namespace Systems
{
    public class DrawEffectManager : MonoBehaviour
    {
        // [SerializeField] private AliceData player;
        [SerializeField] private HandView handView;
        // [SerializeField] private DeckManager deckManager;
        // [SerializeField] private ScryUI scryUI;
    
    
        public void ResolveOnDraw(CardData cardData)
        {
            // Check if Alice.freeze is 0, if yes increment madness before resolving any other on draw card effects, else lower FreezeMadness by 1.
        
            // Check for Shatter (POTENTIALLY?)
        
            // Trigger on draw card effects.
            switch (cardData.cardName)
            {
                case "Looking Glass":
                    // Peek 2.
                    break;
                case "Foresight":
                    // Peek 1.
                    break;
                case "Frozen Time":
                    // Freeze madness for 2 turns.
                    // Most likely I will do this as Alice.freeze = 2
                    break;
                case "Hookah Bubble":
                    // Max Madness +2.
                    break;
                case "Lucid Dream":
                    // Peek 4.
                    break;
                case "Mirror Mirror":
                    // Swap your current Madness with the Madness of the previous card.
                    break;
                case "Pocket Watch Reset":
                    // Reduce your current Madness by 5.
                    break;
                case "Relaxing Exhale":
                    // Reduce your current Madness by 3.
                    break;
                case "The Mushroom's Edge":
                    // Peek 3.
                    break;
                case "Veil of Illusions":
                    // For every 3 cards in your hand, reduce 1 Madness.
                    break;
                case "Wide Grin":
                    // Copy the effect of the previous card
                    // Maybe implement this by swapping the card data of the previous card with a madness cost of 0.
                    break;
                default:
                    // Write a console log signaling that the card name is either incorrect or unknown to this system
                    Debug.Log("This card has no On Draw effects. If this is not the correct resolution, verify that the " +
                              "card name matches in the database and that it's implemented.");
                    break;
            }
        }
        
        /*
        private void ResolvePeek(int peek, ScryUI scryUI)
        {
            // Draw X cards.
            // You may play 1 of them. You may pass.
            // Reshuffle the rest.
        }
        */
        
        /*
        private void IncrementMadness(int madness, AliceData player)
        {
            // Fetch Alice.freeze, if 0: Fetch Alice.madness and then += madness, else: decrement freeze.
        }
        */
    }
}
