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
        //need to put check for dict
        cm.dem.PendingStats[cardtype] *= multiplier;
        //update attack count
        if (cm.dem.DisStats[CardData.CardType.Damage] > 0 && cardtype is CardData.CardType.Damage)
        {
            cm.dem.AttackNum++;
        }

        return card;
    }

}
