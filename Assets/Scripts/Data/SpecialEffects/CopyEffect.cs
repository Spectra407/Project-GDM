using UnityEngine;
using System.Collections.Generic;
using Data.SpecialEffects;

public class CopyEffect : SpecialEffect
{
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("COPY", args => new CopyEffect());
    }

    public CopyEffect() { }

    public CardData Execute(CombatManager cm, CardData card)
    {
        List<CardData> cards = cm.Hand.GetHandData();

        int index = cards.FindIndex(c => c.InstanceID == card.InstanceID);

        if (index <= 0)
        {
            Debug.Log("No valid card behind this instance.");
            return card;
        }

        CardData previousCard = cm.jackpot
            ? cards[index - 1].getJackpot()
            : cards[index - 1];

        if (previousCard.cardType.Contains(CardData.CardType.Bomb))
        {
            Debug.Log("Previous card is a bomb, skip copy effect.");
            return card;
        }

        Debug.Log("Copied card: " + previousCard.cardName);
        return previousCard;
    }
}