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
        int bonus;
        switch (type2)
        {
            case CardData.CardType.Damage:
                bonus = cm.dem.PendingDamage;
                break;
            case CardData.CardType.Poison:
                bonus = cm.dem.PendingPoison;
                break;
            case CardData.CardType.Strength:
                bonus = cm.dem.PendingStrength;
                break;
            case CardData.CardType.Defense:
                bonus = cm.dem.PendingDefense;
                break;
            default:
                Debug.Log("MultiplierEffect given invalid CardType.");
                return card;
        }

        switch (type1)
        {
            case CardData.CardType.Damage:
                cm.dem.PendingDamage += bonus;
                break;
            case CardData.CardType.Poison:
                cm.dem.PendingPoison += bonus;
                break;
            case CardData.CardType.Strength:
                cm.dem.PendingStrength += bonus;
                break;
            case CardData.CardType.Defense:
                cm.dem.PendingDefense += bonus;
                break;
            default:
                Debug.Log("MultiplierEffect given invalid CardType 1.");
                break;
        }
        return card;
    }

}
