using UnityEngine;
//for the rest of the turn, can no longer gain [cardType] stat
//eg: no more defense for rest of the turn
public class DisableEffect : SpecialEffect
{
    private CardData.CardType cardType;
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("DIS", args =>
            new DisableEffect(
                EffectRegistry.ParseType(args[0])
            )
        );
    }
    public DisableEffect(CardData.CardType type)
    {
        cardType = type;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        Debug.Log(cardType + " disabled.");
        switch (cardType)
        {
            case CardData.CardType.Damage:
                cm.dem.DamageMult = 0;
                break;
            case CardData.CardType.Poison:
                cm.dem.PoisonMult = 0;
                break;
            case CardData.CardType.Strength:
                cm.dem.StrengthMult = 0;
                break;
            case CardData.CardType.Defense:
                cm.dem.DefenseMult = 0;
                break;
            default:
                Debug.Log("DisableEffect given invalid CardType.");
                break;
        }
        return card;
    }
}
