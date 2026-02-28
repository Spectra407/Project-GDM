// public abstract class SpecialEffect : ScriptableObject
// {
//     public abstract void Execute(CombatManager cm, CardData card);
// }
using UnityEngine;

public interface SpecialEffect
{
    CardData Execute(CombatManager cm, CardData card);
}

/*

    (0)Mult: multiplies card types value (damage, etc.) by [specialVal]
    (1)Per: adds [# of cards] x [specialVal] of card types value
    (2)Draw: draw [specialVal] cards
    (3)Disable: can no longer get any more of [value defined by specialVal]
    (4)Copy: copy previous card's effect
    (5)Equals: make cardType value equal to value defined by specialVal
    (6)Madness: make madness equal special val
    (7)Shuffle card: shuffle [val] cards from your hand into your deck
    (8)Shuffle hand: shuffle hand into your deck, if you would like


*/
