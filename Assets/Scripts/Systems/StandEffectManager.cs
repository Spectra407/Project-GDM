using UnityEngine;

namespace Systems
{
    public class StandEffectManager : MonoBehaviour
    {
        // [SerializeField] private AliceData player;
        // [SerializeField] private EnemyData enemy;
        [SerializeField] private HandView handView;
        // [SerializeField] private DeckManager deckManager;
        
        public void ResolveOnStand(CardData cardData)
        {
            // Trigger on stand card effects (when you end your turn).
            switch (cardData.cardName)
            {
                case "Flamingo Mallet":
                    // Gain 2 Defense.
                    break;
                case "Hedgehog Mallet":
                    // Deal 8 Damage.
                    break;
                case "Starchy Dormouse":
                    // Gain 5 Defense.
                    break;
                case "Vorpal Snicker":
                    // Deal 14 Damage.
                    // FIGURE OUT JACKPOT, PERHAPS CHANGE THIS IF JACKPOT IS SATISFIED.
                    break;
                case "A Mad Tea Party":
                    // Apply 2 poison for every card in your hand.
                    break;
                case "Bitter Drip":
                    // Deal 5 Damage.
                    // Apply 4 poison.
                    break;
                case "Detached Head":
                    // Gain 2 Defense per card in your hand.
                    break;
                case "Floating Paw":
                    // Deal 6 Damage per card in hand.
                    break;
                case "Foresight":
                    // Gain 5 Defense.
                    break;
                case "Heart-Shaped Box":
                    // Gain 5 defense, jackpot gain 50 instead. 
                    // FIGURE OUT JACKPOT LATER
                    break;
                case "Insidious Brew":
                    // Apply 4 poison. Jackpot apply 10 instead.
                    // FIGURE OUT JACKPOT LATER
                    break;
                case "Off With Their Heads":
                    // Deal 20 Damage. Jackpot: deal 40 instead.
                    // FIGURE OUT JACKPOT LATER.
                    break;
                case "Poisoned Cocktail":
                    // Double the enemy's poison stacks. Jackpot: triple instead.
                    // FIGURE OUT JACKPOT LATER.
                    break;
                case "Pupa Shell":
                    // Gain 20 defense.
                    break;
                case "Red Paint":
                    // Deal 10 Damage, jackpot: Gain 10 defense.
                    // FIGURE OUT JACKPOT LATER.
                    break;
                case "Royal Decree":
                    // Deal 5 Damage. Jackpot: All cards deal double damage.
                    break;
                case "Shell Smash":
                    // Fetch Alice current defense.
                    // Deal Damage equal to your Defense.
                    break;
                default:
                    // Write a console log signaling that the card name is either incorrect or unknown to this system
                    Debug.Log("This card has no On Stand effects. If this is not the correct resolution, verify that the " +
                              "card name matches in the database and that it's implemented.");
                    break;
            }
        }
        
        /*
        public void DealDamage(int damage, EnemyData enemy)
        {
            // Deal damage to the enemy
        }
        */
        
        /*
        public void GainDefense(int defense, AliceData player)
        {
            // Alice gains defense.
        }
        */
        
        /*
        public void ApplyPoison(int poison, EnemyData enemy, bool multiplyOption)
        {
            // Apply Poison to enemy, enemy will take poison damage right before doing their attack!
            // If multiplyOption is true, then instead of incrementing poison we will multiply by the poison value provided.
        }
        */
        
    }
}
