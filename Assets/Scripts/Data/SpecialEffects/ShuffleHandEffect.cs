using System.Collections;
using Data.SpecialEffects;
using UnityEngine;
//lets player choose if they would like to shuffle their whole hand back into their deck
public class ShuffleHandEffect : SpecialEffect
{
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("SHUFFLEHAND", args =>
            new ShuffleHandEffect()
        );
    }
    public ShuffleHandEffect()
    {
        //nothing to init
    }

    public CardData Execute(CombatManager cm, CardData card)
    {
        if (cm.dem.isRecalculating) return card;
        cm.StartCoroutine(DelayedShuffle(cm));
        return card;
    }

    private IEnumerator DelayedShuffle(CombatManager cm)
    {
        yield return new WaitForEndOfFrame();
        cm.MoveToNewState("ShufflingCard");
    }
}
