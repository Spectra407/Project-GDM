using UnityEngine;
//multiplies [stat] by [multiplier]
//eg: x2 damage
public class MultiplierEffect : SpecialEffect
{
    private int multiplier;
    CardData.CardType cardtype;

/*
   static DamageEffect()
    {
        EffectRegistry.Register("DMG", args =>
            new DamageEffect(int.Parse(args[0])));
    }
*/
    public MultiplierEffect(int mult, CardData.CardType type)
    {
        multiplier = mult;
        cardtype = type;
    }
    public void Execute(CombatManager cm, CardData card)
    {
        switch (cardtype)
        {
            case CardData.CardType.Damage:
                cm.dem.PendingDamage *= multiplier;
                break;
            //do this for other effects... also need overall multiplier not at that instance maybe
        }
    }

}
