using UnityEngine;
//sets madness to [madnessLevel]
public class MadnessEffect : SpecialEffect
{
    private int madnessLevel;
    public void MadnessEffect(int madness)
    {
        madnessLevel = madness;
    }
    public abstract void Execute(CombatManager cm, CardData card)
    {
        cm.madness = madnessLevel;
        cm.dem.ProcessMadness();
    }
}
