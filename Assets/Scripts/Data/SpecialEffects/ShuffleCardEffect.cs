using UnityEngine;
using System;
//player shuffles [cardNum] cards back into their deck
public class ShuffleCardEffect : SpecialEffect
{
    private int cardNum;
     [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("SHUFFLECARD", args =>
            new ShuffleCardEffect(
                int.Parse(args[0])
            )
        );
    }

    public ShuffleCardEffect(int num)
    {
        cardNum = num;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        //does the player choose?
        int shuffleNum = Math.Min(cm.Hand.handCardViews.Count, cardNum); //assuming you can choose this card to shuffle back in as well, makes sure you cant shuffle in more than you have
        for (int i = 0; i < shuffleNum; i++)
        {
            Debug.Log("choose a card to shuffle into your deck");
            
            //let player click card... wrap inside function later?

        }
        return card;
    }   
}
