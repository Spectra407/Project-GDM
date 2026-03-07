using System.Collections;
using Data.SpecialEffects;
using UnityEngine;

public class ShuffleHandEffect : SpecialEffect
{
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("SHUFFLEHAND", args =>
            new ShuffleHandEffect()
        );
    }

    public ShuffleHandEffect() { }

    public CardData Execute(CombatManager cm, CardData card)
    {
        Debug.Log($"ShuffleHandEffect.Execute called. isRecalculating={cm.dem.isRecalculating}, isreshuffling={cm.dem.isreshuffling}");
        if (cm.dem.isRecalculating) return card;
        if (cm.dem.isreshuffling) return card;

        if (cm.currentState is HandlingCardState hcs)
            hcs.isShufflePending = true;

        cm.StartCoroutine(DelayedShuffleHand(cm));
        return card;
    }

    private IEnumerator DelayedShuffleHand(CombatManager cm)
    {
        yield return new WaitForEndOfFrame();
        cm.MoveToNewState("ShufflingHand");
    }
}