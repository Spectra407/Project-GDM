using Systems;
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
        //need to add check for dict
        Debug.Log(cardType + " disabled.");
        cm.dem.DisStats[cardType] = 0;
        return card;
    }
}
