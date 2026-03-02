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
        CardData copiedCard;
        
        // Index of the card that triggered the copy
        int index = cards.FindIndex(c => c.InstanceID == card.InstanceID);  

        if (index <= 0) 
        {
            Debug.Log("No valid card behind this instance.");
            return card;
        }
        
        
        if (cm.jackpot)
        {
            copiedCard = cards[index - 1].getJackpot();
        }
        else
        {
            copiedCard = cards[index - 1];
        }

        if (copiedCard.cardType.Contains(CardData.CardType.Bomb))
        {
            // Don't copy bombs
            Debug.Log("Previous card is a bomb, skip copy effect");
            return card;
        }
        Debug.Log("Copied card: " + copiedCard.cardName);
        // FIX NEEDED: NEED TO MAKE THE SPECIAL EFFECTS OF THE COPIED CARD TRIGGER
        return copiedCard;
        
    }
}