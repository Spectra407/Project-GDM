using UnityEngine;
using System.Collections.Generic;
using Data.SpecialEffects;

//copies effect of previous card
public class CopyEffect : SpecialEffect
{
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("COPY", args =>
            new CopyEffect()
        );
    }
    public CopyEffect()
    {
        //nothing to init
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        List<CardData> cards = cm.Hand.GetHandData();

        if (cards.Count < 2) //no previous card to copy
        {
            Debug.Log("No card copied: not enough cards in hand.");
            return card;
        }
        else
        {
            CardData copiedCard;
            if (cm.jackpot)
            {
                copiedCard = cards[cards.Count - 2].getJackpot();
            }
            else
            {
                copiedCard = cards[cards.Count - 2];
            }
            Debug.Log("Copied card: " + copiedCard.cardName);
            //do we need to make sure to skip rest of current iteration?
            return copiedCard;
        }
    }
}