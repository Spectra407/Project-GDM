using Data.SpecialEffects;
using UnityEngine;
using Systems;
//adds [multipler] x [stat] per card in your hand
//eg: 2 damage per card in hand
public class PerEffect : SpecialEffect
{
    internal int percard_multiplier;
    internal CardData.CardType cardtype;
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
        percard_multiplier = mult;
        cardtype = type;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        cm.dem.PerStats[cardtype] += percard_multiplier * cm.dem.DisStats[cardtype];
        //update attack count
        if (cm.dem.DisStats[CardData.CardType.Damage] > 0 && cardtype is CardData.CardType.Damage)
        {
            cm.dem.AttackNum++;
        }
        return card;
    }
    

}
