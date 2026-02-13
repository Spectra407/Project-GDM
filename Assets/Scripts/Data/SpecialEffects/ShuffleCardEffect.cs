using UnityEngine;
using System;
//player shuffles [cardNum] cards back into their deck
public class ShuffleCardEffect : SpecialEffect
{
    private int cardNum;
    public ShuffleCardEffect(int num)
    {
        cardNum = num;
    }
    public void Execute(CombatManager cm, CardData card)
    {
        //does the player choose?
        int shuffleNum = Math.Max(cm.Hand.handCardViews.Count, cardNum); //assuming you can choose this card to shuffle back in as well, makes sure you cant shuffle in more than you have
        for (int i = 0; i < shuffleNum; i++)
        {
            Debug.Log("choose a card to shuffle into your deck");
            //again, need to implement this. detect a click and have it go back in. are there functions in deck manager for this?gi
        }
    }   
}
