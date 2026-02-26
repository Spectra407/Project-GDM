using UnityEngine;
//gain [type1] equal to [type2]
//eg: gain defense equal to your current strength
public class EqualEffect : SpecialEffect
{
    CardData.CardType type1;
    CardData.CardType type2;

    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("EQ", args =>
            new EqualEffect(
                EffectRegistry.ParseType(args[0]),
                EffectRegistry.ParseType(args[1])
            )
        );
    }

    public EqualEffect(CardData.CardType t1, CardData.CardType t2)
    {
        type1 = t1;
        type2 = t2;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        //need to add check for dict
        cm.dem.PendingStats[type1] += cm.dem.PendingStats[type2];
        return card;
    }

}
