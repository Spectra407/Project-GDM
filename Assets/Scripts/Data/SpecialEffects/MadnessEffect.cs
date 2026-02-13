using UnityEngine;
//sets madness to [madnessLevel]
public class MadnessEffect : SpecialEffect
{
    private int madnessLevel;
    public MadnessEffect(int madness)
    {
        madnessLevel = madness;
    }
    public void Execute(CombatManager cm, CardData card)
    {
        cm.madness = madnessLevel;
        cm.dem.CheckMadness();
    }
}
