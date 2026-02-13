public abstract class SpecialEffect : ScriptableObject
{
    public abstract void Execute(CombatManager cm, CardData card);
}
/*
    idea: make a bunch of scripts for all the effects
    have dictionary effect ID : function that does something
    can parse out shit then

    MULT_DMG_5 -> look up MULT in dictionary to get function
    this function then takes field 1 (DMG) and multiplies if by field 2 (5)
    have an add effect function that cards run when loaded
    this parses out stuff and adds the features

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
