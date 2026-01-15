using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public int madnessValue;
    public int damageValue;
    public int blockValue;
    public int poisonValue;
    public int poisonMultiplier;
    public int peekValue;

    public enum CardType
    {
        White,
        Silver,
        Gold
    }
    
    // Add values for the Jackpot
}
