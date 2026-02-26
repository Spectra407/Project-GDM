using Systems;
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
        //need to put check for dict
        cm.dem.PendingStats[cardtype] += cm.Hand.handCardViews.Count * multiplier;
        return card;
    }
}
