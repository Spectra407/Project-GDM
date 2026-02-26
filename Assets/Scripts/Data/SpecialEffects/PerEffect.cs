using UnityEngine;
//adds [multipler] x [stat] per card in your hand
//eg: 2 damage per card in hand
public class PerEffect : SpecialEffect
{
    private int multiplier;
    CardData.CardType cardtype;
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("PER", args =>
            new PerEffect(
                EffectRegistry.ParseType(args[0]),
                int.Parse(args[1])
            )
        );
    }

    public PerEffect(CardData.CardType type, int mult)
    {
        multiplier = mult;
        cardtype = type;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        switch (cardtype)
        {
            case CardData.CardType.Damage:
                cm.dem.PendingDamage += cm.Hand.handCardViews.Count * multiplier;
                break;
            case CardData.CardType.Poison:
                cm.dem.PendingPoison += cm.Hand.handCardViews.Count * multiplier;
                break;
            case CardData.CardType.Strength:
                cm.dem.PendingStrength += cm.Hand.handCardViews.Count * multiplier;
                break;
            case CardData.CardType.Defense:
                cm.dem.PendingDefense += cm.Hand.handCardViews.Count * multiplier;
                break;
            default:
                Debug.Log("PerEffect given invalid CardType.");
                break;
        }
        return card;
    }
}
