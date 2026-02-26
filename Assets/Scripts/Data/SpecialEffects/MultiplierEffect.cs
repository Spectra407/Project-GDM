using UnityEngine;
//multiplies [stat] by [multiplier]
//eg: x2 damage
public class MultiplierEffect : SpecialEffect
{
    private int multiplier;
    CardData.CardType cardtype;

    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("MULT", args =>
            new MultiplierEffect(
                EffectRegistry.ParseType(args[0]),
                int.Parse(args[1])
            )
        );
    }

    public MultiplierEffect(CardData.CardType type, int mult)
    {
        Debug.Log("yurt");
        multiplier = mult;
        cardtype = type;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        switch (cardtype)
        {
            case CardData.CardType.Damage:
                cm.dem.PendingDamage *= multiplier;
                break;
            case CardData.CardType.Poison:
                cm.dem.PendingPoison *= multiplier;
                break;
            case CardData.CardType.Strength:
                cm.dem.PendingStrength *= multiplier;
                break;
            case CardData.CardType.Defense:
                cm.dem.PendingDefense *= multiplier;
                break;
            default:
                Debug.Log("MultiplierEffect given invalid CardType.");
                break;
        }
        return card;
    }

}
