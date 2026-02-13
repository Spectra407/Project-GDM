using UnityEngine;
//adds [multipler] x [stat] per card in your hand
//eg: 2 damage per card in hand
public class PerEffect : SpecialEffect
{
    private int multiplier;
    CardData.CardType cardtype;
    public PerEffect(int mult, CardData.CardType type)
    {
        multiplier = mult;
        cardtype = type;
    }
    public void Execute(CombatManager cm, CardData card)
    {
        switch (cardtype)
        {
            case CardData.CardType.Damage:
                cm.dem.PendingDamage += cm.Hand.handCardViews.Count * multiplier;
                break;
            //do this for other effects... 
        }
    }

}
