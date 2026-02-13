using UnityEngine;
//for the rest of the turn, can no longer gain [cardType] stat
//eg: no more defense for rest of the turn
public class DisableEffect : SpecialEffect
{
    private CardData.CardType cardType;
    public void DrawEffect(CardData.CardType type)
    {
        cardType = type;
    }
    public abstract void Execute(CombatManager cm, CardData card)
    {
        Debug.Log(cardType + " disabled.");
        //need to implement this. could maybe have a multiplier for each stat, and then make it so that additions on draw effect are multiplied by this. then set respective one to zero, and have reset set it back to one.
    }
}
