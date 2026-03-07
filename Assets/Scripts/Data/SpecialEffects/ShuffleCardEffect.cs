using UnityEngine;
using System;
using System.Collections;
using Data.SpecialEffects;

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
        if (cm.dem.isRecalculating) return card;
        if (cm.dem.isreshuffling) return card;
        
        if (cm.currentState is HandlingCardState hcs)
            hcs.isShufflePending = true;
    
        cm.StartCoroutine(DelayedShuffle(cm));
        return card;
    }

    private IEnumerator DelayedShuffle(CombatManager cm)
    {
        yield return new WaitForEndOfFrame();
        cm.MoveToNewState("ShufflingCard");
    }
}
