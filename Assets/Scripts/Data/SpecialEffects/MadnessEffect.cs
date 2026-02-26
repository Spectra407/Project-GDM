using UnityEngine;
//sets madness to [madnessLevel]
public class MadnessEffect : SpecialEffect
{
    private int madnessLevel;
    [RuntimeInitializeOnLoadMethod]
    static void Register()
    {
        EffectRegistry.Register("MAD", args =>
            new MadnessEffect(
                int.Parse(args[0])
            )
        );
    }

    public MadnessEffect(int madness)
    {
        madnessLevel = madness;
    }
    public CardData Execute(CombatManager cm, CardData card)
    {
        cm.madness = madnessLevel;
        cm.dem.CheckMadness();
        return card;
    }
}
