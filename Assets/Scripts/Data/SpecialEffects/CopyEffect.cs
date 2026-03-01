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
        
        // Index of the card that triggered the copy
        int index = cards.FindIndex(c => c.InstanceID == card.InstanceID);  

        if (index <= 0) 
        {
            Debug.Log("No valid card behind this instance.");
            return card;
        }
        else
        {
            CardData copiedCard;
            if (cm.jackpot)
            {
                copiedCard = cards[index - 1].getJackpot();
            }
            else
            {
                copiedCard = cards[index - 1];
            }
            Debug.Log("Copied card: " + copiedCard.cardName);
            return copiedCard;
        }
    }
}