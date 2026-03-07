using Data.SpecialEffects;
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
    public CardData  Execute(CombatManager cm, CardData card)
    {
        Debug.Log($"MadnessEffect.Execute called. isRecalculating = {cm.dem.isRecalculating}");
        if (cm.dem.isRecalculating) return card;
        
        if (cm.madness != madnessLevel)
        {
            cm.madness = madnessLevel;
            //mASSUMING MADNESS NEVER EXCEEDS MAXMADNESS
            cm.dem.ProcessJackpot();
            //if madness decreases do we still play sound effect?
            cm.OnMirrorCrack.Invoke();
        }
        
        return card;
    }
}
